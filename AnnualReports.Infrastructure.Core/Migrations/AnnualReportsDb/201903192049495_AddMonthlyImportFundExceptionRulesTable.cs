namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMonthlyImportFundExceptionRulesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FundExceptionRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OriginalFundId = c.String(),
                        NewFundId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FundExceptionRules");
        }
    }
}
