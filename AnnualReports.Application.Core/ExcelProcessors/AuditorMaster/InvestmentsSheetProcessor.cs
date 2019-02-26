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
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IReportService _reportService;

        public InvestmentsSheetProcessor(IAnnualReportsDbFundRepository fundsRepository, IDistDbFundRepository distDbFundRepo, IReportService reportService)
        {
            _fundsRepository = fundsRepository;
            _distDbFundRepo = distDbFundRepo;
            _reportService = reportService;
        }

        public override IEnumerable<JournalVoucherReportOutputItem> Process(Stream inputStream, int year)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            const int investmentsSheetIndex = 3;
            var investmentsSheetInputItems = InvestmentsSheetParser.Parse(inputStream, investmentsSheetIndex);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var investmentInput in investmentsSheetInputItems)
            {
                var primaryFundId = investmentInput.FundId.Split('-')[0];
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);

                if (existingFund?.DbSource == DbSource.GC)
                {
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, existingFund.GpDescription, investmentInput));
                }
                else
                {
                    primaryFundId = Regex.Replace(investmentInput.FundId, @"[-.]", "")
                                         .Truncate(9)
                                         .Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, investmentInput));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string primaryFundId,
            string description,
            InvestmentsSheetInputItem investmentInput)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, description, investmentInput.Purchases, JournalVoucherType.InvestmentPurchases));
            results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, description, investmentInput.Sales, JournalVoucherType.InvestmentSales));
            results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, description, investmentInput.Interest, JournalVoucherType.InvestmentInterest));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string primaryFundId,
            string description,
            decimal entryValue,
            JournalVoucherType journalVoucher)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            string restOfAccountNumber = "000.00.0000";
            string accountNumber = $"{primaryFundId}.{restOfAccountNumber}";
            return new[] {
                CreateDebitJournalVoucherOutputItem(accountNumber, description.Trim(), entryValue, journalVoucher),
                CreateCreditJournalVoucherOutputItem(accountNumber, description.Trim(), entryValue, journalVoucher)
            };
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string primaryFundId,
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            InvestmentsSheetInputItem investmentInput)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, investmentInput.Purchases, JournalVoucherType.InvestmentPurchases));
            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, investmentInput.Sales, JournalVoucherType.InvestmentSales));
            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, investmentInput.Interest, JournalVoucherType.InvestmentInterest));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string primaryFundId,
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            decimal entryValue,
            JournalVoucherType journalVoucher)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            string debitFundId = string.Empty;
            string creditFundId = string.Empty;

            switch (journalVoucher)
            {
                case JournalVoucherType.InvestmentPurchases:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForDistPurchases();
                    break;

                case JournalVoucherType.InvestmentSales:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForDistSales();
                    break;

                case JournalVoucherType.InvestmentInterest:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForDistInterest(entryValue);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(journalVoucher));
            }
            var debitFund = distFunds.FirstOrDefault(t => t.Actnumbr3.Trim() == debitFundId);
            var creditFund = distFunds.FirstOrDefault(t => t.Actnumbr3.Trim() == creditFundId);
            string debitAccountNumber = $"{primaryFundId}.{debitFund?.Actnumbr2?.Trim()}.{debitFundId}";
            string creditAccountNumber = $"{primaryFundId}.{creditFund?.Actnumbr2?.Trim()}.{creditFundId}";

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund?.Actdescr?.Trim(), entryValue, journalVoucher),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund?.Actdescr?.Trim(), entryValue, journalVoucher)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForDistPurchases()
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentPurchases);
            return (result?.DebitAccount, result?.CreditAccount);
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForDistSales()
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentSales);
            return (result?.DebitAccount, result?.CreditAccount);
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForDistInterest(decimal cancelsValue)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.InvestmentInterest);
            return (result?.DebitAccount, result?.CreditAccount);
        }
    }
}