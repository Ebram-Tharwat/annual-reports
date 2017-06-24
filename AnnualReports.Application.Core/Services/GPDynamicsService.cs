using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Infrastructure.Core.DbContexts.DistDb;
using AnnualReports.Infrastructure.Core.DbContexts.GcDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class GPDynamicsService : IGPDynamicsService
    {
        private readonly IUnitOfWork<DistDbContext> _distUow;
        private readonly IUnitOfWork<GcDbContext> _gcUow;
        private readonly IDistDbFundRepository _distDbFundRepo;
        private readonly IGcDbFundRepository _gcDbFundRepo;

        public GPDynamicsService(IUnitOfWork<DistDbContext> distUow, IUnitOfWork<GcDbContext> gcUow, IDistDbFundRepository distDbFundRepo, IGcDbFundRepository gcDbFundRepo)
        {
            _distUow = distUow;
            _gcUow = gcUow;
            _distDbFundRepo = distDbFundRepo;
            _gcDbFundRepo = gcDbFundRepo;
        }

        public List<GPDynamicsFundDetails> GetAllFunds(DbSource dbSource = DbSource.ALL)
        {
            var distFunds = _distDbFundRepo.GetFundDetails();
            var gcFunds = _gcDbFundRepo.GetFundDetails();
            switch (dbSource)
            {
                case DbSource.DIST:
                    return distFunds.ToList();

                case DbSource.GC:
                    return gcFunds.ToList();

                default:
                    return distFunds.ToList().Union(gcFunds.ToList()).ToList();
            }
        }
    }
}