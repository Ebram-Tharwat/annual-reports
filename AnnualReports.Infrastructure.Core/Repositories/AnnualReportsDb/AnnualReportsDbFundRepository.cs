using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class AnnualReportsDbFundRepository : AnnualReportsDbEfRepository<Fund>, IAnnualReportsDbFundRepository
    {
        public AnnualReportsDbFundRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<AnnualReportDataRow> GetAnnualReportDataRows(int year, int? fundId = null)
        {
            return _dbContext.Database.SqlQuery<AnnualReportDataRow>("GetFundsReportDataPro {0},{1}", year, fundId);
        }
    }
}