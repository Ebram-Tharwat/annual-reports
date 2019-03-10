using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers.AuditorMaster;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Utils;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public class InvestmentsSheetProcessor : AuditorMasterProcessor
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IGcDbFundRepository _gcDbFundRepo;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IReportService _reportService;
        private const string _sheetName = "Investments";

        public InvestmentsSheetProcessor(
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
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            const int investmentsSheetIndex = 3;
            var investmentsSheetInputItems = InvestmentsSheetParser.Parse(inputStream, investmentsSheetIndex);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var investmentInput in investmentsSheetInputItems)
            {
                var primaryFundId = investmentInput.FundId.Split('-')[0];
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);
                if (existingFund == null)
                {
                    matchingResultBuilder.ReportUnmatchedFund(investmentInput.RowIndex, _sheetName, primaryFundId);
                    continue;
                }

                if (existingFund.DbSource == DbSource.GC)
                {
                    var gcFunds = _gcDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, gcFunds, investmentInput, matchingResultBuilder));
                }
                else
                {
                    primaryFundId = Regex.Replace(investmentInput.FundId, @"[-.]", "")
                                         .Truncate(9)
                                         .Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, investmentInput, matchingResultBuilder));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string primaryFundId,
            List<Domain.Core.GcDbModels.Gl00100> gcFunds,
            InvestmentsSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                    primaryFundId, gcFunds, input.Purchases, input.RowIndex, JournalVoucherType.InvestmentPurchases, matchingResultBuilder));

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                    primaryFundId, gcFunds, input.Sales, input.RowIndex, JournalVoucherType.InvestmentSales, matchingResultBuilder));

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                    primaryFundId, gcFunds, input.Interest, input.RowIndex, JournalVoucherType.InvestmentInterest, matchingResultBuilder));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string primaryFundId,
            List<Domain.Core.GcDbModels.Gl00100> gcFunds,
            decimal entryValue,
            int entryRowIndex,
            JournalVoucherType journalVoucher,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            string debitFundId = string.Empty;
            string creditFundId = string.Empty;

            switch (journalVoucher)
            {
                case JournalVoucherType.InvestmentPurchases:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentPurchases(primaryFundId);
                    break;

                case JournalVoucherType.InvestmentSales:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentSales(primaryFundId);
                    break;

                case JournalVoucherType.InvestmentInterest:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentInterest(primaryFundId);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(journalVoucher));
            }
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
            string creditAccountNumber = $"{primaryFundId}.{creditFund.Actnumbr2.Trim()}.{debitFund.Actnumbr3.Trim()}.{debitFund.Actnumbr4.Trim()}.{creditFundId}";

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund.Actdescr.Trim(), entryValue, journalVoucher),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund.Actdescr.Trim(), entryValue, journalVoucher)
            };
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string primaryFundId,
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            InvestmentsSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    primaryFundId, distFunds, input.Purchases, input.RowIndex, JournalVoucherType.InvestmentPurchases, matchingResultBuilder));

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    primaryFundId, distFunds, input.Sales, input.RowIndex, JournalVoucherType.InvestmentSales, matchingResultBuilder));

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    primaryFundId, distFunds, input.Interest, input.RowIndex, JournalVoucherType.InvestmentInterest, matchingResultBuilder));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string primaryFundId,
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            decimal entryValue,
            int entryRowIndex,
            JournalVoucherType journalVoucher,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            string debitFundId = string.Empty;
            string creditFundId = string.Empty;

            switch (journalVoucher)
            {
                case JournalVoucherType.InvestmentPurchases:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentPurchases(primaryFundId);
                    break;

                case JournalVoucherType.InvestmentSales:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentSales(primaryFundId);
                    break;

                case JournalVoucherType.InvestmentInterest:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentInterest(primaryFundId);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(journalVoucher));
            }
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

            string debitAccountNumber = $"{primaryFundId}.{debitFund?.Actnumbr2?.Trim()}.{debitFundId}";
            string creditAccountNumber = $"{primaryFundId}.{creditFund?.Actnumbr2?.Trim()}.{creditFundId}";

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund?.Actdescr?.Trim(), entryValue, journalVoucher),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund?.Actdescr?.Trim(), entryValue, journalVoucher)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForInvestmentPurchases(string primaryFundId)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentPurchases, primaryFundId);
            return (result.DebitAccount, result.CreditAccount);
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForInvestmentSales(string primaryFundId)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentSales, primaryFundId);
            return (result.DebitAccount, result.CreditAccount);
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForInvestmentInterest(string primaryFundId)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentInterest, primaryFundId);
            return (result.DebitAccount, result.CreditAccount);
        }
    }
}