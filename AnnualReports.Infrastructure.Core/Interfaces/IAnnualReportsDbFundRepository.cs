using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using System.Collections.Generic;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IAnnualReportsDbFundRepository : IRepository<Fund>
    {
        IEnumerable<FundsReportDataRow> GetFundsReportDataRows(int year, string fundNumber = null);
    }
}