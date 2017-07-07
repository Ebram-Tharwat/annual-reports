using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using System.Collections.Generic;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IAnnualReportsDbFundRepository : IRepository<Fund>
    {
        IEnumerable<AnnualReportDataRow> GetAnnualReportDataRows(int year, int? fundId = null);

        List<Fund> SearchForFunds(int? year, DbSource? dbSource, string displayName, string fundNumber, bool? isActive, out int total, int index = 0, int size = 50);
    }
}