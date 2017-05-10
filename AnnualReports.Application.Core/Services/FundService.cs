using AnnualReports.Application.Core.Contracts.FundEntities;
using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class FundService : IFundService
    {
        private readonly IGPDynamicsService _gpDynamicsService;
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IUnitOfWork<AnnualReportsDbContext> _uow;

        public FundService(IGPDynamicsService gpDynamicsService, IAnnualReportsDbFundRepository fundsRepository, IUnitOfWork<AnnualReportsDbContext> uow)
        {
            _gpDynamicsService = gpDynamicsService;
            this._fundsRepository = fundsRepository;
            _uow = uow;
        }

        public List<Fund> SyncFunds(int year, DbSource dbSource)
        {
            var gpFunds = _gpDynamicsService.GetAllFunds(dbSource);
            var existedFunds = this.GetAllFunds(year, dbSource);
            var fundsToAdd = gpFunds.Where(gpFund => existedFunds.All(t => t.FundNumber != gpFund.Number)).ToList();
            var fundsToDelete = existedFunds.Where(t => gpFunds.All(gpFund => gpFund.Number != t.FundNumber)).ToList();
            var fundsToUpdate = existedFunds.Where(t => gpFunds.Any(gpFund => gpFund.Number == t.FundNumber
            && !gpFund.Description.Equals(t.GpDescription, StringComparison.InvariantCultureIgnoreCase))).ToList();
            var fundsToSkip = existedFunds.Where(t => gpFunds.Any(gpFund => gpFund.Number == t.FundNumber
            && gpFund.Description.Equals(t.GpDescription, StringComparison.InvariantCultureIgnoreCase))).ToList();

            // insert new funds
            var newFunds = fundsToAdd.Select(fund => new Fund()
            {
                FundNumber = fund.Number,
                GpDescription = fund.Description,
                DbSource = fund.DbSource,
                IsActive = true,
                DisplayName = fund.Description,
                Year = (short)year,
                MapToFundId = null
            }).ToList();
            _fundsRepository.Add(newFunds);

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

            return newFunds.Union(fundsToUpdate).Union(fundsToSkip).ToList();
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

        public List<Fund> AddUploadedFunds(int year, List<FundAddEntity> uploadedFunds, out List<FundAddEntity> rejectedFunds)
        {
            rejectedFunds = new List<FundAddEntity>();
            var validFunds = new List<Fund>();
            var dbFunds = this.GetAllFunds(year, DbSource.ALL);
            foreach (var addFund in uploadedFunds)
            {
                var dbEntity = dbFunds.FirstOrDefault(t => t.FundNumber == addFund.FundNumber);
                if (dbEntity == null)
                    rejectedFunds.Add(addFund);
                else
                {
                    Mapper.Map(addFund, dbEntity);
                    if (addFund.MapTo == addFund.FundNumber || string.IsNullOrEmpty(addFund.MapTo))
                    {
                        dbEntity.MapToFundId = null;
                        dbEntity.MapToFund = null;
                        _fundsRepository.Update(dbEntity);
                        validFunds.Add(dbEntity);
                    }
                    else
                    {
                        var dbMapToFund = dbFunds.FirstOrDefault(t => t.FundNumber == addFund.MapTo);
                        if (dbMapToFund == null)
                            rejectedFunds.Add(addFund);
                        else
                        {
                            dbEntity.MapToFund = dbMapToFund;
                            _fundsRepository.Update(dbEntity);
                            validFunds.Add(dbEntity);
                        }
                    }
                }
            }
            _uow.Commit();

            return validFunds;
        }

        public List<Fund> CopyFunds(int fromYear, int toYear)
        {
            // 1- sync data to make sure that funds to copies exist in GP.
            this.SyncFunds(fromYear, DbSource.ALL);
            // 2- remove any existing funds in the year to copy to.
            this.RemoveFunds(toYear, DbSource.ALL);
            // 3- copy funds.
            var dbFunds = this.GetAllFunds(fromYear, DbSource.ALL);
            var parentFunds = dbFunds.Where(t => t.MapToFundId == null || (t.MapToFundId.HasValue && t.MapToFund.FundNumber == t.FundNumber))
                .ToList();
            var childFunds = dbFunds.Except(parentFunds);

            // add all funds which map to themselves.
            var parentFundsToAdd = parentFunds.Select(t =>
            {
                t.Year = (short)toYear;
                return t;
            }).ToList();
            _fundsRepository.Add(parentFundsToAdd);
            _uow.Commit(); // commit changes to get the Id value.

            var childFundsToAdd = childFunds.Select(t =>
            {
                var mapto = parentFunds.FirstOrDefault(p => p.FundNumber == t.MapToFund.FundNumber);
                t.MapToFundId = mapto?.Id;
                t.Year = (short)toYear;
                return t;
            }).ToList();
            _fundsRepository.Add(childFundsToAdd);
            _uow.Commit();

            return parentFundsToAdd.Union(childFundsToAdd).ToList();
        }

        public void RemoveFunds(int year, DbSource dbSource)
        {
            if (dbSource == DbSource.ALL)
                _fundsRepository.Delete(t => t.Year == year && (t.DbSource == DbSource.DIST || t.DbSource == DbSource.GC));
            else
                _fundsRepository.Delete(t => t.Year == year && (t.DbSource == dbSource));
        }
    }
}