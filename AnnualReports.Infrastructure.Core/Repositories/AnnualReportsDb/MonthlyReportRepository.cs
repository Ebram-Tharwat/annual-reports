using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class MonthlyReportRepository : AnnualReportsDbEfRepository<MonthlyReportRule>, IMonthlyReportRepository
    {
        public MonthlyReportRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }
    }
}