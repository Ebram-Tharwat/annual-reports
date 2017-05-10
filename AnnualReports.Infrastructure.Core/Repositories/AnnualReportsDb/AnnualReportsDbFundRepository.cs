using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Extensions;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Linq.Expressions;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class AnnualReportsDbFundRepository : AnnualReportsDbEfRepository<Fund>, IAnnualReportsDbFundRepository
    {
        public AnnualReportsDbFundRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }

        public void Delete(Expression<Func<Fund, bool>> filter)
        {
            _dbContext.DeleteWhere(filter);
        }
    }
}