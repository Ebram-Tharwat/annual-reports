namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMappingRulesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MappingRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        Operator = c.Int(nullable: false),
                        TargetBarNumber = c.String(nullable: false, maxLength: 9),
                        TargetFundNumber = c.String(nullable: false, maxLength: 9),
                        DebitBarNumber = c.String(nullable: false, maxLength: 9),
                        CreditBarNumber = c.String(nullable: false, maxLength: 9),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MappingRules");
        }
    }
}
