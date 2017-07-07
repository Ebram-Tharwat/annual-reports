using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Extensions;
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
            var finalBars = new List<Bar>(entities);
            foreach (var entity in entities)
            {
                if (entity.BarNumber.Length == 5 && entity.BarNumber.StartsWith("5")) // if BarNumber length is 5 and starts with 5
                {
                    var sourceBarObj = finalBars.FirstOrDefault(t => t.BarNumber == entity.BarNumber);
                    finalBars.AddRange(HandleTrickyBarNumbers(sourceBarObj));
                    // update the orginal bar to be inactive.
                    sourceBarObj.IsActive = false;
                }
            }
            _barRepository.Add(finalBars);
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

        /// <summary>
        /// Tricky bars are the the bars that starts with 5 and have length of 5
        /// </summary>
        /// <returns></returns>
        private List<Bar> HandleTrickyBarNumbers(Bar sourceBarObj)
        {
            var result = new List<Bar>();
            // define the extra bars to add
            var extraBars = new List<string>() { "10", "20", "30", "40", "50", "60", "70", "80", "90" };
            foreach (var item in extraBars)
            {
                var extraEntity = sourceBarObj.CloneJson<Bar>();
                extraEntity.BarNumber = extraEntity.BarNumber + item;
                if (sourceBarObj.MapToBarNumber.EndsWith("*"))
                {
                    extraEntity.MapToBarNumber = "";
                    for (int i = 0; i < 10; i++)
                    {
                        extraEntity.MapToBarNumber = extraEntity.MapToBarNumber + (int.Parse(extraEntity.BarNumber) + i).ToString() + ",";
                    }
                }
                result.Add(extraEntity);
            }

            return result;
        }
    }
}