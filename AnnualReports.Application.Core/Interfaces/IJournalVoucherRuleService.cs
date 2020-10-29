using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IJournalVoucherRuleService
    {
        MonthlyReportRule Get(int id);

        List<MonthlyReportRule> GetAll();

        MonthlyReportRule GetMonthlyReportRule(JournalVoucherType jvType, string fundId);

        MonthlyReportRule Update(MonthlyReportRule monthlyReportRule);

        void Add(MonthlyReportRule entity);

        void Delete(int id);
    }
}