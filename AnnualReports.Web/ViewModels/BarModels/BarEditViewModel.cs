using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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
        public string MapToBarNumber { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public int? Period { get; set; }

        [Display(Name = "Database source")]
        public DbSource? DbSource { get; set; }

        public List<SelectListItem> AvailableDbSources { get; set; }
    }
}