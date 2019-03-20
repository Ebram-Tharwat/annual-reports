using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public abstract class AuditorMasterProcessor
    {
        public abstract IEnumerable<JournalVoucherReportOutputItem> Process(
            Stream inputStream,
            int year,
            JournalVoucherMatchingResultBuilder matchingResultBuilder, List<MonthlyImportFundExceptionRule> exceptionRules);

        protected JournalVoucherReportOutputItem CreateDebitJournalVoucherOutputItem(
           string accountNumber,
           string description,
           decimal entryValue,
           JournalVoucherType journalVoucher,
           DbSource dbSource)
        {
            return new JournalVoucherReportOutputItem()
            {
                AccountNumber = accountNumber,
                Description = description,
                Debit = Math.Abs(entryValue),
                Credit = 0,
                JournalVoucher = journalVoucher,
                DbSource = dbSource
            };
        }

        protected JournalVoucherReportOutputItem CreateCreditJournalVoucherOutputItem(
           string accountNumber,
           string description,
           decimal entryValue,
           JournalVoucherType journalVoucher,
           DbSource dbSource)
        {
            return new JournalVoucherReportOutputItem()
            {
                AccountNumber = accountNumber,
                Description = description,
                Debit = 0,
                Credit = Math.Abs(entryValue),
                JournalVoucher = journalVoucher,
                DbSource = dbSource
            };
        }
    }
}