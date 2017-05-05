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

    [Table("slbAccountSummary", Schema = "dbo")]
    public class SlbAccountSummary
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ACTINDX", Order = 1, TypeName = "int")]
        [Required]
        [Key]
        [Display(Name = "Actindx")]
        public int Actindx { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"YEAR1", Order = 2, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Year 1")]
        public short Year1 { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"PERIODID", Order = 3, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Periodid")]
        public short Periodid { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ASI_Document_Status", Order = 4, TypeName = "int")]
        [Required]
        [Key]
        [Display(Name = "Asi document status")]
        public int AsiDocumentStatus { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"PERDBLNC", Order = 5, TypeName = "numeric")]
        [Required]
        [Key]
        [Display(Name = "Perdblnc")]
        public decimal Perdblnc { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ACTNUMBR", Order = 6, TypeName = "int")]
        [Required]
        [Key]
        [Display(Name = "Actnumbr")]
        public int Actnumbr { get; set; }

        [Column(@"ACTDESCR", Order = 7, TypeName = "varchar")]
        [MaxLength(51)]
        [StringLength(51)]
        [Display(Name = "Actdescr")]
        public string Actdescr { get; set; }

        [Column(@"ACTALIAS", Order = 8, TypeName = "varchar")]
        [MaxLength(21)]
        [StringLength(21)]
        [Display(Name = "Actalias")]
        public string Actalias { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ACTIVE", Order = 9, TypeName = "tinyint")]
        [Required]
        [Key]
        [Display(Name = "Active")]
        public byte Active { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ACCATNUM", Order = 10, TypeName = "varchar")]
        [Required]
        [MaxLength(51)]
        [StringLength(51)]
        [Key]
        [Display(Name = "Accatnum")]
        public string Accatnum { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ACCTTYPE", Order = 11, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Accttype")]
        public short Accttype { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"PSTNGTYP", Order = 12, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Pstngtyp")]
        public short Pstngtyp { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"CRDTAMNT", Order = 13, TypeName = "numeric")]
        [Required]
        [Key]
        [Display(Name = "Crdtamnt")]
        public decimal Crdtamnt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"DEBITAMT", Order = 14, TypeName = "numeric")]
        [Required]
        [Key]
        [Display(Name = "Debitamt")]
        public decimal Debitamt { get; set; }

        [Column(@"MNACSGMT", Order = 15, TypeName = "varchar")]
        [MaxLength(67)]
        [StringLength(67)]
        [Display(Name = "Mnacsgmt")]
        public string Mnacsgmt { get; set; }

        [Column(@"ACTNUMBR_1", Order = 16, TypeName = "varchar")]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 1")]
        public string Actnumbr1 { get; set; }

        [Column(@"ACTNUMBR_2", Order = 17, TypeName = "varchar")]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 2")]
        public string Actnumbr2 { get; set; }

        [Column(@"ACTNUMBR_3", Order = 18, TypeName = "varchar")]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 3")]
        public string Actnumbr3 { get; set; }

        [Column(@"ACTNUMBR_4", Order = 19, TypeName = "varchar")]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 4")]
        public string Actnumbr4 { get; set; }

        [Column(@"ACTNUMBR_5", Order = 20, TypeName = "varchar")]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 5")]
        public string Actnumbr5 { get; set; }

        [Column(@"ACTNUMBR_6", Order = 21, TypeName = "varchar")]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 6")]
        public string Actnumbr6 { get; set; }

        [Column(@"ACTNUMBR_7", Order = 22, TypeName = "varchar")]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 7")]
        public string Actnumbr7 { get; set; }

        [Column(@"ACTNUMBR_8", Order = 23, TypeName = "varchar")]
        [MaxLength(9)]
        [StringLength(9)]
        [Display(Name = "Actnumbr 8")]
        public string Actnumbr8 { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"TPCLBLNC", Order = 24, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Tpclblnc")]
        public short Tpclblnc { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"DECPLACS", Order = 25, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Decplacs")]
        public short Decplacs { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"FXDORVAR", Order = 26, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Fxdorvar")]
        public short Fxdorvar { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"BALFRCLC", Order = 27, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Balfrclc")]
        public short Balfrclc { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"DSPLKUPS", Order = 28, TypeName = "binary")]
        [Required]
        [MaxLength(4)]
        [Key]
        [Display(Name = "Dsplkups")]
        public byte[] Dsplkups { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"CNVRMTHD", Order = 29, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Cnvrmthd")]
        public short Cnvrmthd { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"HSTRCLRT", Order = 30, TypeName = "numeric")]
        [Required]
        [Key]
        [Display(Name = "Hstrclrt")]
        public decimal Hstrclrt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"NOTEINDX", Order = 31, TypeName = "numeric")]
        [Required]
        [Key]
        [Display(Name = "Noteindx")]
        public decimal Noteindx { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"CREATDDT", Order = 32, TypeName = "datetime")]
        [Required]
        [Key]
        [Display(Name = "Creatddt")]
        public System.DateTime Creatddt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"MODIFDT", Order = 33, TypeName = "datetime")]
        [Required]
        [Key]
        [Display(Name = "Modifdt")]
        public System.DateTime Modifdt { get; set; }

        [Column(@"USERDEF1", Order = 34, TypeName = "varchar")]
        [MaxLength(21)]
        [StringLength(21)]
        [Display(Name = "Userdef 1")]
        public string Userdef1 { get; set; }

        [Column(@"USERDEF2", Order = 35, TypeName = "varchar")]
        [MaxLength(21)]
        [StringLength(21)]
        [Display(Name = "Userdef 2")]
        public string Userdef2 { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"PostSlsIn", Order = 36, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Post sls in")]
        public short PostSlsIn { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"PostIvIn", Order = 37, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Post iv in")]
        public short PostIvIn { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"PostPurchIn", Order = 38, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Post purch in")]
        public short PostPurchIn { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"PostPRIn", Order = 39, TypeName = "smallint")]
        [Required]
        [Key]
        [Display(Name = "Post pri n")]
        public short PostPrIn { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"ADJINFL", Order = 40, TypeName = "tinyint")]
        [Required]
        [Key]
        [Display(Name = "Adjinfl")]
        public byte Adjinfl { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"INFLAEQU", Order = 41, TypeName = "int")]
        [Required]
        [Key]
        [Display(Name = "Inflaequ")]
        public int Inflaequ { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(@"INFLAREV", Order = 42, TypeName = "int")]
        [Required]
        [Key]
        [Display(Name = "Inflarev")]
        public int Inflarev { get; set; }
    }

}
// </auto-generated>
