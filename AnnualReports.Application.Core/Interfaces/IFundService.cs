using AnnualReports.Application.Core.Contracts.FundEntities;
using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IFundService
    {
        /// <summary>
        /// Sync existing funds (in default DB), with funds existed in GP Dynamics db
        /// </summary>
        /// <param name="year">year to compare existing data against</param>
        /// <param name="dbSource">which db to sync with</param>

        List<Fund> SyncFunds(int year, DbSource dbSource);

        List<Fund> GetAllFunds(int? year, DbSource? dbSource = null, string displayName = null, string fundNumber = null, bool? isActive = null, PagingInfo pagingInfo = null);

        List<Fund> AddUploadedFunds(int year, List<FundAddEntity> uploadedFunds, out List<FundAddEntity> rejectedFunds);

        List<Fund> CopyFunds(int fromYear, int toYear);

        void RemoveFunds(int year, DbSource dbSource);

        List<FundBasicInfo> GetPrimaryFunds(int year, DbSource dbSource, PagingInfo pagingInfo = null);

        Fund GetById(int id);

        Fund GetByFundNumberAndYear(string fundNumber, int year);

        void Update(Fund fund);

        void Add(IEnumerable<Fund> entities);
    }
}