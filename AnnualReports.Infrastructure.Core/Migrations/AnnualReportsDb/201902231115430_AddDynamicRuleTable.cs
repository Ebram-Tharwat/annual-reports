namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDynamicRuleTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DynamicRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JvType = c.String(nullable: false, maxLength: 30),
                        DebitAccount = c.String(nullable: false, maxLength: 10),
                        CreditAccount = c.String(nullable: false, maxLength: 10),
                        DebitExceptionNegative = c.String(maxLength: 10),
                        CreditExceptionNegative = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DynamicRules");
        }
    }
}
