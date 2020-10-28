using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelProcessors.AuditorMaster;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.UseCases
{
    public interface IGenerateJournalVoucherReportUseCase
    {
        MemoryStream Execute(Stream inputStream, int year, List<MonthlyImportFundExceptionRule> exceptionRules);

        List<MonthlyReportRule> GetMonthlyReportRules();

        List<MonthlyImportFundExceptionRule> GetMonthlyImportExceptionRules();

        MonthlyReportRule GetMonthlyReport(int id);

        MonthlyImportFundExceptionRule GetMonthlyImportExceptionRuleReport(int id);

        MonthlyReportRule UpdateMonthlyReport(MonthlyReportRule monthlyReportRule);

        MonthlyImportFundExceptionRule UpdateMonthlyImportExceptionRuleReport(MonthlyImportFundExceptionRule monthlyImportFundExceptionRule);

        void AddMonthlyImportFundExceptionRuleReport(MonthlyImportFundExceptionRule entity);
    }

    public class GenerateJournalVoucherReportUseCase : IGenerateJournalVoucherReportUseCase
    {
        private readonly IExportingService _exportingService;
        private readonly IReportService _reportService;
        private readonly AuditorMasterProcessor[] _sheetProcessors;

        public GenerateJournalVoucherReportUseCase(
            IExportingService exportingService, IReportService reportService,
            AuditorMasterProcessor[] sheetProcessors)
        {
            _exportingService = exportingService;
            _sheetProcessors = sheetProcessors;
            _reportService = reportService;
        }

        public MemoryStream Execute(Stream inputStream, int year, List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();
            var matchingResultBuilder = new JournalVoucherMatchingResultBuilder();

            foreach (var processor in _sheetProcessors)
            {
                results.AddRange(processor.Process(inputStream, year, matchingResultBuilder, exceptionRules));
            }

            return _exportingService.GetJournalVoucherReportExcel(results, matchingResultBuilder.UnmatchedFunds);
        }

        public List<MonthlyReportRule> GetMonthlyReportRules()
        {
            return _reportService.GetMonthlyReportRules()
                .OrderBy(t => t.JournalVoucherType)
                .ToList();
        }

        public List<MonthlyImportFundExceptionRule> GetMonthlyImportExceptionRules()
        {
            return _reportService.GetMonthlyImportExceptionRules();
        }

        public MonthlyReportRule GetMonthlyReport(int id)
        {
            return _reportService.GetMonthlyReportRule(id);
        }

        public MonthlyImportFundExceptionRule GetMonthlyImportExceptionRuleReport(int id)
        {
            return _reportService.GetMonthlyImportExceptionRuleReport(id);
        }

        public MonthlyReportRule UpdateMonthlyReport(MonthlyReportRule monthlyReportRule)
        {
            return _reportService.UpdateMonthlyReportRule(monthlyReportRule);
        }

        public MonthlyImportFundExceptionRule UpdateMonthlyImportExceptionRuleReport(MonthlyImportFundExceptionRule monthlyImportFundExceptionRule)
        {
            return _reportService.UpdateMonthlyImportExceptionRuleReport(monthlyImportFundExceptionRule);
        }

        public void AddMonthlyImportFundExceptionRuleReport(MonthlyImportFundExceptionRule entity)
        {
            _reportService.AddMonthlyImportFundExceptionRuleReport(entity);
        }
    }
}