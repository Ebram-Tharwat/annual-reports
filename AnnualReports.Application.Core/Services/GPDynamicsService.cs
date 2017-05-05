using AnnualReports.Application.Core.Contracts;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.DistDb;
using AnnualReports.Infrastructure.Core.DbContexts.GcDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class GPDynamicsService : IGPDynamicsService
    {
        private IUnitOfWork<DistDbContext> _distUow;
        private IUnitOfWork<GcDbContext> _gcUow;
        private IRepository<Domain.Core.DistDbModels.Gl00100> _distFundsRepo;
        private IRepository<Domain.Core.GcDbModels.Gl00100> _gcFundsRepo;

        public GPDynamicsService(IUnitOfWork<DistDbContext> distUow, IUnitOfWork<GcDbContext> gcUow, IRepository<Domain.Core.DistDbModels.Gl00100> distFundsRepo, IRepository<Domain.Core.GcDbModels.Gl00100> gcFundsRepo)
        {
            _distUow = distUow;
            _gcUow = gcUow;
            _distFundsRepo = distFundsRepo;
            _gcFundsRepo = gcFundsRepo;
        }

        public List<GPDynamicsFundViewModel> GetAllFunds(DbSource dbSource)
        {
            var distFunds = _distFundsRepo.Get(t => t.Active == 1).Select(t => new GPDynamicsFundViewModel() { Number = t.FundNumber });
            var gcFunds = _gcFundsRepo.Get(t => t.Active == 1).Select(t => new GPDynamicsFundViewModel() { Number = t.FundNumber });
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