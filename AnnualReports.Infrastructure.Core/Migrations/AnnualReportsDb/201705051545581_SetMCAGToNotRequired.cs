namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetMCAGToNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Funds", "MCAG", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Funds", "MCAG", c => c.String(nullable: false, maxLength: 10));
        }
    }
}
