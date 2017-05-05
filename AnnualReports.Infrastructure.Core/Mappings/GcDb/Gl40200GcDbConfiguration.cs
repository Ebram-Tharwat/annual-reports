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


namespace AnnualReports.Infrastructure.Core.Mappings.GcDb
{
    using AnnualReports.Domain.Core.GcDbModels;

    public class Gl40200GcDbConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Gl40200>
    {
        public Gl40200GcDbConfiguration()
            : this("dbo")
        {
        }

        public Gl40200GcDbConfiguration(string schema)
        {
            Property(x => x.Sgmntid).IsFixedLength().IsUnicode(false);
            Property(x => x.FundDescription).IsFixedLength().IsUnicode(false);
            Property(x => x.Noteindx).HasPrecision(19,5);
        }
    }

}
// </auto-generated>
