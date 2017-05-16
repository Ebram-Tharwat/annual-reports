using AnnualReports.Application.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Infrastructure.Core.Interfaces;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;

namespace AnnualReports.Application.Core.Services
{
    public class BarService : IBarService
    {
        private readonly IRepository<Bar> _barRepository;
        private readonly IUnitOfWork<AnnualReportsDbContext> _uow;
        public BarService(IRepository<Bar> barRepository, IUnitOfWork<AnnualReportsDbContext> uow)
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
            // 2- remove any existing bars in the year to copy to.
            var barsNeedToBeUpdated = _barRepository.Get(b => b.Year == toYear);
            if (barsNeedToBeUpdated != null && barsNeedToBeUpdated.Any())
            {
                foreach (var bar in barsNeedToBeUpdated)
                {
                    bar.MapToBarId = null;
                }
                _uow.Commit();
                this.RemoveBars(toYear);
            }
            // 3- copy bars.
            var dbFunds = this.GetAllBars(fromYear);
            var parentBars = dbFunds.Where(t => t.MapToBarId == null || (t.MapToBarId.HasValue && t.MapToBar.BarNumber == t.BarNumber))
                .ToList();
            var childFunds = dbFunds.Except(parentBars);

            // add all funds which map to themselves.
            var parentBarsToAdd = parentBars.Select(t =>
            {
                t.Year = (short)toYear;
                //t.Id = 0;
                t.MapToBarId = null;
                return t;
            }).ToList();
            _barRepository.Add(parentBarsToAdd);
            _uow.Commit(); // commit changes to get the Id value.

            var childFundsToAdd = childFunds.Select(t =>
            {
                var mapto = parentBars.FirstOrDefault(p => p.BarNumber == t.MapToBar.BarNumber);
                t.MapToBarId = mapto?.Id;
                t.Year = (short)toYear;
                return t;
            }).ToList();
            _barRepository.Add(childFundsToAdd);
            _uow.Commit();

            return parentBarsToAdd.Union(childFundsToAdd).ToList();
        }

        private void RemoveBars(int year)
        {
            _barRepository.Delete(t => t.Year == year);
        }

        public List<Bar> GetAllBars(int year, PagingInfo pagingInfo = null)
        {
            if (pagingInfo == null)
                return _barRepository.Get(t => t.Year == year, (list => list.OrderBy(t => t.BarNumber)), t => t.MapToBar).ToList();
            else
            {
                int total = 0;
                var result = _barRepository.Get(t => t.Year == year, (list => list.OrderBy(t => t.BarNumber))
                    , out total, pagingInfo.PageIndex, AppSettings.PageSize,
                    t => t.MapToBar).ToList();
                pagingInfo.Total = total;
                return result;
            }
        }

        public Bar GetByBarNumber(int barNumber)
        {
            return _barRepository.OneOrDefault(b => b.BarNumber == barNumber.ToString());
        }

        public List<Bar> GetByYear(int year)
        {
            var result = _barRepository.Get(b => b.Year == year);
            if(result == null)
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
