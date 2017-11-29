using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class MappingRuleRepository : AnnualReportsDbEfRepository<MappingRule>, IMappingRuleRepository
    {
        public MappingRuleRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }
    }
}