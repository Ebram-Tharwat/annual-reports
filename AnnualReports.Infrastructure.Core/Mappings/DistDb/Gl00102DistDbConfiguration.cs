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


namespace AnnualReports.Infrastructure.Core.Mappings.DistDb
{
    using AnnualReports.Domain.Core.DistDbModels;

    public class Gl00102DistDbConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Gl00102>
    {
        public Gl00102DistDbConfiguration()
            : this("dbo")
        {
        }

        public Gl00102DistDbConfiguration(string schema)
        {
            Property(x => x.Accatdsc).IsFixedLength().IsUnicode(false);
        }
    }

}
// </auto-generated>
