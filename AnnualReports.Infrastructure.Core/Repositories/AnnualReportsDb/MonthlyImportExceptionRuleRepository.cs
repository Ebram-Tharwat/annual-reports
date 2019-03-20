using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb
{
    public class MonthlyImportExceptionRuleRepository : AnnualReportsDbEfRepository<MonthlyImportFundExceptionRule>,
                                                        IMonthlyImportExceptionRuleRepository
    {
        public MonthlyImportExceptionRuleRepository(AnnualReportsDbContext dbContext) : base(dbContext)
        {
        }
    }
}
