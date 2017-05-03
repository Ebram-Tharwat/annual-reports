using AnnualReports.Infrastructure.Core.DbContexts.GcDb;
using AnnualReports.Infrastructure.Core.Interfaces;

namespace AnnualReports.Infrastructure.Core.Repositories.GcDb
{
    public class GcDbEfRepository<T> : EFRepository<T>, IRepository<T> where T : class
    {
        public GcDbEfRepository(GcDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}