using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers.AuditorMaster;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.DistDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public class WarrantsInterestSheetProcessor : AuditorMasterProcessor
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IReportService _reportService;
        private const string _sheetName = "WarrantInt";

        public WarrantsInterestSheetProcessor(
            IAnnualReportsDbFundRepository fundsRepository,
            IDistDbFundRepository distDbFundRepo,
            IReportService reportService)
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

            const int warrantsInterestSheetIndex = 4;
            var warrantsInterestSheetInputItems = WarrantInterestSheetParser.Parse(inputStream, warrantsInterestSheetIndex);
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
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, existingFund.GpDescription, warrantInterestInput));
                }
                else
                {
                    primaryFundId = warrantInterestInput.FundId.Replace("-", "").Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, warrantInterestInput, matchingResultBuilder));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string primaryFundId,
            string gpDescription,
            WarrantsInterestSheetInputItem warrantInterest)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, gpDescription, warrantInterest.WarrantInterest, JournalVoucherType.WarrantInterest));

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
            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForWarrantInterest();
            string accountNumberDebit = $"{primaryFundId}.{restOfAccountNumber}.{debitFundId}";
            string accountNumberCredit = $"{primaryFundId}.{restOfAccountNumber}.{creditFundId}";

            return new[] {
                CreateDebitJournalVoucherOutputItem(accountNumberDebit, description.Trim(), entryValue, journalVoucher),
                CreateCreditJournalVoucherOutputItem(accountNumberCredit, description.Trim(), entryValue, journalVoucher)
            };
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string primaryFundId,
            List<Gl00100> distFunds,
            WarrantsInterestSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    primaryFundId, distFunds, input.WarrantInterest, input.RowIndex, JournalVoucherType.WarrantInterest, matchingResultBuilder));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDist(
            string primaryFundId,
            List<Gl00100> distFunds,
            decimal entryValue,
            int entryRowIndex,
            JournalVoucherType journalVoucher,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForWarrantInterest();

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
            string creditAccountNumber = $"{primaryFundId}.{creditFund.Actnumbr2.Trim()}.{creditFundId}";

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund.Actdescr.Trim(), entryValue, journalVoucher),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund.Actdescr.Trim(), entryValue, journalVoucher)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForWarrantInterest()
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.WarrantInterest);
            return (result.DebitAccount, result.CreditAccount);
        }
    }
}