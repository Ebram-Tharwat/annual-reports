using System;
using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.ReportModels
{
    public class MonthlyReportDataViewModel
    {
        [Display(Name = "Account")]
        public string AccountNumber { get; set; }

        public string Description { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public string Category { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}