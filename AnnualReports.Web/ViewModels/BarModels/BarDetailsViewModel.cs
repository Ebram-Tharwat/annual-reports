using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AnnualReports.Web.ViewModels.BarModels
{
    public class BarDetailsViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Bar Number")]

        public int BarNumber { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Map to Bar number")]
        public int MapToBarId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public int? Period { get; set; }
    }
}