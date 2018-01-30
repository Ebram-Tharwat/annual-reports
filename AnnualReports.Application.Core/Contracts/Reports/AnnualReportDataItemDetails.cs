using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class AnnualReportDataItemDetails
    {
        public string FundNumber { get; set; }

        public string FundDisplayName { get; set; }

        public string BarNumber { get; set; }

        public string BarDisplayName { get; set; }

        public string MapToBarNumber { get; set; }

        public int Year { get; set; }

        public string MCAG { get; set; }

        public List<AnnualReportDataRow> Rows { get; set; }

        public decimal Amount { get; set; }
    }

    public class AnnualReportDataItemGroup
    {
        public string PrimaryFundNumber { get; set; }

        public string FundDisplayName { get; set; }

        public string MCAG { get; set; }

        public DbSource DbSource { get; set; }

        public List<AnnualReportDataRow> GroupData { get; set; }
    }
}