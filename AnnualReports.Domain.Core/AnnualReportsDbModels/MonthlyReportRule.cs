using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
    [Table("DynamicRules")]
    public class MonthlyReportRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "JV Type")]
        public JournalVoucherType JournalVoucherType { get; set; }

        [Display(Name = "Apply this rule for these fund Ids only")]
        public string FundIds { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Debit Account")]
        public string DebitAccount { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Credit Account")]
        public string CreditAccount { get; set; }

        [StringLength(10)]
        [Display(Name = "Debit Negative")]
        public string DebitExceptionNegative { get; set; }

        [StringLength(10)]
        [Display(Name = "Credit Account")]
        public string CreditExceptionNegative { get; set; }
    }
}