using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;

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
    }
}