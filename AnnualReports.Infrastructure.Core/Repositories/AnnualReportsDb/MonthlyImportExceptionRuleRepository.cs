using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class MonthlyImportExceptionRuleRepository : AnnualReportsDbEfRepository<MonthlyImportFundExceptionRule>,
                                                        IMonthlyImportExceptionRuleRepository
    {
        public MonthlyImportExceptionRuleRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }
    }
}