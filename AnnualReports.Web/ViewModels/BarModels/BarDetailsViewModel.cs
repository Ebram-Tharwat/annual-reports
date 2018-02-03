using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.BarModels
{
    public class BarDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Bar Number")]
        public string BarNumber { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Map to Bar number")]
        public string MapToBarNumber { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public int? Period { get; set; }

        [Display(Name = "DB Source")]
        public DbSource? DbSource { get; set; }

        [Display(Name = "Debits/Credits")]
        public BarNumberTarget? BarTarget { get; set; }
    }
}