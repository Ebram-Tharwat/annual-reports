namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPeriodColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bars", "Period", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bars", "Period");
        }
    }
}
