﻿using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.BarModels
{
    public class BarAddViewModel
    {
        [Required]
        [Display(Name = "Bar Number")]
        public string BarNumber { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Year")]
        [UIHint("YearDatePicker")]
        public int? Year { get; set; }

        [Required]
        [Display(Name = "Map to Bar number")]
        public string MapToBarNumber { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public int? Period { get; set; }
    }
}