﻿namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class WarrantReportInputItem
    {
        public string FundId { get; set; }
        public string Name { get; set; }
        public decimal Issues { get; set; }
        public decimal Presented { get; set; }
        public decimal Cancels { get; set; }
    }
}