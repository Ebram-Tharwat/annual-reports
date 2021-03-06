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


namespace AnnualReports.Infrastructure.Core.Mappings.GcDb
{
    using AnnualReports.Domain.Core.GcDbModels;

    public class SlbAccountSummaryGcDbConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SlbAccountSummary>
    {
        public SlbAccountSummaryGcDbConfiguration()
            : this("dbo")
        {
        }

        public SlbAccountSummaryGcDbConfiguration(string schema)
        {
            Property(x => x.Perdblnc).HasPrecision(19,5);
            Property(x => x.Actdescr).IsOptional().IsUnicode(false);
            Property(x => x.Actalias).IsOptional().IsUnicode(false);
            Property(x => x.Accatnum).IsUnicode(false);
            Property(x => x.Crdtamnt).HasPrecision(19,5);
            Property(x => x.Debitamt).HasPrecision(19,5);
            Property(x => x.Mnacsgmt).IsOptional().IsUnicode(false);
            Property(x => x.Actnumbr1).IsOptional().IsUnicode(false);
            Property(x => x.Actnumbr2).IsOptional().IsUnicode(false);
            Property(x => x.Actnumbr3).IsOptional().IsUnicode(false);
            Property(x => x.Actnumbr4).IsOptional().IsUnicode(false);
            Property(x => x.Actnumbr5).IsOptional().IsUnicode(false);
            Property(x => x.Actnumbr6).IsOptional().IsUnicode(false);
            Property(x => x.Actnumbr7).IsOptional().IsUnicode(false);
            Property(x => x.Actnumbr8).IsOptional().IsUnicode(false);
            Property(x => x.Hstrclrt).HasPrecision(19,7);
            Property(x => x.Noteindx).HasPrecision(19,5);
            Property(x => x.Userdef1).IsOptional().IsUnicode(false);
            Property(x => x.Userdef2).IsOptional().IsUnicode(false);
        }
    }

}
// </auto-generated>
