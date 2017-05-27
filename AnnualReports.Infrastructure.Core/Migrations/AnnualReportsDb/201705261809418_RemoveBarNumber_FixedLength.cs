namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBarNumber_FixedLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bars", "BarNumber", c => c.String(nullable: false, maxLength: 9));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Bars", "BarNumber", c => c.String(nullable: false, maxLength: 9, fixedLength: true, unicode: false));
        }
    }
}
