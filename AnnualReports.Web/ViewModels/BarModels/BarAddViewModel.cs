﻿using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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

        [Display(Name = "Map to Bar number")]
        public string MapToBarNumber { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public int? Period { get; set; }

        [Display(Name = "DB source")]
        public DbSource? DbSource { get; set; }

        [Display(Name = "Debits or Credits")]
        public BarNumberTarget? BarTarget { get; set; }

        public List<SelectListItem> AvailableDbSources { get; set; }

        public List<SelectListItem> AvailableBarNumberTargets { get; set; }
    }
}