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
            JournalVoucherMatchingResultBuilder matchingResultBuilder,
            List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            const int investmentsSheetIndex = 3;
            var investmentsSheetInputItems = InvestmentsSheetParser.Parse(inputStream, investmentsSheetIndex, exceptionRules);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var investmentInput in investmentsSheetInputItems)
            {
                var primaryFundId = investmentInput.IsExceptionRuleMatched ? investmentInput.FundId.Split('.')[0] : investmentInput.FundId.Split('-')[0];
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);
                if (existingFund == null)
                {
                    matchingResultBuilder.ReportUnmatchedFund(investmentInput.RowIndex, _sheetName, primaryFundId);
                    continue;
                }

                if (existingFund.DbSource == DbSource.GC)
                {
                    var gcFunds = _gcDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(investmentInput.FundId, primaryFundId, gcFunds, investmentInput, matchingResultBuilder));
                }
                else
                {
                    primaryFundId = Regex.Replace(investmentInput.FundId, @"[-.]", "")
                                         .Truncate(9)
                                         .Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(investmentInput.FundId, primaryFundId, distFunds, investmentInput, matchingResultBuilder));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string fundId,
            string primaryFundId,
            List<Domain.Core.GcDbModels.Gl00100> gcFunds,
            InvestmentsSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                    fundId, primaryFundId, gcFunds, input.Purchases, input.RowIndex, input.IsExceptionRuleMatched, JournalVoucherType.InvestmentPurchases, matchingResultBuilder));

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                    fundId, primaryFundId, gcFunds, input.Sales, input.RowIndex, input.IsExceptionRuleMatched, JournalVoucherType.InvestmentSales, matchingResultBuilder));

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                    fundId, primaryFundId, gcFunds, input.Interest, input.RowIndex, input.IsExceptionRuleMatched, JournalVoucherType.InvestmentInterest, matchingResultBuilder));

            return results;
        }

        private (string debit, string credit) GetDebitAndCreditForInvestmentJournalVoucher(JournalVoucherType journalVoucher, string primaryFundId, decimal entryValue)
        {
            string debitFundId = string.Empty;
            string creditFundId = string.Empty;

            switch (journalVoucher)
            {
                case JournalVoucherType.InvestmentPurchases:
                    return (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentPurchases(primaryFundId, entryValue);

                case JournalVoucherType.InvestmentSales:
                    return (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentSales(primaryFundId, entryValue);

                case JournalVoucherType.InvestmentInterest:
                    return (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentInterest(primaryFundId, entryValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(journalVoucher));
            }
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

            string debitFundId = string.Empty;
            string creditFundId = string.Empty;

            var getDebitAndCreditForInvestmentJournalVoucherResult = GetDebitAndCreditForInvestmentJournalVoucher(journalVoucher, primaryFundId, entryValue);
            debitFundId = getDebitAndCreditForInvestmentJournalVoucherResult.debit;
            creditFundId = getDebitAndCreditForInvestmentJournalVoucherResult.credit;
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
            InvestmentsSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                   fundId, primaryFundId, distFunds, input.IsExceptionRuleMatched, input.Purchases, input.RowIndex, JournalVoucherType.InvestmentPurchases, matchingResultBuilder));

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    fundId, primaryFundId, distFunds, input.IsExceptionRuleMatched, input.Sales, input.RowIndex, JournalVoucherType.InvestmentSales, matchingResultBuilder));

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    fundId, primaryFundId, distFunds, input.IsExceptionRuleMatched, input.Interest, input.RowIndex, JournalVoucherType.InvestmentInterest, matchingResultBuilder));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string fundId,
            string primaryFundId,
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            bool isExceptionRule,
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
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentPurchases(primaryFundId, entryValue);
                    break;

                case JournalVoucherType.InvestmentSales:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentSales(primaryFundId, entryValue);
                    break;

                case JournalVoucherType.InvestmentInterest:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForInvestmentInterest(primaryFundId, entryValue);
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
            if (isExceptionRule)
            {
                fundId = fundId.Replace('-', '.');
                if (fundId.IndexOf('.') == 3)
                {
                    fundId = fundId.Remove(3, 1);
                }
                debitAccountNumber = $"{fundId}.{debitFundId}";
            }
            string creditAccountNumber = $"{primaryFundId}.{creditFund?.Actnumbr2?.Trim()}.{creditFundId}";
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
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund?.Actdescr?.Trim(), entryValue, journalVoucher, DbSource.DIST),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund?.Actdescr?.Trim(), entryValue, journalVoucher, DbSource.DIST)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForInvestmentPurchases(string primaryFundId, decimal entryValue)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentPurchases, primaryFundId);
            return (entryValue > 0) ? (result.DebitAccount, result.CreditAccount) : (result.DebitExceptionNegative, result.CreditExceptionNegative);
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForInvestmentSales(string primaryFundId, decimal entryValue)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentSales, primaryFundId);
            return (entryValue > 0) ? (result.DebitAccount, result.CreditAccount) : (result.DebitExceptionNegative, result.CreditExceptionNegative);
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForInvestmentInterest(string primaryFundId, decimal entryValue)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentInterest, primaryFundId);
            return (entryValue > 0) ? (result.DebitAccount, result.CreditAccount) : (result.DebitExceptionNegative, result.CreditExceptionNegative);
        }
    }
}