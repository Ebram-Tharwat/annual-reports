using System.ComponentModel.DataAnnotations;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
    public class MappingRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public MappingRuleOperator Operator { get; set; }

        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        public string TargetBarNumber { get; set; }

        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        public string TargetFundNumber { get; set; }

        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        public string DebitBarNumber { get; set; }

        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        public string CreditBarNumber { get; set; }
    }
}