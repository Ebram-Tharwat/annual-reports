using AnnualReports.Application.Core.Contracts;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IGPDynamicsService
    {
        List<GPDynamicsFundViewModel> GetAllFunds(DbSource dbSource);
    }
}