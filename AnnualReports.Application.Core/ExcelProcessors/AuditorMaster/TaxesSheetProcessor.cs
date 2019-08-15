using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers.AuditorMaster;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Utils;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public class TaxesSheetProcessor : AuditorMasterProcessor
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IGcDbFundRepository _gcDbFundRepo;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IReportService _reportService;
        private const string _sheetName = "Taxes";

        public TaxesSheetProcessor(
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

            const int taxesSheetIndex = 2;
            var taxesSheetInputItems = TaxesSheetParser.Parse(inputStream, taxesSheetIndex,exceptionRules);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var taxesInput in taxesSheetInputItems)
            {
                var primaryFundId = new string(taxesInput.FundId.Take(3).ToArray());
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);
                if (existingFund == null)
                {
                    matchingResultBuilder.ReportUnmatchedFund(taxesInput.RowIndex, _sheetName, primaryFundId);
                    continue;
                }

                if (existingFund.DbSource == DbSource.GC)
                {
                    var gcFunds = _gcDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(taxesInput.FundId,primaryFundId, gcFunds, taxesInput, matchingResultBuilder));
                }
                else
                {
                    primaryFundId = Regex.Replace(taxesInput.FundId, @"[-.]", "")
                                         .Truncate(9)
                                         .Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(taxesInput.FundId,primaryFundId, distFunds, taxesInput, matchingResultBuilder));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string fundId,
             string primaryFundId,
             List<Domain.Core.GcDbModels.Gl00100> gcFunds,
             TaxesSheetInputItem input,
             JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                    fundId,primaryFundId, gcFunds, input.Taxes, input.RowIndex,input.IsExceptionRuleMatched, JournalVoucherType.Taxes, matchingResultBuilder));

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

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForTaxes(primaryFundId,entryValue);

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
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            TaxesSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    fundId,primaryFundId, distFunds, input.Taxes, input.RowIndex,input.IsExceptionRuleMatched, JournalVoucherType.Taxes, matchingResultBuilder));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string fundId,
            string primaryFundId,
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            decimal entryValue,
            int entryRowIndex,
            bool isExceptionRule,
            JournalVoucherType journalVoucher,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForTaxes(primaryFundId,entryValue);

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
                debitAccountNumber = $"{fundId}.{debitFundId}";
            }

            string creditAccountNumber = $"{primaryFundId}.{creditFund.Actnumbr2.Trim()}.{creditFundId}";

            if (isExceptionRule)
            {
                fundId = fundId.Replace('-', '.');
                creditAccountNumber = $"{fundId}.{creditFundId}";
            }

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund.Actdescr.Trim(), entryValue, journalVoucher, DbSource.DIST),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund.Actdescr.Trim(), entryValue, journalVoucher, DbSource.DIST)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForTaxes(string primaryFundId,decimal entryValue)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.Taxes, primaryFundId.Truncate(3));
            return (entryValue > 0) ? (result.DebitAccount, result.CreditAccount) : (result.DebitExceptionNegative, result.CreditExceptionNegative);
        }
    }
}