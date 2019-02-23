using AnnualReports.Application.Core.Contracts.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Application.Core.ExcelProcessors.AuditorMaster
{
    public abstract class AuditorMasterProcessor
    {
        public abstract IEnumerable<JournalVoucherReportOutputItem> Process(Stream inputStream, int year);

        protected JournalVoucherReportOutputItem CreateDebitJournalVoucherOutputItem(
           string accountNumber,
           string description,
           decimal entryValue,
           JournalVoucherType journalVoucher)
        {
            return new JournalVoucherReportOutputItem()
            {
                AccountNumber = accountNumber,
                Description = description,
                Debit = Math.Abs(entryValue),
                Credit = 0,
                JournalVoucher = journalVoucher
            };
        }

        protected JournalVoucherReportOutputItem CreateCreditJournalVoucherOutputItem(
           string accountNumber,
           string description,
           decimal entryValue,
           JournalVoucherType journalVoucher)
        {
            return new JournalVoucherReportOutputItem()
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
