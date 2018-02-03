namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBarNumberTarget : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bars", "BarTarget", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bars", "BarTarget");
        }
    }
}
