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
        public string JvType { get; set; }

        [Required]
        [StringLength(10)]
        public string DebitAccount { get; set; }

        [Required]
        [StringLength(10)]
        public string CreditAccount { get; set; }

        [StringLength(10)]
        public string DebitExceptionNegative { get; set; }

        [StringLength(10)]
        public string CreditExceptionNegative { get; set; }
    }
}
