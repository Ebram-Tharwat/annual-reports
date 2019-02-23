using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IMonthlyReportRepository : IRepository<MonthlyReportRule>
    {
    }
}
