using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AnnualReports.Infrastructure.Core.Extensions
{
    public static class DbContextExtensions
    {
        public static int DeleteWhere<T>(this DbContext db, Expression<Func<T, bool>> filter) where T : class
        {
            var query = db.Set<T>().Where(filter);

            string selectSql = query.ToString();
            string deleteSql = "DELETE [Extent1] " + selectSql.Substring(selectSql.IndexOf("FROM"));

            var internalQuery = query.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.Name == "_internalQuery").Select(field => field.GetValue(query)).First();
            var objectQuery = internalQuery.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.Name == "_objectQuery").Select(field => field.GetValue(internalQuery)).First() as ObjectQuery;
            var parameters = objectQuery.Parameters.Select(p => new SqlParameter(p.Name, p.Value)).ToArray();

            return db.Database.ExecuteSqlCommand(deleteSql, parameters);
        }
    }
}