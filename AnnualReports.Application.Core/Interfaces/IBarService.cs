using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IBarService
    {
        void Add(IEnumerable<Bar> entities);
        void Update(Bar entity);

        List<Bar> GetAllBars(int year,  PagingInfo pagingInfo = null);

    }
}
