using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IMonthlyImportExceptionRuleService
    {
        List<MonthlyImportFundExceptionRule> GetAll();

        MonthlyImportFundExceptionRule Get(int id);

        MonthlyImportFundExceptionRule Update(MonthlyImportFundExceptionRule monthlyImportFundExceptionRule);

        void Add(MonthlyImportFundExceptionRule entity);
    }
}