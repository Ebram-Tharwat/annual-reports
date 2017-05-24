using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Extensions;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
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

        public IEnumerable<FundsReportDataRow> GetFundsReportDataRows(int year, string fundNumber = null)
        {
            return _dbContext.Database.SqlQuery<FundsReportDataRow>("GetFundsReportDataPro {0},{1}", year, fundNumber);
        }
    }
}