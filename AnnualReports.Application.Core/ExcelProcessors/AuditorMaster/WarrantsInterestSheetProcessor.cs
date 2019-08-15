using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers.AuditorMaster;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Utils;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.DistDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public class WarrantsInterestSheetProcessor : AuditorMasterProcessor
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IGcDbFundRepository _gcDbFundRepo;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IReportService _reportService;
        private const string _sheetName = "WarrantInt";

        public WarrantsInterestSheetProcessor(
            IAnnualReportsDbFundRepository fundsRepository,
            IGcDbFundRepository gcDbFundRepo,
            IDistDbFundRepository distDbFundRepo,
            IReportService reportService)
        {
            _fundsRepository = fundsRepository;
            _gcDbFundRepo = gcDbFundRepo;
            _distDbFundRepo = distDbFundRepo;
            _reportService = reportService;
        }

        public override IEnumerable<JournalVoucherReportOutputItem> Process(
            Stream inputStream,
            int year,
            JournalVoucherMatchingResultBuilder matchingResultBuilder, List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            const int warrantsInterestSheetIndex = 4;
            var warrantsInterestSheetInputItems = WarrantInterestSheetParser.Parse(inputStream, warrantsInterestSheetIndex,exceptionRules);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var warrantInterestInput in warrantsInterestSheetInputItems)
            {
                var primaryFundId = new string(warrantInterestInput.FundId.Take(3).ToArray());
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);
                if (existingFund == null)
                {
                    matchingResultBuilder.ReportUnmatchedFund(warrantInterestInput.RowIndex, _sheetName, primaryFundId);
                    continue;
                }

                if (existingFund.DbSource == DbSource.GC)
                {
                    var gcFunds = _gcDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(warrantInterestInput.FundId,primaryFundId, gcFunds, warrantInterestInput, matchingResultBuilder));
                }
                else
                {
                    primaryFundId = warrantInterestInput.FundId.Replace("-", "").Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(warrantInterestInput.FundId,primaryFundId, distFunds, warrantInterestInput, matchingResultBuilder));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
             string fundId,
             string primaryFundId,
             List<Domain.Core.GcDbModels.Gl00100> gcFunds,
             WarrantsInterestSheetInputItem input,
             JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                    input.FundId,primaryFundId, gcFunds, input.WarrantInterest, input.RowIndex,input.IsExceptionRuleMatched, JournalVoucherType.WarrantInterest, matchingResultBuilder));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string fundId,
            string primaryFundId,
            List<Domain.Core.GcDbModels.Gl00100> gcFunds,
            decimal entryValue,
            int entryRowIndex,
            bool isExceptionRule,
            JournalVoucherType journalVoucher,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForWarrantInterest(primaryFundId,entryValue);

            var debitFund = gcFunds.FirstOrDefault(t => t.Actnumbr5.Trim() == debitFundId);
            var creditFund = gcFunds.FirstOrDefault(t => t.Actnumbr5.Trim() == creditFundId);

            if (debitFund == null)
            {
                matchingResultBuilder.ReportUnmatchedGcFund(entryRowIndex, _sheetName, primaryFundId, journalVoucher, debitFundId);
                return Enumerable.Empty<JournalVoucherReportOutputItem>();
            }

            if (creditFund == null)
            {
                matchingResultBuilder.ReportUnmatchedGcFund(entryRowIndex, _sheetName, primaryFundId, journalVoucher, creditFundId);
                return Enumerable.Empty<JournalVoucherReportOutputItem>();
            }

            string debitAccountNumber = $"{primaryFundId}.{debitFund.Actnumbr2.Trim()}.{debitFund.Actnumbr3.Trim()}.{debitFund.Actnumbr4.Trim()}.{debitFundId}";

            if (isExceptionRule)
            {
                fundId = fundId.Replace('-', '.');
                debitAccountNumber = $"{fundId}.{debitFundId}";
            }

            string creditAccountNumber = $"{primaryFundId}.{creditFund.Actnumbr2.Trim()}.{debitFund.Actnumbr3.Trim()}.{debitFund.Actnumbr4.Trim()}.{creditFundId}";

            if (isExceptionRule)
            {
                fundId = fundId.Replace('-', '.');
                creditAccountNumber = $"{fundId}.{creditFundId}";
            }

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund.Actdescr.Trim(), entryValue, journalVoucher, DbSource.GC),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund.Actdescr.Trim(), entryValue, journalVoucher, DbSource.GC)
            };
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string fundId,
            string primaryFundId,
            List<Gl00100> distFunds,
            WarrantsInterestSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                   fundId, primaryFundId, distFunds, input.WarrantInterest, input.RowIndex,input.IsExceptionRuleMatched, JournalVoucherType.WarrantInterest, matchingResultBuilder));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string fundId,
            string primaryFundId,
            List<Gl00100> distFunds,
            decimal entryValue,
            int entryRowIndex,
            bool isExceptionRule,
            JournalVoucherType journalVoucher,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForWarrantInterest(primaryFundId,entryValue);

            var debitFund = distFunds.FirstOrDefault(t => t.Actnumbr3.Trim() == debitFundId);
            var creditFund = distFunds.FirstOrDefault(t => t.Actnumbr3.Trim() == creditFundId);
            if (debitFund == null)
            {
                matchingResultBuilder.ReportUnmatchedDistFund(entryRowIndex, _sheetName, primaryFundId, journalVoucher, debitFundId);
                return Enumerable.Empty<JournalVoucherReportOutputItem>();
            }

            if (creditFund == null)
            {
                matchingResultBuilder.ReportUnmatchedDistFund(entryRowIndex, _sheetName, primaryFundId, journalVoucher, creditFundId);
                return Enumerable.Empty<JournalVoucherReportOutputItem>();
            }

            string debitAccountNumber = $"{primaryFundId}.{debitFund.Actnumbr2.Trim()}.{debitFundId}";

            if (isExceptionRule)
            {
                fundId = fundId.Replace('-', '.');
                if (fundId.IndexOf('.') == 3)
                {
                    fundId = fundId.Remove(3, 1);
                }
                debitAccountNumber = $"{fundId}.{debitFundId}";
            }

            string creditAccountNumber = $"{primaryFundId}.{creditFund.Actnumbr2.Trim()}.{creditFundId}";

            if (isExceptionRule)
            {
                fundId = fundId.Replace('-', '.');
                if (fundId.IndexOf('.') == 3)
                {
                    fundId = fundId.Remove(3, 1);
                }
                creditAccountNumber = $"{fundId}.{creditFundId}";
            }

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund.Actdescr.Trim(), entryValue, journalVoucher, DbSource.DIST),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund.Actdescr.Trim(), entryValue, journalVoucher, DbSource.DIST)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForWarrantInterest(string primaryFundId,decimal entryValue)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.WarrantInterest, primaryFundId);
            return (entryValue > 0) ? (result.DebitAccount, result.CreditAccount) : (result.DebitExceptionNegative, result.CreditExceptionNegative);
        }
    }
}