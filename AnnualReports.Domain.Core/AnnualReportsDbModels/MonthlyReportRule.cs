using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
    [Table("DynamicRules")]
    public class MonthlyReportRule
    {
        [Key]
        
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name ="Jv Type")]
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
