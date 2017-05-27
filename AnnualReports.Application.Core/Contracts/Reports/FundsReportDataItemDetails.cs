using AnnualReports.Domain.Core.Contracts;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class FundsReportDataItemDetails
    {
        public string FundNumber { get; set; }

        public string FundDisplayName { get; set; }

        public string BarNumber { get; set; }

        public string BarDisplayName { get; set; }

        public int Year { get; set; }

        public string MCAG { get; set; }

        public List<FundsReportDataRow> Rows { get; set; }

        public decimal Amount { get; set; }
    }
}