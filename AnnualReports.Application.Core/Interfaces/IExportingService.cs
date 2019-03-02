using AnnualReports.Application.Core.Contracts.Reports;
using System.Collections.Generic;
using System.IO;
using static AnnualReports.Application.Core.Contracts.Reports.JournalVoucherMatchingResultBuilder;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IExportingService
    {
        MemoryStream GetFundsTemplate(int year);
        MemoryStream GetBarsTemplate(int year);
        MemoryStream GetAnnualReportExcel(int year, int? fundId);
        MemoryStream GetDistExceptionReportExcel(int year);
        MemoryStream GetGcExceptionReportExcel(int year);
        MemoryStream GetJournalVoucherReportExcel(IEnumerable<JournalVoucherReportOutputItem> reportData, IEnumerable<JournalVoucherUnamtchedResult> unamtchedResults);
    }
}