﻿

// ------------------------------------------------------------------------------------------------
// This code was generated by EntityFramework Reverse POCO Generator (http://www.reversepoco.com/).
// Created by Simon Hughes (https://about.me/simon.hughes).
//
// Do not make changes directly to this file - edit the template instead.
//
// The following connection settings were used to generate this file:
//     Configuration file:     "AnnualReports.Web\Web.config"
//     Connection String Name: "DistDbContext"
//     Connection String:      "Data Source=.;Initial Catalog=DISTTest;Integrated Security=True;"
// ------------------------------------------------------------------------------------------------
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


namespace AnnualReports.Infrastructure.Core.DbContexts.DistDb
{
    using AnnualReports.Domain.Core.DistDbModels;
    using AnnualReports.Infrastructure.Core.Mappings.DistDb;

    #region Database context

    public class DistDbContext : System.Data.Entity.DbContext
    {
        public System.Data.Entity.DbSet<Gl00100> Gl00100 { get; set; }
        public System.Data.Entity.DbSet<Gl00102> Gl00102 { get; set; }
        public System.Data.Entity.DbSet<Gl10110> Gl10110 { get; set; }
        public System.Data.Entity.DbSet<Gl10111> Gl10111 { get; set; }
        public System.Data.Entity.DbSet<Gl40200> Gl40200 { get; set; }
        public System.Data.Entity.DbSet<SlbAccountSummary> SlbAccountSummaries { get; set; }

        static DistDbContext()
        {
            System.Data.Entity.Database.SetInitializer<DistDbContext>(null);
        }

        public DistDbContext()
            : base("Name=DistDbContext")
        {
        }

        public DistDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public DistDbContext(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(connectionString, model)
        {
        }

        public DistDbContext(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public DistDbContext(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public bool IsSqlParameterNull(System.Data.SqlClient.SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as System.Data.SqlTypes.INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == System.DBNull.Value);
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new Gl00100DistDbConfiguration());
            modelBuilder.Configurations.Add(new Gl00102DistDbConfiguration());
            modelBuilder.Configurations.Add(new Gl10110DistDbConfiguration());
            modelBuilder.Configurations.Add(new Gl10111DistDbConfiguration());
            modelBuilder.Configurations.Add(new Gl40200DistDbConfiguration());
            modelBuilder.Configurations.Add(new SlbAccountSummaryDistDbConfiguration());
        }

        public static System.Data.Entity.DbModelBuilder CreateModel(System.Data.Entity.DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new Gl00100DistDbConfiguration(schema));
            modelBuilder.Configurations.Add(new Gl00102DistDbConfiguration(schema));
            modelBuilder.Configurations.Add(new Gl10110DistDbConfiguration(schema));
            modelBuilder.Configurations.Add(new Gl10111DistDbConfiguration(schema));
            modelBuilder.Configurations.Add(new Gl40200DistDbConfiguration(schema));
            modelBuilder.Configurations.Add(new SlbAccountSummaryDistDbConfiguration(schema));
            return modelBuilder;
        }
    }
    #endregion

}
// </auto-generated>

