using AnnualReports.Domain.Core.AnnualReportsDbModels;

namespace AnnualReports.Infrastructure.Core.Mappings.AnnualReportsDb
{
    public class FundConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Fund>
    {
        public FundConfiguration()
            : this("dbo")
        {
        }

        public FundConfiguration(string schema)
        {
            //Property(x => x.FundNumber).IsFixedLength().IsUnicode(false);
            //Property(x => x.GpDescription).IsFixedLength().IsUnicode(false);
        }
    }
}