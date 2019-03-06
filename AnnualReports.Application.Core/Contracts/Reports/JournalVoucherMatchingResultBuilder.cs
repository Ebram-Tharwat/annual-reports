using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class JournalVoucherMatchingResultBuilder
    {
        public List<JournalVoucherUnamtchedResult> UnmatchedFunds { get; }

        public JournalVoucherMatchingResultBuilder()
        {
            UnmatchedFunds = new List<JournalVoucherUnamtchedResult>();
        }

        public void ReportUnmatchedFund(int rowIndex, string sheetName, string unmatchedFund)
        {
            UnmatchedFunds.Add(new JournalVoucherUnamtchedResult(rowIndex, unmatchedFund, sheetName, "Doesn't Exist", null, ""));
        }

        public void ReportUnmatchedGcFund(int rowIndex, string sheetName, string unmatchedFund, JournalVoucherType journalVoucher, string unmatchedJvRuleAccount)
        {
            UnmatchedFunds.Add(new JournalVoucherUnamtchedResult(rowIndex, unmatchedFund, sheetName, "GC", journalVoucher, unmatchedJvRuleAccount));
        }

        public void ReportUnmatchedDistFund(int rowIndex, string sheetName, string unmatchedFund, JournalVoucherType journalVoucher, string unmatchedJvRuleAccount)
        {
            UnmatchedFunds.Add(new JournalVoucherUnamtchedResult(rowIndex, unmatchedFund, sheetName, "DIST", journalVoucher, unmatchedJvRuleAccount));
        }

        public class JournalVoucherUnamtchedResult
        {
            public JournalVoucherUnamtchedResult(int rowIndex, string fundId, string sheetName, string database, JournalVoucherType? journalVoucher, string journalVoucherRuleAccount)
            {
                RowIndex = rowIndex;
                FundId = fundId;
                SheetName = sheetName;
                Database = database;
                JournalVoucher = journalVoucher;
                JournalVoucherRuleAccount = journalVoucherRuleAccount;
            }

            public int RowIndex { get; set; }
            public string FundId { get; set; }
            public string SheetName { get; set; }
            public string Database { get; set; }
            public JournalVoucherType? JournalVoucher { get; set; }
            public string JournalVoucherRuleAccount { get; set; }
        }
    }
}