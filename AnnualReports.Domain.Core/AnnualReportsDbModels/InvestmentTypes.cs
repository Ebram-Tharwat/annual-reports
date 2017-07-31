using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
   public class InvestmentTypes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        [StringLength(25)]
        public string Name { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
