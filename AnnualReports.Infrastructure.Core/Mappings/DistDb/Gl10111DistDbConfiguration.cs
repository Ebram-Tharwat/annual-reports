// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.5
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace AnnualReports.Infrastructure.Core.Mappings.DistDb
{
    using AnnualReports.Domain.Core.DistDbModels;

    public class Gl10111DistDbConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Gl10111>
    {
        public Gl10111DistDbConfiguration()
            : this("dbo")
        {
        }

        public Gl10111DistDbConfiguration(string schema)
        {
            Property(x => x.Actnumbr1).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr2).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr3).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr4).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr5).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr6).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr7).IsFixedLength().IsUnicode(false);
            Property(x => x.Actnumbr8).IsFixedLength().IsUnicode(false);
            Property(x => x.Perdblnc).HasPrecision(19,5);
            Property(x => x.Debitamt).HasPrecision(19,5);
            Property(x => x.Crdtamnt).HasPrecision(19,5);
        }
    }

}
// </auto-generated>
