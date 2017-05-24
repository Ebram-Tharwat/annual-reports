using AnnualReports.Application.Core.Contracts.Reports;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IReportService
    {
        List<FundsReportDataItemDetails> GetFundsReportData(int year, string fundNumber = null, string barNumber = null);
    }
}