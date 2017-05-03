using AnnualReports.Infrastructure.Core.DbContexts.DistDb;
using AnnualReports.Infrastructure.Core.Interfaces;

namespace AnnualReports.Infrastructure.Core.Repositories.DistDb
{
    public class DistDbEfRepository<T> : EFRepository<T>, IRepository<T> where T : class
    {
        public DistDbEfRepository(DistDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}