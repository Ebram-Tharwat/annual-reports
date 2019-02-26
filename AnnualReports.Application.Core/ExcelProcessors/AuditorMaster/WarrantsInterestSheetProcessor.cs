using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers.AuditorMaster;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.DistDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public class WarrantsInterestSheetProcessor : AuditorMasterProcessor
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IDistDbFundRepository _distDbFundRepo;

        public WarrantsInterestSheetProcessor(IAnnualReportsDbFundRepository fundsRepository, IDistDbFundRepository distDbFundRepo)
        {
            _fundsRepository = fundsRepository;
            _distDbFundRepo = distDbFundRepo;
        }

        public override IEnumerable<JournalVoucherReportOutputItem> Process(Stream inputStream, int year)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            const int warrantsInterestSheetIndex = 4;
            var warrantsInterestSheetInputItems = WarrantInvestmentSheetParser.Parse(inputStream, warrantsInterestSheetIndex);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();
            foreach (var warrantInterestInput in warrantsInterestSheetInputItems)
            {
                var primaryFundId = new string(warrantInterestInput.FundId.Take(3).ToArray());
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);

                if (existingFund.DbSource == DbSource.GC)
                {
                    results.AddRange(CreateJournalVoucherOutputItemsForGcForWarrantsInterest(primaryFundId, existingFund.GpDescription, warrantInterestInput));
                }
                else
                {
                    primaryFundId = warrantInterestInput.FundId.Replace("-", "").Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDistForWarrantsInterest(primaryFundId, distFunds, warrantInterestInput));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDistForWarrantsInterest(string primaryFundId, List<Gl00100> distFunds,
                                                                                                           WarrantsInterestSheetInputItem input)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForDistForWarrantsInterest(primaryFundId, distFunds, input.WarrantInterest, JournalVoucherType.WarrantInterest));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForDistForWarrantsInterest(string primaryFundId, List<Gl00100> distFunds, 
                                                                                                           decimal entryValue, JournalVoucherType journalVoucher)
        {
            if (entryValue == 0)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForDistWarrants();

            var debitFund = distFunds.FirstOrDefault(t => t.Actnumbr3.Trim() == debitFundId);
            var creditFund = distFunds.FirstOrDefault(t => t.Actnumbr3.Trim() == creditFundId);
            if (debitFund == null || creditFund == null)
                return Enumerable.Empty<JournalVoucherReportOutputItem>();

            string debitAccountNumber = $"{primaryFundId}.{debitFund.Actnumbr2.Trim()}.{debitFundId}";
            string creditAccountNumber = $"{primaryFundId}.{creditFund.Actnumbr2.Trim()}.{creditFundId}";

            return new[] {
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund.Actdescr.Trim(), entryValue, journalVoucher),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund.Actdescr.Trim(), entryValue, journalVoucher)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForDistWarrants()
        {
            return ("299000000", "101000000");
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGcForWarrantsInterest(string primaryFundId, string gpDescription, 
                                                                                                         WarrantsInterestSheetInputItem warrantInterest)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForGcForWarrantsInterest(primaryFundId, gpDescription, warrantInterest.WarrantInterest, JournalVoucherType.WarrantInterest));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGcForWarrantsInterest(string primaryFundId, string description, decimal entryValue, 
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
    }
}
