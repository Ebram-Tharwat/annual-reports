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

    public class Gl00100GcDbConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Gl00100>
    {
        public Gl00100GcDbConfiguration()
            : this("dbo")
        {
        }

        public Gl00100GcDbConfiguration(string schema)
        {
            Property(x => x.FundNumber).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr2).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr3).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr4).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr5).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr6).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr7).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr8).IsFixedLength().IsUnicode(false);
            Property(x => x.Actalias).IsFixedLength().IsUnicode(false);
            Property(x => x.Mnacsgmt).IsFixedLength().IsUnicode(false);
            Property(x => x.Actdescr).IsFixedLength().IsUnicode(false);
            Property(x => x.Hstrclrt).HasPrecision(19,7);
            Property(x => x.Noteindx).HasPrecision(19,5);
            Property(x => x.Userdef1).IsFixedLength().IsUnicode(false);
            Property(x => x.Userdef2).IsFixedLength().IsUnicode(false);
            Property(x => x.Usrdefs1).IsFixedLength().IsUnicode(false);
            Property(x => x.Usrdefs2).IsFixedLength().IsUnicode(false);
        }
    }

}
// </auto-generated>
