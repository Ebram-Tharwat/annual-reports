using AnnualReports.Common.Extensions;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.MappingRuleModels
{
    public class MappingRuleEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Year")]
        [UIHint("YearDatePicker")]
        public int? Year { get; set; }

        [Display(Name = "Target Fund Number")]
        [Required]
        public string TargetFundNumber { get; set; }

        [Display(Name = "Target Bar Number")]
        [Required]
        public string TargetBarNumber { get; set; }

        [Display(Name = "Account selector condition")]
        [Required]
        public MappingRuleOperator? Operator { get; set; }

        [Display(Name = "Mapping Bar for Debits")]
        [Required]
        public string DebitBarNumber { get; set; }

        [Display(Name = "Mapping Bar for Credits")]
        [Required]
        public string CreditBarNumber { get; set; }

        public string OperatorText
        {
            get
            {
                return $"{this.Operator.GetDisplayName()}";
            }
        }
    }
}