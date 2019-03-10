using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IReportService
    {
        List<AnnualReportDataItemDetails> GetAnnualReportData(int year, int? fundId = null, string barNumber = null);

        List<ExceptionReportDataItemDetails> GetDistExceptionReportData(int year);

        List<ExceptionReportDataItemDetails> GetGcExceptionReportData(int year);

        List<MonthlyReportRule> GetMonthlyReportRules();

        MonthlyReportRule GetMonthlyReportRule(int id);

        MonthlyReportRule GetMonthlyReportRule(JournalVoucherType jvType, string fundId);

        MonthlyReportRule UpdateMonthlyReportRule(MonthlyReportRule monthlyReportRule);

        List<Bar> GetDistTargetBarMappings(List<Bar> dbBars, string bar);
    }
}