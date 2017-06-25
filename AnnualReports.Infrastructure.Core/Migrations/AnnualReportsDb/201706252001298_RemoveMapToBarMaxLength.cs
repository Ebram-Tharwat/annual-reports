namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using AnnualReports.Infrastructure.Core.Extensions;
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveMapToBarMaxLength : DbMigration
    {
        public override void Up()
        {
            this.DeleteDefaultContraint("dbo.Bars", "MapToBarNumber");
            AlterColumn("dbo.Bars", "MapToBarNumber", c => c.String());
        }

        public override void Down()
        {
            AlterColumn("dbo.Bars", "MapToBarNumber", c => c.String(nullable: false, maxLength: 9, defaultValue: ""));
        }
    }
}
