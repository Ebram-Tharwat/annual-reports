using AnnualReports.Application.Core.Contracts.Paging;
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

        public void SyncFunds(int year, DbSource dbSource)
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
                    Year = (short)year,
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

        public List<Fund> GetAllFunds(int year, DbSource dbSource, PagingInfo pagingInfo = null)
        {
            if (dbSource == DbSource.ALL)
            {
                if (pagingInfo == null)
                    return _fundsRepository.Get(t => t.Year == year, (list => list.OrderBy(t => t.FundNumber)), t => t.MapToFund).ToList();
                else
                {
                    int total = 0;
                    var result = _fundsRepository.Get(t => t.Year == year, (list => list.OrderBy(t => t.FundNumber))
                        , out total, pagingInfo.PageIndex, AppSettings.PageSize,
                        t => t.MapToFund).ToList();
                    pagingInfo.Total = total;
                    return result;
                }
            }
            else
            {
                if (pagingInfo == null)
                    return _fundsRepository.Get(t => t.Year == year && t.DbSource == dbSource
                            , (list => list.OrderBy(t => t.FundNumber))).ToList();
                else
                {
                    int total = 0;
                    var result = _fundsRepository.Get(t => t.Year == year && t.DbSource == dbSource
                        , (list => list.OrderBy(t => t.FundNumber))
                        , out total, pagingInfo.PageIndex, AppSettings.PageSize,
                        t => t.MapToFund).ToList();
                    pagingInfo.Total = total;
                    return result;
                }
            }
        }
    }
}