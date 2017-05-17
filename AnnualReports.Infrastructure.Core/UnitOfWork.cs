using System;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;

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
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = new StringBuilder();

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg.AppendFormat("Property: {0} Error: {1}", validationError.PropertyName,
                            validationError.ErrorMessage);
                        msg.AppendLine();
                    }
                }

                // ToDo: Errors should be logged.
                var fail = new Exception(msg.ToString(), dbEx);
                throw fail;
            }
        }

        #endregion IUnitOfWork Members
    }
}