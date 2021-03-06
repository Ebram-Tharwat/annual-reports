// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.6
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnnualReports.Domain.Core.GcDbModels
{

    [Table("GL10111", Schema = "dbo")]
    public class Gl10111
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ACTINDX", Order = 1, TypeName = "int")]
        [Index(@"PKGL10111", 1, IsUnique = true, IsClustered = false)]
        [Required]
        [Key]
        [Display(Name = "Actindx")]
        public int Actindx { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"YEAR1", Order = 2, TypeName = "smallint")]
        [Index(@"PKGL10111", 2, IsUnique = true, IsClustered = false)]
        [Required]
        [Key]
        [Display(Name = "Year 1")]
        public short Year1 { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"PERIODID", Order = 3, TypeName = "smallint")]
        [Index(@"PKGL10111", 3, IsUnique = true, IsClustered = false)]
        [Required]
        [Key]
        [Display(Name = "Periodid")]
        public short Periodid { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"Ledger_ID", Order = 4, TypeName = "smallint")]
        [Index(@"PKGL10111", 4, IsUnique = true, IsClustered = false)]
        [Required]
        [Key]
        [Display(Name = "Ledger ID")]
        public short LedgerId { get; set; }

        [Column(@"ACTNUMBR_1", Order = 5, TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 1")]
        public string Actnumbr1 { get; set; }

        [Column(@"ACTNUMBR_2", Order = 6, TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 2")]
        public string Actnumbr2 { get; set; }

        [Column(@"ACTNUMBR_3", Order = 7, TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 3")]
        public string Actnumbr3 { get; set; }

        [Column(@"ACTNUMBR_4", Order = 8, TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 4")]
        public string Actnumbr4 { get; set; }

        [Column(@"ACTNUMBR_5", Order = 9, TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 5")]
        public string Actnumbr5 { get; set; }

        [Column(@"ACTNUMBR_6", Order = 10, TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 6")]
        public string Actnumbr6 { get; set; }

        [Column(@"ACTNUMBR_7", Order = 11, TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 7")]
        public string Actnumbr7 { get; set; }

        [Column(@"ACTNUMBR_8", Order = 12, TypeName = "char")]
        [Required]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 8")]
        public string Actnumbr8 { get; set; }

        [Column(@"ACCATNUM", Order = 13, TypeName = "smallint")]
        [Required]
        [Display(Name = "Accatnum")]
        public short Accatnum { get; set; }

        [Column(@"PERDBLNC", Order = 14, TypeName = "numeric")]
        [Required]
        [Display(Name = "Perdblnc")]
        public decimal Perdblnc { get; set; }

        [Column(@"DEBITAMT", Order = 15, TypeName = "numeric")]
        [Required]
        [Display(Name = "Debitamt")]
        public decimal Debitamt { get; set; }

        [Column(@"CRDTAMNT", Order = 16, TypeName = "numeric")]
        [Required]
        [Display(Name = "Crdtamnt")]
        public decimal Crdtamnt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"DEX_ROW_ID", Order = 17, TypeName = "int")]
        [Required]
        [Display(Name = "Dex row ID")]
        public int DexRowId { get; set; }
    }

}
// </auto-generated>
