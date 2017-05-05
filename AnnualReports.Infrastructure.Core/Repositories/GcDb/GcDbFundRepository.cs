using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Domain.Core.GcDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.GcDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Infrastructure.Core.Repositories.GcDb
{
    public class GcDbFundRepository : GcDbEfRepository<Gl00100>, IGcDbFundRepository
    {
        public GcDbFundRepository(GcDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<GPDynamicsFundDetails> GetFundDetails()
        {
            var query = from fund in _dbContext.Gl00100
                        join fundDescription in _dbContext.Gl40200 on fund.FundNumber equals fundDescription.Sgmntid into fundDetails
                        from item in fundDetails.DefaultIfEmpty()
                        where fund.Active == 1 && !string.IsNullOrEmpty(item.FundDescription)
                        select new GPDynamicsFundDetails()
                        {
                            Number = fund.FundNumber,
                            Description = (item == null) ? "" : item.FundDescription,
                            DbSource = DbSource.GC
                        };

            return query.Distinct().ToList();
        }
    }
}