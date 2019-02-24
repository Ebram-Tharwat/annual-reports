using System.ComponentModel;

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

    public enum JournalVoucherType
    {
        [Description("Warrant Issues")]
        WarrantIssues,

        [Description("Warrant Presented")]
        WarrantPresented,

        [Description("Warrant Cancels")]
        WarrantCancels,

        [Description("Taxes")]
        Taxes,

        [Description("Investment Purchases")]
        InvestmentPurchases,

        [Description("Investment Sales")]
        InvestmentSales,

        [Description("Investment Interest")]
        InvestmentInterest,
    }
}