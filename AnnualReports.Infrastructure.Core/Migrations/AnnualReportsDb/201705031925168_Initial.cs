namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bars",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    BarNumber = c.String(nullable: false, maxLength: 9, fixedLength: true, unicode: false),
                    DisplayName = c.String(nullable: false, maxLength: 200),
                    Year = c.Short(nullable: false),
                    MapToBarId = c.Int(),
                    IsActive = c.Boolean(nullable: false, defaultValue: true),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bars", t => t.MapToBarId)
                .Index(t => t.MapToBarId);

            CreateTable(
                "dbo.Funds",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    FundNumber = c.String(nullable: false, maxLength: 9, fixedLength: true, unicode: false),
                    GpDescription = c.String(nullable: false, maxLength: 31, fixedLength: true, unicode: false),
                    DisplayName = c.String(nullable: false, maxLength: 100),
                    Year = c.Short(nullable: false),
                    MCAG = c.String(nullable: false, maxLength: 10),
                    MapToFundId = c.Int(),
                    Type = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false, defaultValue: true),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Funds", t => t.MapToFundId)
                .Index(t => t.MapToFundId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Funds", "MapToFundId", "dbo.Funds");
            DropForeignKey("dbo.Bars", "MapToBarId", "dbo.Bars");
            DropIndex("dbo.Funds", new[] { "MapToFundId" });
            DropIndex("dbo.Bars", new[] { "MapToBarId" });
            DropTable("dbo.Funds");
            DropTable("dbo.Bars");
        }
    }
}
