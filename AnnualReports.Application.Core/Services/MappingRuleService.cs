using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;

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

        public MappingRule GetById(int id)
        {
            return _mappingRuleRepository.GetById(id);
        }

        public void Add(MappingRule entity)
        {
            FixEntityProperties(entity);
            _mappingRuleRepository.Add(entity);
            _uow.Commit();
        }

        public void Update(MappingRule entity)
        {
            FixEntityProperties(entity);
            _mappingRuleRepository.Update(entity);
            _uow.Commit();
        }

        public void Remove(MappingRule entity)
        {
            _mappingRuleRepository.Delete(entity);
            _uow.Commit();
        }

        public List<MappingRule> GetAll(int? year = default(int?), string fundNumber = null, PagingInfo pagingInfo = null)
        {
            int total = 0;
            if (pagingInfo == null)
                return _mappingRuleRepository.SearchForRules(year, fundNumber, out total, -1, -1);
            else
            {
                var result = _mappingRuleRepository.SearchForRules(year, fundNumber, out total, pagingInfo.PageIndex, AppSettings.PageSize);
                pagingInfo.Total = total;
                return result;
            }
        }

        #region Helpers

        private void FixEntityProperties(MappingRule entity)
        {
            if (string.IsNullOrWhiteSpace(entity.CreditBarNumber) && !string.IsNullOrWhiteSpace(entity.DebitBarNumber))
                entity.CreditBarNumber = entity.DebitBarNumber;
            else if (string.IsNullOrWhiteSpace(entity.DebitBarNumber) && !string.IsNullOrWhiteSpace(entity.CreditBarNumber))
                entity.DebitBarNumber = entity.CreditBarNumber;

            entity.CreditBarNumber = entity.CreditBarNumber.Trim();
            entity.DebitBarNumber = entity.DebitBarNumber.Trim();
            entity.TargetBarNumber = entity.TargetBarNumber.Trim();
            entity.TargetFundNumber = entity.TargetFundNumber.Trim();
        }

        #endregion Helpers
    }
}