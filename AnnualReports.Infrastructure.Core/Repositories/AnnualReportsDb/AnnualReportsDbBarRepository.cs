using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class AnnualReportsDbBarRepository : AnnualReportsDbEfRepository<Bar>, IAnnualReportsDbBarRepository
    {
        public AnnualReportsDbBarRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }

        public List<Bar> SearchForBars(int? year, string displayName, string barNumber, bool? isActive, DbSource? dbSource, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var query = this.GetAll();

            if (year.HasValue)
                query = query.Where(t => t.Year == year);

            if (!string.IsNullOrWhiteSpace(displayName))
                query = query.Where(t => t.DisplayName.Contains(displayName.Trim()));

            if (!string.IsNullOrWhiteSpace(barNumber))
                query = query.Where(t => t.BarNumber.StartsWith(barNumber.Trim()));

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive);

            if (dbSource.HasValue && dbSource.Value != DbSource.ALL)
                query = query.Where(t => t.DbSource == dbSource);

            total = query.Count();
            query = query.OrderBy(t => t.BarNumber);
            query = skipCount == 0 ? query.Take(size) : query.Skip(skipCount).Take(size);

            return query.ToList();
        }
    }
}