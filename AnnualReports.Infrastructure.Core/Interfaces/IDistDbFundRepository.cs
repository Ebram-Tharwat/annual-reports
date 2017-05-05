using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Domain.Core.DistDbModels;
using System.Collections.Generic;

namespace AnnualReports.Infrastructure.Core.Interfaces
{
    public interface IDistDbFundRepository : IRepository<Gl00100>
    {
        IEnumerable<GPDynamicsFundDetails> GetFundDetails();
    }
}