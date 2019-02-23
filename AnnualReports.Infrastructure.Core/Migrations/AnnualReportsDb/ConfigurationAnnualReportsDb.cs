namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using Domain.Core.AnnualReportsDbModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
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

            var UserStore = new UserStore<ApplicationUser>(context);
            var UserManager = new UserManager<ApplicationUser>(UserStore);

            context.InvestmentTypes.AddOrUpdate(new InvestmentTypes { Id = 1, Name = "General Receipts" });
            context.InvestmentTypes.AddOrUpdate(new InvestmentTypes { Id = 2, Name = "Investment Interest" });
            context.InvestmentTypes.AddOrUpdate(new InvestmentTypes { Id = 3, Name = "Investment Purchases" });
            context.InvestmentTypes.AddOrUpdate(new InvestmentTypes { Id = 4, Name = "Investment Sales" });
            context.InvestmentTypes.AddOrUpdate(new InvestmentTypes { Id = 5, Name = "Taxes" });
            context.InvestmentTypes.AddOrUpdate(new InvestmentTypes { Id = 6, Name = "Warrants Canceled" });
            context.InvestmentTypes.AddOrUpdate(new InvestmentTypes { Id = 7, Name = "Warrants Issued" });
            context.InvestmentTypes.AddOrUpdate(new InvestmentTypes { Id = 8, Name = "Warrants Presented" });

            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule { Id = 1, JvType = "Warrant Issue", CreditAccount = "211000000", DebitAccount = "299000000" });
            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule { Id = 2, JvType = "Warrant Presented", CreditAccount = "101000000", DebitAccount = "211000000" });
            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule
            {
                Id = 3,
                JvType = "Warrant Cancel",
                CreditAccount = "211000000",
                DebitAccount = "229000000",
                CreditExceptionNegative = "299000000",
                DebitExceptionNegative = "211000000"
            });
            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule { Id = 4, JvType = "Taxes", CreditAccount = "311110000", DebitAccount = "101000000" });
            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule { Id = 5, JvType = "Investment Purchase", CreditAccount = "101000000", DebitAccount = "118000000" });
            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule { Id = 6, JvType = "Investment Sales", CreditAccount = "118000000", DebitAccount = "101000000" });
            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule { Id = 7, JvType = "Investment Interest", CreditAccount = "361110000", DebitAccount = "118000000" });
            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule { Id = 8, JvType = "Warrant Interest", CreditAccount = "101000000", DebitAccount = "299000000" });
            context.MonthlyReportRules.AddOrUpdate(new MonthlyReportRule { Id = 9, JvType = "Remits", CreditAccount = "101000000", DebitAccount = "229000000" });

            string[] systemRoles = new string[] { "Admin", "Clerk" };
            List<string> names = new List<string>();
            names.Add("Admin");
            names.Add("Clerk");

            for (int i = 0; i < systemRoles.Count(); i++)
            {
                string sysRoelName = systemRoles[i];
                IdentityRole sysRole = context.Roles.Where(m => m.Name == sysRoelName).FirstOrDefault();
                if (sysRole == null)
                {
                    sysRole = new IdentityRole(sysRoelName);
                    context.Roles.AddOrUpdate(sysRole);
                    context.SaveChanges();
                    sysRole = context.Roles.Where(m => m.Name == sysRoelName).FirstOrDefault();

                    string userName = names[i];
                    int deptId = i + 1;
                    if (deptId > 5)
                        deptId += 1;
                    ApplicationUser defaultUsers = new ApplicationUser { UserName = userName, Email = userName + "@gris.com", FullName = "Test User", PhoneNumber = "054863478", LockoutEnabled = true };
                    var result = UserManager.Create(defaultUsers, "123456");
                    if (result.Succeeded)
                    {
                        var roelResult = UserManager.AddToRoles(defaultUsers.Id, sysRole.Name);
                    }

                }
            }
        }
    }
}
