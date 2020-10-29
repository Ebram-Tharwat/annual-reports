using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class MonthlyImportExceptionRuleService : IMonthlyImportExceptionRuleService
    {
        private readonly IMonthlyImportExceptionRuleRepository _monthlyImportExceptionRuleRepository;
        private readonly IUnitOfWork<AnnualReportsDbContext> _uow;

        public MonthlyImportExceptionRuleService(
            IMonthlyImportExceptionRuleRepository monthlyImportExceptionRuleRepository,
            IUnitOfWork<AnnualReportsDbContext> uow)
        {
            _monthlyImportExceptionRuleRepository = monthlyImportExceptionRuleRepository;
            _uow = uow;
        }

        public List<MonthlyImportFundExceptionRule> GetAll()
        {
            return _monthlyImportExceptionRuleRepository.GetAll().ToList();
        }

        public MonthlyImportFundExceptionRule Get(int id)
        {
            return _monthlyImportExceptionRuleRepository.GetById(id);
        }

        public MonthlyImportFundExceptionRule Update(MonthlyImportFundExceptionRule monthlyImportFundExceptionRule)
        {
            _monthlyImportExceptionRuleRepository.Update(monthlyImportFundExceptionRule);
            _uow.Commit();
            return monthlyImportFundExceptionRule;
        }

        public void Add(MonthlyImportFundExceptionRule entity)
        {
            _monthlyImportExceptionRuleRepository.Add(entity);
            _uow.Commit();
        }
    }
}