using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IMappingRuleService
    {
        List<MappingRule> GetAll(int? year = default(int?), string fundNumber = null, PagingInfo pagingInfo = null);

        MappingRule GetById(int id);

        void Add(MappingRule entity);

        void Update(MappingRule entity);

        void Remove(MappingRule entity);
    }
}