using AnnualReports.Infrastructure.Core.DbContexts.DistDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;

namespace AnnualReports.Infrastructure.Core.Repositories.DistDb
{
    public class DistDbEfRepository<T> : EFRepository<T>, IRepository<T>
        where T : class
    {
        public DistDbEfRepository(DistDbContext dbContext)
            : base(dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");

            this._dbContext = dbContext;
        }

        #region Properties

        protected new DistDbContext _dbContext;

        #endregion Properties
    }
}