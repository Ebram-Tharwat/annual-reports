using System;

namespace AnnualReports.Domain.Core.Contracts
{
    public class FundsReportDataRow
    {
        public string PrimaryFundNumber { get; set; }

        public Int16 Year { get; set; }

        public string FundDisplayName { get; set; }

        public string MCAG { get; set; }

        public Int16 View_Period { get; set; }

        public string View_FundNumber { get; set; }

        public string View_BarNumber { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }
    }
}