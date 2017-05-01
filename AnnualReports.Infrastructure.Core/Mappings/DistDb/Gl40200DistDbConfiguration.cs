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

    public class Gl40200DistDbConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Gl40200>
    {
        public Gl40200DistDbConfiguration()
            : this("dbo")
        {
        }

        public Gl40200DistDbConfiguration(string schema)
        {
            Property(x => x.Sgmntid).IsFixedLength().IsUnicode(false);
            Property(x => x.Dscriptn).IsFixedLength().IsUnicode(false);
            Property(x => x.Noteindx).HasPrecision(19,5);
        }
    }

}
// </auto-generated>
