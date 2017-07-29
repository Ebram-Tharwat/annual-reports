using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;

namespace AnnualReports.Domain.Core.Contracts
{
    public class AnnualReportDataRow
    {
        public string PrimaryFundNumber { get; set; }

        public Int16 Year { get; set; }

        public string FundDisplayName { get; set; }

        public string MCAG { get; set; }

        //public DbSource DbSource { get; set; }

        public Int16 View_Period { get; set; }

        public string View_FundNumber { get; set; }

        public string View_BarNumber { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public string ACTNUMBR_1 { get; set; }

        public string ACTNUMBR_2 { get; set; }

        public string ACTNUMBR_3 { get; set; }

        public string ACTNUMBR_4 { get; set; }

        public string ACTNUMBR_5 { get; set; }

        public string AccountDescription { get; set; }
    }
}