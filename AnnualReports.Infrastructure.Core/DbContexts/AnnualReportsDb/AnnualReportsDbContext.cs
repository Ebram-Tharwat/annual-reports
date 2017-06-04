using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.Mappings.AnnualReportsDb;
using System.Data.Entity;

namespace AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb
{
    public class AnnualReportsDbContext : DbContext
    {
        public DbSet<Fund> Funds { get; set; }

        public DbSet<Bar> Bars { get; set; }

        static AnnualReportsDbContext()
        {
            System.Data.Entity.Database.SetInitializer<AnnualReportsDbContext>(null);
        }

        public AnnualReportsDbContext()
            : base("Name=AnnualReportsDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Database.CommandTimeout = 180;
        }

        public AnnualReportsDbContext(string connectionString)
            : base(connectionString)
        {
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new BarConfiguration());
            modelBuilder.Configurations.Add(new FundConfiguration());
        }

        public static System.Data.Entity.DbModelBuilder CreateModel(System.Data.Entity.DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new BarConfiguration(schema));
            modelBuilder.Configurations.Add(new FundConfiguration(schema));
            return modelBuilder;
        }
    }
}