using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class JournalVoucherRuleService : IJournalVoucherRuleService
    {
        private readonly IMonthlyReportRepository _monthlyReportRepository;
        private readonly IUnitOfWork<AnnualReportsDbContext> _uow;

        public JournalVoucherRuleService(IMonthlyReportRepository monthlyReportRepository, IUnitOfWork<AnnualReportsDbContext> uow)
        {
            _monthlyReportRepository = monthlyReportRepository;
            _uow = uow;
        }

        public List<MonthlyReportRule> GetAll()
        {
            return _monthlyReportRepository
                .GetAll()
                .OrderBy(t => t.JournalVoucherType)
                .ToList();
        }

        public MonthlyReportRule Get(int id)
        {
            return _monthlyReportRepository.GetById(id);
        }

        public MonthlyReportRule GetMonthlyReportRule(JournalVoucherType jvType, string fundId)
        {
            var rules = _monthlyReportRepository.Get(t => t.JournalVoucherType == jvType).ToList();
            var specificityRules = rules.Where(t => !string.IsNullOrWhiteSpace(t.FundIds));
            var defaultRule = rules.FirstOrDefault(t => string.IsNullOrWhiteSpace(t.FundIds));

            foreach (var rule in specificityRules)
            {
                var isRuleHasMatchedFundId =
                    rule.FundIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Any(fund => fund.Trim() == fundId.Trim());

                if (isRuleHasMatchedFundId)
                    return rule;
            }
            return defaultRule;
        }

        public void Add(MonthlyReportRule entity)
        {
            _monthlyReportRepository.Add(entity);
            _uow.Commit();
        }

        public MonthlyReportRule Update(MonthlyReportRule monthlyReportRule)
        {
            _monthlyReportRepository.Update(monthlyReportRule);
            _uow.Commit();
            return monthlyReportRule;
        }

        public void Delete(int id)
        {
            _monthlyReportRepository.Delete(id);
            _uow.Commit();
        }
    }
}