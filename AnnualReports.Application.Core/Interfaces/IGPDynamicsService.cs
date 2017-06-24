using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IGPDynamicsService
    {
        List<GPDynamicsFundDetails> GetAllFunds(DbSource dbSource = DbSource.ALL);
    }
}