using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class FundService : IFundService
    {
        private readonly IGPDynamicsService _gpDynamicsService;
        private readonly IRepository<Fund> _fundsRepository;
        private readonly IUnitOfWork<AnnualReportsDbContext> _uow;

        public FundService(IGPDynamicsService gpDynamicsService, IRepository<Fund> fundsRepository, IUnitOfWork<AnnualReportsDbContext> uow)
        {
            _gpDynamicsService = gpDynamicsService;
            this._fundsRepository = fundsRepository;
            _uow = uow;
        }

        public void SyncFunds(Int16 year, DbSource dbSource)
        {
            var gpFunds = _gpDynamicsService.GetAllFunds(dbSource);
            var existedFunds = Enumerable.Empty<Fund>();
            if (dbSource == DbSource.ALL)
            {
                existedFunds = _fundsRepository.Get(null, null, t => t.MapToFund).ToList();
            }
            else
            {
                existedFunds = _fundsRepository.Get(t => t.DbSource == dbSource).ToList();
            }
            var fundsToAdd = gpFunds.Where(gpFund => existedFunds.All(t => t.FundNumber != gpFund.Number));
            var fundsToDelete = existedFunds.Where(t => gpFunds.All(gpFund => gpFund.Number != t.FundNumber));
            var fundsToUpdate = existedFunds.Where(t => gpFunds.Any(gpFund => gpFund.Number == t.FundNumber
            && !gpFund.Description.Equals(t.GpDescription, StringComparison.InvariantCultureIgnoreCase)));

            // insert new funds
            foreach (var fund in fundsToAdd)
            {
                _fundsRepository.Add(new Fund()
                {
                    FundNumber = fund.Number,
                    GpDescription = fund.Description,
                    DbSource = fund.DbSource,
                    IsActive = true,
                    DisplayName = fund.Description,
                    Year = year,
                    MapToFundId = null
                });
            }

            foreach (var fund in fundsToDelete)
            {
                _fundsRepository.Delete(fund);
            }

            foreach (var fund in fundsToUpdate)
            {
                fund.GpDescription = gpFunds.FirstOrDefault(t => t.Number == fund.FundNumber).Description;
                _fundsRepository.Update(fund);
            }

            _uow.Commit();
        }

        public List<Fund> GetAllFunds(Int16 year, DbSource dbSource)
        {
            if (dbSource == DbSource.ALL)
            {
                return _fundsRepository.Get(null, null, t => t.MapToFund).ToList();
            }
            else
            {
                return _fundsRepository.Get(t => t.DbSource == dbSource).ToList();
            }
        }
    }
}