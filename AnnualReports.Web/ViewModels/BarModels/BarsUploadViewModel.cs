using AnnualReports.Web.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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


        public string DateAsMonthYear
        {
            get { return Date.HasValue ? Date.Value.ToString("MM/yyyy") : ""; }
        }
    }
}