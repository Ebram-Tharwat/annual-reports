using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Domain.Core.DistDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.DistDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AnnualReports.Domain.Core.AnnualReportsDbModels;

namespace AnnualReports.Infrastructure.Core.Repositories.DistDb
{
    public class DistDbFundRepository : DistDbEfRepository<Gl00100>, IDistDbFundRepository
    {
        public DistDbFundRepository(DistDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<GPDynamicsFundDetails> GetFundDetails()
        {
            var query = from fund in _dbContext.Gl00100
                        join fundDescription in _dbContext.Gl40200 on fund.FundNumber equals fundDescription.Sgmntid into
                            fundDetails
                        from item in fundDetails.DefaultIfEmpty()
                        where fund.Active == 1 && !string.IsNullOrEmpty(item.FundDescription)
                        select new GPDynamicsFundDetails()
                        {
                            Number = fund.FundNumber,
                            Description = item.FundDescription,
                            DbSource = DbSource.DIST
                        } into fundFullDetails
                        group fundFullDetails by fundFullDetails.Number.Substring(0, 3) into fundGroup
                        select new GPDynamicsFundDetails()
                        {
                            Number = fundGroup.Key,
                            Description = fundGroup.FirstOrDefault().Description,
                            DbSource = DbSource.DIST
                        };


            var test = query.Distinct();
            return query.Distinct().ToList();
        }
    }
}