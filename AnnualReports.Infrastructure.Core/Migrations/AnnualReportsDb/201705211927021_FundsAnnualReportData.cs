namespace AnnualReports.Infrastructure.Core.Migrations.AnnualReportsDb
{
    using System.Data.Entity.Migrations;

    public partial class FundsAnnualReportData : DbMigration
    {
        public override void Up()
        {
            Sql(ScriptResources.GetFundsReportDataPro_UP);
            Sql(ScriptResources.GetGCFundsReportDataPro_UP);
            Sql(ScriptResources.GetDISTFundsReportDataPro_UP);
        }

        public override void Down()
        {
            Sql(ScriptResources.GetFundsReportDataPro_Down);
            Sql(ScriptResources.GetGCFundsReportDataPro_Down);
            Sql(ScriptResources.GetDISTFundsReportDataPro_Down);
        }
    }
}