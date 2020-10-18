using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.BarModels
{
    public class BarsCopyViewModel
    {
        [Required(ErrorMessage = "The from year field is required.")]
        [UIHint("YearDatePicker")]
        [Display(Name = "Please select year to copy from")]
        public int? FromYear { get; set; }

        [Required(ErrorMessage = "The to year field is required.")]
        [UIHint("YearDatePicker")]
        [Display(Name = "Please select year to copy to")]
        public int? ToYear { get; set; }
    }
}