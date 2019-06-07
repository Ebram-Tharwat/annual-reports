namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class InvestmentsSheetInputItem
    {
        public int RowIndex { get; set; }
        public string Name { get; set; }
        public string FundId { get; set; }
        public decimal Purchases { get; set; }
        public decimal Sales { get; set; }
        public decimal Interest { get; set; }

        public bool IsExceptionRuleMatched { get; set; }
    }
}