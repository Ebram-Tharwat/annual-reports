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
        MemoryStream Execute(Stream inputStream, int year, List<MonthlyImportFundExceptionRule> exceptionRules);
    }

    public class GenerateJournalVoucherReportUseCase : IGenerateJournalVoucherReportUseCase
    {
        private readonly IExportingService _exportingService;
        private readonly AuditorMasterProcessor[] _sheetProcessors;

        public GenerateJournalVoucherReportUseCase(
            IExportingService exportingService,
            AuditorMasterProcessor[] sheetProcessors)
        {
            _exportingService = exportingService;
            _sheetProcessors = sheetProcessors;
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
    }
}