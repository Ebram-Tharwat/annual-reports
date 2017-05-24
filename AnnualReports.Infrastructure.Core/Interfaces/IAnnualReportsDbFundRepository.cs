using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IAnnualReportsDbFundRepository : IRepository<Fund>
    {
        void Delete(Expression<Func<Fund, bool>> filter);

        IEnumerable<FundsReportDataRow> GetFundsReportDataRows(int year, string fundNumber = null);
    }
}