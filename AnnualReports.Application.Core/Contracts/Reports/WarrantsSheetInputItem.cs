namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class WarrantsSheetInputItem
    {
        public int RowIndex { get; set; }
        public string FundId { get; set; }
        public string Name { get; set; }
        public decimal Issues { get; set; }
        public decimal Presented { get; set; }
        public decimal Cancels { get; set; }
    }
}