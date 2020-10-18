using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.Mappings.AnnualReportsDb;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb
{
    public class AnnualReportsDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Fund> Funds { get; set; }

        public DbSet<Bar> Bars { get; set; }

        public DbSet<MonthlyReportRule> MonthlyReportRules { get; set; }

        public DbSet<MonthlyImportFundExceptionRule> MonthlyImportFundExceptionRules { get; set; }

        public DbSet<InvestmentTypes> InvestmentTypes { get; set; }

        public DbSet<MappingRule> MappingRules { get; set; }

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
            //base.OnModelCreating(modelBuilder);

            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            // Keep this:
            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");
            modelBuilder.Entity<IdentityUser>().HasMany<IdentityUserRole>((IdentityUser u) => u.Roles);
            modelBuilder.Entity<IdentityUserRole>().HasKey((IdentityUserRole r) =>
                new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");

            // Leave this alone:
            EntityTypeConfiguration<IdentityUserLogin> entityTypeConfiguration =
                modelBuilder.Entity<IdentityUserLogin>().HasKey((IdentityUserLogin l) =>
                    new
                    {
                        UserId = l.UserId,
                        LoginProvider = l.LoginProvider,
                        ProviderKey
                            = l.ProviderKey
                    }).ToTable("AspNetUserLogins");

            EntityTypeConfiguration<IdentityUserClaim> table1 =
                modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");

            // Add this, so that IdentityRole can share a table with ApplicationRole:
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");

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