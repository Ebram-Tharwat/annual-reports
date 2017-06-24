using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AnnualReports.Web.ViewModels.FundModels
{
    public class FundAddViewModel
    {
        [Required]
        [Display(Name = "Fund Number")]
        public string FundNumber { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Year")]
        [UIHint("YearDatePicker")]
        public int? Year { get; set; }

        [Required]
        public string MCAG { get; set; }

        [Required]
        [Display(Name = "Databse source")]
        public DbSource DbSource { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Map to Fund number")]
        public int? MapToFundId { get; set; }

        public List<SelectListItem> AvailableDbSources { get; set; }

        public List<SelectListItem> PrimaryFunds { get; set; }
    }
}