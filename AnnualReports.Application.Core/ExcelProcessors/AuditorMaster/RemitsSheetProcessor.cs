﻿using AnnualReports.Application.Core.Contracts.Reports;
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
    public class RemitsSheetProcessor : AuditorMasterProcessor
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IGcDbFundRepository _gcDbFundRepo;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IJournalVoucherRuleService _journalVoucherRuleService;
        private readonly string _sheetName = "Remits";

        public RemitsSheetProcessor(
            IAnnualReportsDbFundRepository fundsRepository,
            IGcDbFundRepository gcDbFundRepo,
            IDistDbFundRepository distDbFundRepo,
            IJournalVoucherRuleService journalVoucherRuleService)
        {
            _fundsRepository = fundsRepository;
            _gcDbFundRepo = gcDbFundRepo;
            _distDbFundRepo = distDbFundRepo;
            _journalVoucherRuleService = journalVoucherRuleService;
        }

        public override IEnumerable<JournalVoucherReportOutputItem> Process(
            Stream inputStream,
            int year,
            JournalVoucherMatchingResultBuilder matchingResultBuilder,
            List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            const int remitsSheetIndex = 5;
            var remitsSheetInputItems = RemitSheetParser.Parse(inputStream, remitsSheetIndex, exceptionRules);
            var funds = _fundsRepository.Get(t => t.Year == year).ToList();

            foreach (var remitInput in remitsSheetInputItems)
            {
                var primaryFundId = new string(remitInput.FundId.Take(3).ToArray());
                var existingFund = funds.FirstOrDefault(t => t.FundNumber.Trim() == primaryFundId);
                if (existingFund == null)
                {
                    matchingResultBuilder.ReportUnmatchedFund(remitInput.RowIndex, _sheetName, primaryFundId);
                    continue;
                }

                if (existingFund.DbSource == DbSource.GC)
                {
                    var gcFunds = _gcDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();
                    results.AddRange(CreateJournalVoucherOutputItemsForGc(remitInput.FundId, primaryFundId, gcFunds, remitInput, matchingResultBuilder));
                }
                else
                {
                    primaryFundId = Regex.Replace(remitInput.FundId, @"[-.]", "")
                                         .Truncate(9)
                                         .Remove(6, 1);
                    var distFunds = _distDbFundRepo.Get(t => t.FundNumber.StartsWith(primaryFundId)).ToList();

                    results.AddRange(CreateJournalVoucherOutputItemsForDist(remitInput.FundId, primaryFundId, distFunds, remitInput, matchingResultBuilder));
                }
            }

            return results;
        }

        private IEnumerable<JournalVoucherReportOutputItem> CreateJournalVoucherOutputItemsForGc(
            string fundId,
            string primaryFundId,
            List<Domain.Core.GcDbModels.Gl00100> gcFunds,
            RemitsSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForGc(
                   fundId, primaryFundId, gcFunds, input.Remits, input.RowIndex, input.IsExceptionRuleMatched, JournalVoucherType.Remits, matchingResultBuilder));

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

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForRemit(primaryFundId, entryValue);

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
            RemitsSheetInputItem input,
            JournalVoucherMatchingResultBuilder matchingResultBuilder)
        {
            var results = new List<JournalVoucherReportOutputItem>();

            results.AddRange(
                CreateJournalVoucherOutputItemsForDist(
                    fundId, primaryFundId, distFunds, input.Remits, input.RowIndex, input.IsExceptionRuleMatched, JournalVoucherType.Remits, matchingResultBuilder));

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

            (string debitFundId, string creditFundId) = GetDebitAndCreditFundIdsForRemit(primaryFundId, entryValue);

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
                if (fundId.IndexOf('.') == 3)
                {
                    fundId = fundId.Remove(3, 1);
                }
                debitAccountNumber = $"{fundId}.{debitFundId}";
            }
            string creditAccountNumber = $"{primaryFundId}.{creditFund.Actnumbr2.Trim()}.{creditFundId}";

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
                CreateDebitJournalVoucherOutputItem(debitAccountNumber, debitFund.Actdescr.Trim(), entryValue, journalVoucher, DbSource.DIST),
                CreateCreditJournalVoucherOutputItem(creditAccountNumber, creditFund.Actdescr.Trim(), entryValue, journalVoucher, DbSource.DIST)
            };
        }

        private (string debitFundId, string creditFundId) GetDebitAndCreditFundIdsForRemit(string primaryFundId, decimal entryValue)
        {
            var result = _journalVoucherRuleService.GetMonthlyReportRule(JournalVoucherType.Remits, primaryFundId);
            return (entryValue > 0) ? (result.DebitAccount, result.CreditAccount) : (result.DebitExceptionNegative, result.CreditExceptionNegative);
        }
    }
}