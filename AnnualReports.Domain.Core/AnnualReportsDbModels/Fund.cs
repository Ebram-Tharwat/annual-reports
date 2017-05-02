using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
    public class Fund
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        public string FundNumber { get; set; }

        [Column(TypeName = "char")]
        [Required]
        [MaxLength(31)]
        [StringLength(31)]
        public string GpDescription { get; set; }

        [MaxLength(100)]
        [Required]
        public string DisplayName { get; set; }

        [Column(TypeName = "smallint")]
        [Required]
        public Int16 Year { get; set; }

        [MaxLength(10)]
        [Required]
        public string MCAG { get; set; }

        public int? MapToFundId { get; set; }

        [Required]
        public DbType Type { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [ForeignKey("MapToFundId")]
        public Fund MapToFund { get; set; }
    }
}