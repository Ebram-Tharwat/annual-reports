using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;
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

        void SyncFunds(Int16 year, DbSource dbSource);

        List<Fund> GetAllFunds(Int16 year, DbSource dbSource, PagingInfo pagingInfo = null);
    }
}