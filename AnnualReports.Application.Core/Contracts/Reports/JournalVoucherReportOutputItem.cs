using AnnualReports.Domain.Core.AnnualReportsDbModels;

namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class JournalVoucherReportOutputItem
    {
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public JournalVoucherType JournalVoucher { get; set; }
    }
}