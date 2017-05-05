using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Domain.Core.GcDbModels;
using System.Collections.Generic;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IGcDbFundRepository : IRepository<Gl00100>
    {
        IEnumerable<GPDynamicsFundDetails> GetFundDetails();
    }
}