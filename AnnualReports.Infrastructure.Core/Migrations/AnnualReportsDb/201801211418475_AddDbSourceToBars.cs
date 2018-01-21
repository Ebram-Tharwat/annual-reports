namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using AnnualReports.Domain.Core.AnnualReportsDbModels;
    using System.Data.Entity.Migrations;

    public partial class AddDbSourceToBars : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bars", "DbSource", c => c.Int(defaultValue: (int)DbSource.GC));
        }

        public override void Down()
        {
            DropColumn("dbo.Bars", "DbSource");
        }
    }
}