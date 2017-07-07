using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Z.EntityFramework.Plus;

namespace AnnualReports.Infrastructure.Core.Repositories
{
    public class EFRepository<T> : IRepository<T>, IDisposable where T : class
    {
        public EFRepository(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        #region Properties

        protected DbContext _dbContext;

        protected DbSet<T> _dbSet;

        #endregion Properties

        #region IRepository Methods

        public virtual int Count
        {
            get { return _dbSet.Count(); }
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public virtual T GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual T OneOrDefault(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = this.GetAll();
            if (includes != null && includes.Any())
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }
            return query.FirstOrDefault(filter);
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = this.GetAll();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null && includes.Any())
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }

            return query;
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, out int total, int index = 0, int size = 50, params Expression<Func<T, object>>[] includes)
        {
            int skipCount = index * size;
            var query = this.Get(filter, orderBy, includes);
            total = query.Count();
            query = skipCount == 0 ? query.Take(size) : query.Skip(skipCount).Take(size);
            return query;
        }

        public virtual bool Contains(Expression<Func<T, bool>> predicate)
        {
            return this.GetAll().Count(predicate) > 0;
        }

        public virtual void Add(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                DbEntityEntry dbEntityEntry = _dbContext.Entry<T>(entity);
                if (dbEntityEntry.State != EntityState.Detached)
                {
                    dbEntityEntry.State = EntityState.Added;
                }
                else
                {
                    _dbSet.Add(entity);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                LogEntityValidationException(dbEx);
            }
        }

        public virtual void Add(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                _dbSet.AddRange(entities);
            }
            catch (DbEntityValidationException dbEx)
            {
                LogEntityValidationException(dbEx);
            }
        }

        // see: http://stackoverflow.com/questions/12585664/an-object-with-the-same-key-already-exists-in-the-objectstatemanager-the-object/12587752#12587752
        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                DbEntityEntry dbEntityEntry = _dbContext.Entry<T>(entity);
                if (dbEntityEntry.State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                dbEntityEntry.State = EntityState.Modified;
            }
            catch (DbEntityValidationException dbEx)
            {
                LogEntityValidationException(dbEx);
            }
        }

        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                DbEntityEntry dbEntityEntry = _dbContext.Entry<T>(entity);
                if (dbEntityEntry.State != EntityState.Deleted)
                {
                    dbEntityEntry.State = EntityState.Deleted;
                }
                else
                {
                    _dbSet.Attach(entity);
                    _dbSet.Remove(entity);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                LogEntityValidationException(dbEx);
            }
        }

        public virtual void Delete(object id)
        {
            var entity = GetById(id);
            if (entity == null)
                return;// not found; assume already deleted.

            Delete(entity);
        }

        /// <summary>
        /// Logs exceptions of type DbEntityValidationException.
        /// </summary>
        /// <param name="dbEx">The exception to log.</param>
        protected virtual void LogEntityValidationException(DbEntityValidationException dbEx)
        {
            var msg = new StringBuilder();

            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    msg.AppendFormat("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    msg.AppendLine();
                }
            }

            // ToDo: Errors should be logged.
            var fail = new Exception(msg.ToString(), dbEx);
            throw fail;
        }

        public virtual void BatchDelete(Expression<Func<T, bool>> filter)
        {
            _dbSet.Where(filter).Delete();
        }

        public virtual void BatchUpdate(Expression<Func<T, bool>> filter, Expression<Func<T, T>> updateFactory)
        {
            _dbSet.Where(filter).Update(updateFactory);
        }

        #endregion IRepository Methods

        #region IDisposable

        public virtual void Dispose()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }

        #endregion IDisposable
    }
}