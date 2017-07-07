using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class BarService : IBarService
    {
        private readonly IAnnualReportsDbBarRepository _barRepository;
        private readonly IUnitOfWork<AnnualReportsDbContext> _uow;

        public BarService(IAnnualReportsDbBarRepository barRepository, IUnitOfWork<AnnualReportsDbContext> uow)
        {
            _barRepository = barRepository;
            _uow = uow;
        }

        public void Add(IEnumerable<Bar> entities)
        {
            _barRepository.Add(entities);
            _uow.Commit();
        }

        public List<Bar> CopyBars(int fromYear, int toYear)
        {
            // 1- remove any existing bars in the year to copy to.
            this.RemoveBars(toYear);

            // 2- copy bars.
            var dbBars = this.GetAllBars(fromYear);

            // 3- add bars
            var barsToAdd = dbBars.Select(t =>
            {
                t.Year = (short)toYear;
                return t;
            }).ToList();
            _barRepository.Add(barsToAdd);
            _uow.Commit(); // commit changes to get the Id value.

            return barsToAdd;
        }

        public Bar GetById(int id)
        {
            return _barRepository.GetById(id);
        }

        private void RemoveBars(int year)
        {
            _barRepository.BatchDelete(t => t.Year == year);
        }

        public List<Bar> GetAllBars(int? year = null, string displayName = null, string barNumber = null, bool? isActive = null, PagingInfo pagingInfo = null)
        {
            int total = 0;

            if (pagingInfo == null)
                return _barRepository.SearchForBars(year, displayName, barNumber, isActive, out total, 0, int.MaxValue).ToList();
            else
            {
                List<Bar> result = null;
                result = _barRepository.SearchForBars(year, displayName, barNumber, isActive, out total, pagingInfo.PageIndex, AppSettings.PageSize).ToList();
                pagingInfo.Total = total;
                return result;
            }
        }

        public Bar GetByBarNumberAndYear(string barNumber, int year)
        {
            return _barRepository.OneOrDefault(b => b.Year == year && b.BarNumber == barNumber);
        }

        public List<Bar> GetByYear(int year)
        {
            var result = _barRepository.Get(b => b.Year == year);
            if (result == null)
            {
                return null;
            }
            return result.ToList();
        }

        public void Update(Bar entity)
        {
            _barRepository.Update(entity);
            _uow.Commit();
        }
    }
}