using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;

namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class AnnualReportsDbEfRepository<T> : EFRepository<T>, IRepository<T> where T : class
    {
        public AnnualReportsDbEfRepository(AnnualReportsDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}