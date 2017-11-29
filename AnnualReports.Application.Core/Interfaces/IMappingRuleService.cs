using AnnualReports.Domain.Core.AnnualReportsDbModels;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IMappingRuleService
    {
        void Add(MappingRule entity);
    }
}