using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;
using System.Linq.Expressions;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IAnnualReportsDbFundRepository : IRepository<Fund>
    {
        //void Delete(Expression<Func<Fund, bool>> filter);
    }
}