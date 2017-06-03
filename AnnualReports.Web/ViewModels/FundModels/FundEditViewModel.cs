using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AnnualReports.Web.ViewModels.FundModels
{
    public class FundEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fund number")]
        public int FundNumber { get; set; }

        [Required]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; }

        [Required]
        public string MCAG { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Map to")]
        public int? MapToFundId { get; set; }

        public IEnumerable<SelectListItem> PrimaryFunds { get; set; }
    }
}