using AnnualReports.Application.Core.Contracts.Reports;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IReportService
    {
        List<AnnualReportDataItemDetails> GetAnnualReportData(int year, int? fundId = null, string barNumber = null);

        List<DistOrGcReportDataItemDetails> GetDistExceptionReportData(int year);
        List<DistOrGcReportDataItemDetails> GetGcExceptionReportData(int year);
    }
}