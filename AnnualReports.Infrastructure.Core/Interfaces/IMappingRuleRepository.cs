using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IMappingRuleRepository : IRepository<MappingRule>
    {
        List<MappingRule> SearchForRules(int? year, string fundNumber, out int total, int pageIndex, int pageSize);
    }
}