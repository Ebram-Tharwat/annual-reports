using AnnualReports.Domain.Core.Contracts;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class DISTAnnualReportItem
    {
        public string BarNumber { get; set; }

        public decimal Amount { get; set; }

        public List<AnnualReportDataRow> Rows { get; set; }
    }
}