using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers.AuditorMaster;
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

        public WarrantsSheetProcessor(IAnnualReportsDbFundRepository fundsRepository, IDistDbFundRepository distDbFundRepo)
        {
            _fundsRepository = fundsRepository;
            _distDbFundRepo = distDbFundRepo;
        }

        public override IEnumerable<JournalVoucherReportOutputItem> Process(Stream inputStream, int year)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            const int warrantsSheetIndex = 1;
            var warrantSheetInputItems = WarrantSheetParser.Parse(inputStream, warrantsSheetIndex);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var warrantInput in warrantSheetInputItems)
            {
                var primaryFundId = warrantInput.FundId.Split('-')[0];
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);

                if (existingFund.DbSource == DbSource.GC)
                {
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(primaryFundId, existingFund.GpDescription, warrantInput));
                }
                else
                {
                    primaryFundId = warrantInput.FundId.Replace("-", "").Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, warrantInput));
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
            WarrantsSheetInputItem warrantInput)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, warrantInput.Issues, JournalVoucherType.WarrantIssues));
            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, warrantInput.Presented, JournalVoucherType.WarrantPresented));
            results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, warrantInput.Cancels, JournalVoucherType.WarrantCancels));

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
                case JournalVoucherType.WarrantIssues:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForDistIssues();
                    break;

                case JournalVoucherType.WarrantPresented:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForDistPresented();
                    break;

                case JournalVoucherType.WarrantCancels:
                    (debitFundId, creditFundId) = GetDebitAndCreditFundIdsForDistCancels(entryValue);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(journalVoucher));
            }
            var debitFund = distFunds.FirstOrDefault(t => t.Actnumbr3.Trim() == debitFundId);
            var creditFund = distFunds.FirstOrDefault(t => t.Actnumbr3.Trim() == creditFundId);
            string debitAccountNumber = $"{primaryFundId}.{debitFund.Actnumbr2.Trim()}.{debitFundId}";
            string creditAccountNumber = $"{primaryFundId}.{creditFund.Actnumbr2.Trim()}.{creditFundId}";

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund.Actdescr.Trim(), entryValue, journalVoucher),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund.Actdescr.Trim(), entryValue, journalVoucher)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForDistIssues()
        {
            return ("229000000", "211000000");
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForDistPresented()
        {
            return ("211000000", "101000000");
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForDistCancels(decimal cancelsValue)
        {
            return (cancelsValue > 0)
                ? ("229000000", "211000000") : ("211000000", "229000000");
        }
    }
}