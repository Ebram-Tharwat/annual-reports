using AnnualReports.Web.Validations;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AnnualReports.Web.ViewModels.FundModels
{
    public class FundsUploadViewModel
    {
        [AllowedFileExtension(".xlsx")]
        [Required(ErrorMessage = "Please select file to process")]
        [Display(Name = "Funds excel file")]
        [UIHint("FileUpload")]
        public HttpPostedFileBase ExcelFile { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [UIHint("YearDatePicker")]
        [Display(Name = "Please select year")]
        public int? FundsYear { get; set; }
    }
}