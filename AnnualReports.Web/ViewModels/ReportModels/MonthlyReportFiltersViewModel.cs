using AnnualReports.Web.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AnnualReports.Web.ViewModels.ReportModels
{
    public class MonthlyReportGenerateViewModel
    {
        [Required(ErrorMessage = "Date is required.")]
        [UIHint("YearMonthDatePicker")]
        [Display(Name = "Please select date")]
        public DateTime? Date { get; set; } =  DateTime.Now;

        [AllowedFileExtension(".xlsx")]
        [Required(ErrorMessage = "Please select file to process")]
        [Display(Name = "Amounts excel file")]
        [UIHint("FileUpload")]
        public HttpPostedFileBase ExcelFile { get; set; }

        public string DateAsMonthYear
        {
            get { return Date.HasValue ? Date.Value.ToString("MM/yyyy") : ""; }
        }
    }
}