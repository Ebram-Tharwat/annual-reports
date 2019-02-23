using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.ExcelProcessors.AuditorMaster;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;
using System.IO;

namespace AnnualReports.Application.Core.UseCases
{
    public interface IGenerateJournalVoucherReportUseCase
    {
        MemoryStream Execute(Stream inputStream, int year);

        List<MonthlyReportRule> GetMonthlyReportRules();
        MonthlyReportRule GetMonthlyReport(int id);
        MonthlyReportRule GetMonthlyReport(JournalVoucherType type);

        MonthlyReportRule UpdateMonthlyReport(MonthlyReportRule monthlyReportRule);
    }

    public class GenerateJournalVoucherReportUseCase : IGenerateJournalVoucherReportUseCase
    {
        private readonly IExportingService _exportingService;
        private readonly IReportService _reportService;
        private readonly AuditorMasterProcessor[] _sheetProcessors;

        public GenerateJournalVoucherReportUseCase(
            IExportingService exportingService,IReportService reportService,
            AuditorMasterProcessor[] sheetProcessors)
        {
            _exportingService = exportingService;
            _sheetProcessors = sheetProcessors;
            _reportService = reportService;
        }

        public MemoryStream Execute(Stream inputStream, int year)
        {
            List<JournalVoucherReportOutputItem> results = new List<JournalVoucherReportOutputItem>();

            foreach (var processor in _sheetProcessors)
            {
                results.AddRange(processor.Process(inputStream, year));
            }

            return _exportingService.GetJournalVoucherReportExcel(results);
        }

        public List<MonthlyReportRule> GetMonthlyReportRules()
        {
            return _reportService.GetMonthlyReportRules();   
        }

        public MonthlyReportRule GetMonthlyReport(int id)
        {
            return _reportService.GetMonthlyReportRule(id);
        }

        public MonthlyReportRule GetMonthlyReport(JournalVoucherType type)
        {
            return _reportService.GetMonthlyReportRule(type);
        }

        public MonthlyReportRule UpdateMonthlyReport(MonthlyReportRule monthlyReportRule)
        {
            return _reportService.UpdateMonthlyReportRule(monthlyReportRule);
        }
    }
}