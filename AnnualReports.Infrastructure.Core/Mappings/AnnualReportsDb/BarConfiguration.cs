using AnnualReports.Domain.Core.AnnualReportsDbModels;

namespace AnnualReports.Infrastructure.Core.Mappings.AnnualReportsDb
{
    public class BarConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Bar>
    {
        public BarConfiguration()
            : this("dbo")
        {
        }

        public BarConfiguration(string schema)
        {
            Property(x => x.BarNumber).IsFixedLength().IsUnicode(false);
        }
    }
}