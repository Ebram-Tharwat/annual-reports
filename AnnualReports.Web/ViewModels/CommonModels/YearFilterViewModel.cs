using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.CommonModels
{
    public class YearFilterViewModel
    {
        [Required(ErrorMessage = "Year is required.")]
        [UIHint("YearDatePicker")]
        [Display(Name = "Please select year")]
        public int? Year { get; set; }

        [Display(Name = "Display name")]
        public string DisplayName { get; set; }

        [Display(Name = "Bar Number")]
        public string BarNumber { get; set; }

        [Display(Name = "Fund Number")]
        public string FundNumber { get; set; }

        [Display(Name = "Database source")]
        public DbSource? DbSource { get; set; }

        public string DateAsYear
        {
            get { return Year.HasValue ? Year.Value.ToString() : ""; }
        }

        public bool IsEmpty
        {
            get
            {
                return (!Year.HasValue && string.IsNullOrWhiteSpace(DisplayName)
                  && string.IsNullOrWhiteSpace(BarNumber) && string.IsNullOrWhiteSpace(FundNumber)
                  && !DbSource.HasValue);
            }
        }
    }
}