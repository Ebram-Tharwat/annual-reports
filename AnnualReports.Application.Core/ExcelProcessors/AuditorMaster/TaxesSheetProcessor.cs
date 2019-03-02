﻿using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelParsers.AuditorMaster;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public class TaxesSheetProcessor : AuditorMasterProcessor
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IReportService _reportService;
        private const string _sheetName = "Taxes";

        public TaxesSheetProcessor(
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

            const int taxesSheetIndex = 2;
            var taxesSheetInputItems = TaxesSheetParser.Parse(inputStream, taxesSheetIndex);
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
                    results.AddRange(CreateJournalVoucherOutputItemsForGcForTaxes(primaryFundId, existingFund.GpDescription, taxesInput));
                }
                else
                {
                    primaryFundId = taxesInput.FundId.Replace("-", "").Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(primaryFundId, distFunds, taxesInput, matchingResultBuilder));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGcForTaxes(
           string primaryFundId,
           string description,
           TaxesSheetInputItem input)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(CreateJournalVoucherOutputItemsForGcForTaxes(primaryFundId, description, input.Taxes, JournalVoucherType.Taxes));

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGcForTaxes(
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
            TaxesSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    primaryFundId, distFunds, input.Taxes, input.RowIndex, JournalVoucherType.Taxes, matchingResultBuilder));

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

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForTaxes();

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

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForTaxes()
        {
            var result = _reportService.GetMonthlyReportRule(JournalVoucherType.Taxes);
            return (result.DebitAccount, result.CreditAccount);
        }
    }
}