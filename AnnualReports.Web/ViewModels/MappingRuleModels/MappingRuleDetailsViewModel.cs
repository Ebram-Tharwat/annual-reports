using AnnualReports.Common.Extensions;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Web.ViewModels.MappingRuleModels
{
    public class MappingRuleDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Bar selector condition")]
        public MappingRuleOperator Operator { get; set; }

        [Display(Name = "Target Bar Number")]
        public string TargetBarNumber { get; set; }

        [Display(Name = "Target Fund Number")]
        public string TargetFundNumber { get; set; }

        [Display(Name = "Mapping Bar for Debits")]
        public string DebitBarNumber { get; set; }

        [Display(Name = "Mapping Bar for Credits")]
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