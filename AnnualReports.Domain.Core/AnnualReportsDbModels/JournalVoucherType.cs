using System.ComponentModel;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
    public enum JournalVoucherType
    {
        [Description("Warrant Issues")]
        WarrantIssues = 1,

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

        [Description("Warrant Interest")]
        WarrantInterest,

        [Description("Remits")]
        Remits,
    }
}