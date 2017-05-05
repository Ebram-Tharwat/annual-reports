namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System.Data.Entity.Migrations;

    public partial class RenameFundTypeColumn : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Funds", "Type", "DbSource");
            //AddColumn("dbo.Funds", "DbSource", c => c.Int(nullable: false));
            //DropColumn("dbo.Funds", "Type");
        }

        public override void Down()
        {
            RenameColumn("dbo.Funds", "DbSource", "Type");
            //AddColumn("dbo.Funds", "Type", c => c.Int(nullable: false));
            //DropColumn("dbo.Funds", "DbSource");
        }
    }
}