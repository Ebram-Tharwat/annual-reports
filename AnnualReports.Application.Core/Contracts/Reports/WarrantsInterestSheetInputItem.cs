namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class WarrantsInterestSheetInputItem
    {
        public int RowIndex { get; set; }
        public string FundId { get; set; }
        public string Name { get; set; }
        public decimal WarrantInterest { get; set; }
        public bool IsExceptionRuleMatched { get; set; }
    }
}