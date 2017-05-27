namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMapToBarFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bars", "MapToBarId", "dbo.Bars");
            DropIndex("dbo.Bars", new[] { "MapToBarId" });
            AddColumn("dbo.Bars", "MapToBarNumber", c => c.String(nullable: false, maxLength: 9));
            DropColumn("dbo.Bars", "MapToBarId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bars", "MapToBarId", c => c.Int());
            DropColumn("dbo.Bars", "MapToBarNumber");
            CreateIndex("dbo.Bars", "MapToBarId");
            AddForeignKey("dbo.Bars", "MapToBarId", "dbo.Bars", "Id");
        }
    }
}
