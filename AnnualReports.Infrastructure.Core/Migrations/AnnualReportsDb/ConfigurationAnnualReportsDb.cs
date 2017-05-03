namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class ConfigurationAnnualReportsDb : DbMigrationsConfiguration<AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb.AnnualReportsDbContext>
    {
        public ConfigurationAnnualReportsDb()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\AnnualReportsDb";
        }

        protected override void Seed(AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb.AnnualReportsDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
