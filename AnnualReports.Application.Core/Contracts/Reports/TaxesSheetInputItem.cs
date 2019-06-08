namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class TaxesSheetInputItem
    {
        public int RowIndex { get; set; }
        public string FundId { get; set; }
        public string Name { get; set; }
        public decimal Taxes { get; set; }
        public bool IsExceptionRuleMatched { get; set; }

    }
}