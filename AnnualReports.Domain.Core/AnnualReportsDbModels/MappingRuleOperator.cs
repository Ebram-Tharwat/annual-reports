using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
    public enum MappingRuleOperator
    {
        [Display(Name = "Equal to")]
        Equal = 1,

        [Display(Name = "Starts with")]
        StartWith = 2,

        [Display(Name = "Ends with")]
        EndWith = 3
    }
}