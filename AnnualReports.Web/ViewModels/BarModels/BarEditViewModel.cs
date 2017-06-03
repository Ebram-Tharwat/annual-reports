using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.BarModels
{
    public class BarEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Bar Number")]
        public string BarNumber { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Map to Bar number")]
        [MaxLength(9)]
        public string MapToBarNumber { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public int? Period { get; set; }
    }
}