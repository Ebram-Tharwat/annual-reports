using AnnualReports.Infrastructure.Core.DbContexts.GcDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;

namespace AnnualReports.Infrastructure.Core.Repositories.GcDb
{
    public class GcDbEfRepository<T> : EFRepository<T>, IRepository<T> where T : class
    {
        public GcDbEfRepository(GcDbContext dbContext)
            : base(dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");

            this._dbContext = dbContext;
        }

        #region Properties

        protected new GcDbContext _dbContext;

        #endregion Properties
    }
}