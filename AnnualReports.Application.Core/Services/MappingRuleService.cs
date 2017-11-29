using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;

namespace AnnualReports.Application.Core.Services
{
    public class MappingRuleService : IMappingRuleService
    {
        private readonly IMappingRuleRepository _mappingRuleRepository;
        private readonly IUnitOfWork<AnnualReportsDbContext> _uow;

        public MappingRuleService(IMappingRuleRepository mappingRuleRepository, IUnitOfWork<AnnualReportsDbContext> uow)
        {
            _mappingRuleRepository = mappingRuleRepository;
            _uow = uow;
        }

        public void Add(MappingRule entity)
        {
            _mappingRuleRepository.Add(entity);
            _uow.Commit();
        }
    }
}