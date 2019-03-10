namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFundIdsColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DynamicRules", "JournalVoucherType", c => c.Int(nullable: false));
            AddColumn("dbo.DynamicRules", "FundIds", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DynamicRules", "FundIds");
            DropColumn("dbo.DynamicRules", "JournalVoucherType");
        }
    }
}
