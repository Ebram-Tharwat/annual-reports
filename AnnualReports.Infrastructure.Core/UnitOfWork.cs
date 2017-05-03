using AnnualReports.Infrastructure.Core.Interfaces;
using System.Data.Entity;

namespace AnnualReports.Infrastructure.Core
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        #region Fields

        private TContext _dbContext;

        #endregion Fields

        #region Ctor

        public UnitOfWork(TContext context)
        {
            _dbContext = context;
        }

        #endregion Ctor

        #region IUnitOfWork Members

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        #endregion IUnitOfWork Members
    }
}