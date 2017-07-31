using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class AnnualReportsDbFundRepository : AnnualReportsDbEfRepository<Fund>, IAnnualReportsDbFundRepository
    {
        public AnnualReportsDbFundRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<AnnualReportDataRow> GetAnnualReportDataRows(int year, int? fundId = null)
        {
            return _dbContext.Database.SqlQuery<AnnualReportDataRow>("GetFundsReportDataPro {0},{1}", year, fundId).ToList();
        }

        public List<Fund> SearchForFunds(int? year, DbSource? dbSource, string displayName, string fundNumber, bool? isActive, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var query = this.GetAll();

            if (year.HasValue)
                query = query.Where(t => t.Year == year);

            if (dbSource.HasValue && dbSource.Value != DbSource.ALL)
                query = query.Where(t => t.DbSource == dbSource);

            if (!string.IsNullOrWhiteSpace(displayName))
                query = query.Where(t => t.DisplayName.Contains(displayName.Trim()));

            if (!string.IsNullOrWhiteSpace(fundNumber))
                query = query.Where(t => t.FundNumber.StartsWith(fundNumber.Trim()));

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive);

            total = query.Count();
            query = query.OrderBy(t => t.FundNumber);
            query = skipCount == 0 ? query.Take(size) : query.Skip(skipCount).Take(size);

            return query.ToList();
        }
    }
}