using AnnualReports.Web.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AnnualReports.Web.ViewModels.BarModels
{
    public class BarsUploadViewModel
    {
        [AllowedFileExtension(".xlsx")]
        [Required(ErrorMessage = "Please select file to process")]
        [Display(Name = "Excel file")]
        [UIHint("FileUpload")]
        public HttpPostedFileBase ExcelFile { get; set; }

        [UIHint("YearMonthDatePicker")]
        [Display(Name = "Filter by Date")]
        [Required]
        public DateTime? Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Year is required.")]
        [UIHint("YearDatePicker")]
        [Display(Name = "Please select year")]
        public int? BarsYear { get; set; }

        public string DateAsMonthYear
        {
            get { return Date.HasValue ? Date.Value.ToString("MM/yyyy") : ""; }
        }
    }
}