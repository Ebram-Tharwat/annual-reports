using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IMappingRuleRepository : IRepository<MappingRule>
    {
        List<MappingRule> SearchForRules(int? year, string fundNumber, out int total, int pageIndex, int pageSize);
    }
}
