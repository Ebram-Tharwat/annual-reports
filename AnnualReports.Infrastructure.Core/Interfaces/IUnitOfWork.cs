using System.Data.Entity;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        void Commit();
    }
}