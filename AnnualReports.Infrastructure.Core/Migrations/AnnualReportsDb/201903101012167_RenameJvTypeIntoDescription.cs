namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RenameJvTypeIntoDescription : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DynamicRules", "JvType", "Description");
        }

        public override void Down()
        {
            RenameColumn("dbo.DynamicRules", "Description", "JvType");
        }
    }
}
