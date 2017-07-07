using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Interfaces
{
    public interface IBarService
    {
        void Add(IEnumerable<Bar> entities);

        void Update(Bar entity);

        List<Bar> GetAllBars(int? year = null, string displayName = null, string barNumber = null, bool? isActive = null, PagingInfo pagingInfo = null);

        Bar GetByBarNumberAndYear(string barNumber, int year);

        List<Bar> GetByYear(int year);

        List<Bar> CopyBars(int fromYear, int toYear);

        Bar GetById(int id);
    }
}