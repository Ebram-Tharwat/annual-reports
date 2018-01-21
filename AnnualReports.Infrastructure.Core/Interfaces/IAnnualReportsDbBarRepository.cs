using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IAnnualReportsDbBarRepository : IRepository<Bar>
    {
        List<Bar> SearchForBars(int? year, string displayName, string barNumber, bool? isActive, DbSource? dbSource, out int total, int index = 0, int size = 50);
    }
}