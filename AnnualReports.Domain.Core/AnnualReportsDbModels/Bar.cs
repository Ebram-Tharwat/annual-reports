﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnnualReports.Domain.Core.AnnualReportsDbModels
{
    public class Bar
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        public string BarNumber { get; set; }

        [MaxLength(200)]
        [Required]
        public string DisplayName { get; set; }

        [Column(TypeName = "smallint")]
        [Required]
        public Int16 Year { get; set; }

        public int? MapToBarId { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [ForeignKey("MapToBarId")]
        public Bar MapToBar { get; set; }
    }
}