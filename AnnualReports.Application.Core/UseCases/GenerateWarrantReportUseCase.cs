using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.UseCases
{
    public interface IGenerateWarrantReportUseCase
    {
        MemoryStream Execute(Stream inputStream, int year);
    }

    public class GenerateWarrantReportUseCase : IGenerateWarrantReportUseCase
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IExportingService _exportingService;

        public GenerateWarrantReportUseCase(
            IAnnualReportsDbFundRepository fundsRepository,
            IDistDbFundRepository distDbFundRepo,
            IExportingService exportingService)
        {
            _fundsRepository = fundsRepository;
            _distDbFundRepo = distDbFundRepo;
            _exportingService = exportingService;
        }

        public MemoryStream Execute(Stream inputStream, int year)
        {
            List<WarrantReportOutputItem> results = new List<WarrantReportOutputItem>();
            var warrantInputItems = WarrantSheetParser.Parse(inputStream);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var warrantInput in warrantInputItems)
            {
                var primaryFundId = warrantInput.FundId.Split('-')[0];
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);

                if (existingFund?.DbSource == DbSource.GC)
                {
                    results.AddRange(CreateWarrentReportOutputEntriesForGc(primaryFundId, existingFund.GpDescription, warrantInput));
                }
                else
                {
                    primaryFundId = warrantInput.FundId.Replace("-", "").Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateWarrentReportOutputEntriesForDist(primaryFundId, distFunds, warrantInput));
                }
            }

            return _exportingService.GetWarrantsReportExcel(results);
        }

        private IEnumerable<WarrantReportOutputItem> CreateWarrentReportOutputEntriesForGc(
            string primaryFundId,
            string description,
            WarrantReportInputItem warrantInput)
        {
            var results = new List<WarrantReportOutputItem>();

            results.AddRange(CreateWarrentReportOutputEntriesForGc(primaryFundId, description, warrantInput.Issues, JournalVoucherType.WarrantIssues));
            results.AddRange(CreateWarrentReportOutputEntriesForGc(primaryFundId, description, warrantInput.Presented, JournalVoucherType.WarrantPresented));
            results.AddRange(CreateWarrentReportOutputEntriesForGc(primaryFundId, description, warrantInput.Cancels, JournalVoucherType.WarrantCancels));

            return results;
        }

        private IEnumerable<WarrantReportOutputItem> CreateWarrentReportOutputEntriesForGc(
            string primaryFundId,
            string description,
            decimal entryValue,
            JournalVoucherType journalVoucher)
        {
            if (entryValue == 0)
                return Enumerable.Empty<WarrantReportOutputItem>();

            string restOfAccountNumber = "000.00.0000";
            string accountNumber = $"{primaryFundId}.{restOfAccountNumber}";
            return new[] {
                CreateDebitWarrantOutputEntry(accountNumber, description.Trim(), entryValue, journalVoucher),
                CreateCreditWarrantOutputEntry(accountNumber, description.Trim(), entryValue, journalVoucher)
            };
        }

        private IEnumerable<WarrantReportOutputItem> CreateWarrentReportOutputEntriesForDist(
            string primaryFundId,
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            WarrantReportInputItem warrantInput)
        {
            var results = new List<WarrantReportOutputItem>();

            results.AddRange(CreateWarrentReportOutputEntriesForDist(primaryFundId, distFunds, warrantInput.Issues, JournalVoucherType.WarrantIssues));
            results.AddRange(CreateWarrentReportOutputEntriesForDist(primaryFundId, distFunds, warrantInput.Presented, JournalVoucherType.WarrantPresented));
            results.AddRange(CreateWarrentReportOutputEntriesForDist(primaryFundId, distFunds, warrantInput.Cancels, JournalVoucherType.WarrantCancels));

            return results;
        }

        private IEnumerable<WarrantReportOutputItem> CreateWarrentReportOutputEntriesForDist(
            string primaryFundId,
            List<Domain.Core.DistDbModels.Gl00100> distFunds,
            decimal entryValue,
            JournalVoucherType journalVoucher)
        {
            if (entryValue == 0)
                return Enumerable.Empty<WarrantReportOutputItem>();

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
            string debitAccountNumber = $"{primaryFundId}.{debitFund?.Actnumbr2?.Trim()}.{debitFundId}";
            string creditAccountNumber = $"{primaryFundId}.{creditFund?.Actnumbr2?.Trim()}.{creditFundId}";

            return new[] {
                CreateDebitWarrantOutputEntry(debitAccountNumber, debitFund?.Actdescr?.Trim(), entryValue, journalVoucher),
                CreateCreditWarrantOutputEntry(creditAccountNumber, creditFund?.Actdescr?.Trim(), entryValue, journalVoucher)
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

        private WarrantReportOutputItem CreateDebitWarrantOutputEntry(
           string accountNumber,
           string description,
           decimal entryValue,
           JournalVoucherType journalVoucher)
        {
            return new WarrantReportOutputItem()
            {
                AccountNumber = accountNumber,
                Description = description,
                Debit = Math.Abs(entryValue),
                Credit = 0,
                JournalVoucher = journalVoucher
            };
        }

        private WarrantReportOutputItem CreateCreditWarrantOutputEntry(
           string accountNumber,
           string description,
           decimal entryValue,
           JournalVoucherType journalVoucher)
        {
            return new WarrantReportOutputItem()
            {
                AccountNumber = accountNumber,
                Description = description,
                Debit = 0,
                Credit = Math.Abs(entryValue),
                JournalVoucher = journalVoucher
            };
        }
    }
}