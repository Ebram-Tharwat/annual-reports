using System;
using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.CommonModels
{
    public class YearFilterViewModel
    {
        [Required(ErrorMessage = "Year is required.")]
        [UIHint("YearDatePicker")]
        [Display(Name = "Please select year")]
        public int? Year { get; set; } = DateTime.Now.Year;

        [Display(Name = "Display name")]
        public string DisplayName { get; set; }

        public string DateAsYear
        {
            get { return Year.HasValue ? Year.Value.ToString() : ""; }
        }
    }
}