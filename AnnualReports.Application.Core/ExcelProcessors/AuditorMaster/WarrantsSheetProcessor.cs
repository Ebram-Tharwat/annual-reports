using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers.AuditorMaster;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public class WarrantsSheetProcessor : AuditorMasterProcessor
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IReportService _reportService;
        private const string _sheetName = "Warrants";

        public WarrantsSheetProcessor(IAnnualReportsDbFundRepository fundsRepository, IDistDbFundRepository distDbFundRepo, IReportService reportService)
        {
            _fundsRepository = fundsRepository;
            _distDbFundRepo = distDbFundRepo;
            _reportService = reportService;
        }

        public override IEnumerable<JournalVoucherReportOutputItem> Process(
            Stream inputStream,
            int year,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            const int warrantsSheetIndex = 1;
            var warrantSheetInputItems = WarrantSheetParser.Parse(inputStream, warrantsSheetIndex);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var warrantInput in warrantSheetInputItems)
            {
                var primaryFundId = warrantInput.FundId.Split('-')[0];
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);
                if (existingFund == null)
                {
                    matchingResultBuilder.ReportUnmatchedFund(warrantInput.RowIndex, _sheetName, primaryFundId);
                    continue;
                }

                if (existingFund?.DbSource == DbSource.GC)
                {
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, existingFund.GpDescription, warrantInput));
                }
                else
                {
                    primaryFundId = warrantInput.FundId.Replace("-", "").Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, warrantInput, matchingResultBuilder));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string primaryFundId,
            string description,
            WarrantsSheetInputItem warrantInput)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, description, warrantInput.Issues, JournalVoucherType.WarrantIssues));
            results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, description, warrantInput.Presented, JournalVoucherType.WarrantPresented));
            results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, description, warrantInput.Cancels, JournalVoucherType.WarrantCancels));

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
            WarrantsSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, input.Issues, input.RowIndex, JournalVoucherType.WarrantIssues, matchingResultBuilder));
            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, input.Presented, input.RowIndex, JournalVoucherType.WarrantPresented, matchingResultBuilder));
            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, input.Cancels, input.RowIndex, JournalVoucherType.WarrantCancels, matchingResultBuilder));

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
                case JournalVoucherType.WarrantIssues:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForWarrantIssues();
                    break;

                case JournalVoucherType.WarrantPresented:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForWarrantPresented();
                    break;

                case JournalVoucherType.WarrantCancels:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForWarrantCancels(entryValue);
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

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForWarrantIssues()
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.WarrantIssues);
            return (result?.DebitAccount, result?.CreditAccount);
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForWarrantPresented()
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.WarrantPresented);
            return (result?.DebitAccount, result?.CreditAccount);
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForWarrantCancels(decimal cancelsValue)
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.WarrantCancels);
            return (cancelsValue > 0) ? (result?.DebitAccount, result?.CreditAccount) : (result?.DebitExceptionNegative, result?.CreditExceptionNegative);
        }
    }
}