namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFixedLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Funds", "FundNumber", c => c.String(nullable: false, maxLength: 9));
            AlterColumn("dbo.Funds", "GpDescription", c => c.String(nullable: false, maxLength: 31));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Funds", "GpDescription", c => c.String(nullable: false, maxLength: 31, fixedLength: true, unicode: false));
            AlterColumn("dbo.Funds", "FundNumber", c => c.String(nullable: false, maxLength: 9, fixedLength: true, unicode: false));
        }
    }
}
