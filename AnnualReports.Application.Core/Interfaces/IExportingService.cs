using System;
using System.IO;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IExportingService
    {
        MemoryStream GetFundsTemplate(int year);

        MemoryStream GetBarsTemplate(int year);

        MemoryStream GetAnnualReportExcel(int year);
    }
}