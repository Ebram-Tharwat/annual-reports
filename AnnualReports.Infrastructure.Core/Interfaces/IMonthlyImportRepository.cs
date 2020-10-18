using AnnualReports.Domain.Core.AnnualReportsDbModels;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IMonthlyReportRepository : IRepository<MonthlyReportRule>
    {
    }
}