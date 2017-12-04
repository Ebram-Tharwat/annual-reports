using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class MappingRuleRepository : AnnualReportsDbEfRepository<MappingRule>, IMappingRuleRepository
    {
        public MappingRuleRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }

        public List<MappingRule> SearchForRules(int? year, string fundNumber, out int total, int pageIndex, int pageSize)
        {
            var query = _dbContext.MappingRules.AsQueryable();
            if (year.HasValue)
                query = query.Where(t => t.Year == year.Value);
            if (!string.IsNullOrWhiteSpace(fundNumber))
                query = query.Where(t => t.TargetFundNumber == fundNumber);

            if (pageSize > 0)
            {
                var result = query.OrderBy(t => t.TargetFundNumber).Skip(pageIndex * pageSize).Take(pageSize).ToList();
                total = query.Count();
                return result;
            }
            else
            {
                var result = query.OrderBy(t => t.TargetFundNumber).ToList();
                total = result.Count;
                return result;
            }
        }
    }
}