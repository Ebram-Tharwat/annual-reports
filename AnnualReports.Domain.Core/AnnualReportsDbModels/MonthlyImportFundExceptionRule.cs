using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
    [Table("FundExceptionRules")]
    public class MonthlyImportFundExceptionRule
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Original Fund Id")]
        public string OriginalFundId { get; set; }

        [Display(Name = "New Fund Id that will replace original fund id when applying rules")]
        public string NewFundId { get; set; }

    }
}
