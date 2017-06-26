using AnnualReports.Domain.Core.AnnualReportsDbModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IUnitOfWork<TContext> where TContext : IdentityDbContext<ApplicationUser>
    {
        void Commit();
    }
}