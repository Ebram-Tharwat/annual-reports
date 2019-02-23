namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class TaxesSheetInputItem
    {
        public string FundId { get; set; }
        public string Name { get; set; }
        public decimal Taxes { get; set; }
    }
}