namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class RemitsSheetInputItem
    {
        public int RowIndex { get; set; }
        public string FundId { get; set; }
        public string Name { get; set; }
        public decimal Remits { get; set; }
        public bool IsExceptionRuleMatched { get; set; }
    }
}