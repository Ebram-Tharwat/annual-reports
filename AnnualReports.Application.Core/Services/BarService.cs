﻿using AnnualReports.Application.Core.Contracts.BarEntities;
using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Extensions;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.DistDbModels;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using AnnualReports.Infrastructure.Core.Repositories.DistDb;
using AnnualReports.Infrastructure.Core.Repositories.GcDb;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class BarService : IBarService
    {
        private readonly IAnnualReportsDbBarRepository _barRepository;
        private readonly DistDbEfRepository<Gl00100> _distDbRepository;
        private readonly GcDbEfRepository<AnnualReports.Domain.Core.GcDbModels.Gl00100> _gcDbRepository;
        private readonly IUnitOfWork<AnnualReportsDbContext> _uow;

        public BarService(IAnnualReportsDbBarRepository barRepository, DistDbEfRepository<Gl00100> distDbRepository, GcDbEfRepository<AnnualReports.Domain.Core.GcDbModels.Gl00100> gcDbRepository, IUnitOfWork<AnnualReportsDbContext> uow)
        {
            _barRepository = barRepository;
            _uow = uow;
            _distDbRepository = distDbRepository;
            _gcDbRepository = gcDbRepository;
        }


        public List<DistOrGcReportDataItemDetails> GetDistExceptionByYear(int year)
        {
            //step 1 get all bars in the selected year from Dist DB
            var distBars = _distDbRepository.Get(dist => dist.Active == 1 && dist.Creatddt.Year == year).ToList();
            if (distBars == null)
            {
                return null;
            }
            //step 2 get all bars in the selected year from Annual report Db
            var annualReportBars = _barRepository.Get(dist => dist.IsActive && dist.Year == year).ToList();
            if(annualReportBars == null)
            {
                return null;
            }
            List<DistOrGcReportDataItemDetails> results = new List<DistOrGcReportDataItemDetails>();
            //step 3 compare between two lists to find difference that found in dist and was not fount in Annual report
            foreach (var dist in distBars)
            {
                if(annualReportBars.FirstOrDefault(bar => bar.MapToBarNumber != null && dist.Actnumbr3!=null && 
                                                          (bar.MapToBarNumber == dist.Actnumbr3.Substring(0,4) || 
                                                           bar.MapToBarNumber == dist.Actnumbr3.Substring(0,6))) != null)
                {
                    continue;
                }
                else
                {
                    results.Add(new DistOrGcReportDataItemDetails
                    {
                        AccountIndex = dist.Actindx,
                        ActDesc = dist.Actdescr,
                        ActNum1 = dist.FundNumber,
                        ActNum2 = dist.Actnumbr2,
                        ActNum3 = dist.Actnumbr3,
                        ActNum4 = dist.Actnumbr4,
                        ActNum5 = dist.Actnumbr5,
                        ActType = dist.Accttype
                    });
                }
            }
            return results;

        }

        public List<DistOrGcReportDataItemDetails> GetGcExceptionByYear(int year)
        {
            //step 1 get all bars in the selected year from Dist DB
            var distBars = _gcDbRepository.Get(dist => dist.Active == 1 && dist.Creatddt.Year == year).ToList();
            if (distBars == null)
            {
                return null;
            }
            //step 2 get all bars in the selected year from Annual report Db
            var annualReportBars = _barRepository.Get(dist => dist.IsActive && dist.Year == year).ToList();
            if (annualReportBars == null)
            {
                return null;
            }
            List<DistOrGcReportDataItemDetails> results = new List<DistOrGcReportDataItemDetails>();
            //step 3 compare between two lists to find difference that found in dist and was not fount in Annual report
            foreach (var dist in distBars)
            {
                if (annualReportBars.FirstOrDefault(bar => bar.MapToBarNumber != null && dist.Actnumbr5 != null &&
                                                           (bar.MapToBarNumber == dist.Actnumbr5.Substring(0, 5) ||
                                                            bar.MapToBarNumber == dist.Actnumbr5.Substring(0, 7))) != null)
                {
                    continue;
                }
                else
                {
                    results.Add(new DistOrGcReportDataItemDetails
                    {
                        AccountIndex = dist.Actindx,
                        ActDesc = dist.Actdescr,
                        ActNum1 = dist.FundNumber,
                        ActNum2 = dist.Actnumbr2,
                        ActNum3 = dist.Actnumbr3,
                        ActNum4 = dist.Actnumbr4,
                        ActNum5 = dist.Actnumbr5,
                        ActType = dist.Accttype
                    });
                }
            }
            return results;
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
            if (entity.BarNumber.Length == 5 && entity.BarNumber.StartsWith("5")) // if BarNumber length is 5 and starts with 5
            {
                var trickyBars = HandleTrickyBarNumbers(entity);
                // load tricky bars from DB. Note that bar number lenght is equal to 5 + XX ; where XX is (10,20 ... 90)
                var dbTrickyBars = _barRepository.Get(t => t.Year == entity.Year && t.BarNumber.StartsWith(entity.BarNumber) && t.BarNumber.Length == 7).ToList();
                if (dbTrickyBars.Any())
                {
                    dbTrickyBars.ForEach(t =>
                    {
                        t.DisplayName = entity.DisplayName;
                        t.Period = entity.Period;
                        _barRepository.Update(t);
                    });
                }
                else
                {
                    this.Add(trickyBars);
                }
            }
            _uow.Commit();
        }

        public void UploadBars(int year, List<BarUploadEntity> excelData, out int numOfAddedEntities, out int numOfUpdatedEntities)
        {
            var existedEntities = this.GetByYear(year);
            var entitiesToAdd = new List<Bar>();
            var entitiesToUpdate = new List<Bar>();
            foreach (var entityViewModel in excelData)
            {
                var existedEntity = existedEntities.FirstOrDefault(t => t.BarNumber == entityViewModel.BarNumber && t.Year == entityViewModel.Year);
                if (existedEntity == null)
                {
                    var entity = Mapper.Map<BarUploadEntity, Bar>(entityViewModel);
                    entitiesToAdd.Add(entity);
                }
                else
                {
                    Mapper.Map(entityViewModel, existedEntity);
                    entitiesToUpdate.Add(existedEntity);

                    if (existedEntity.BarNumber.Length == 5 && existedEntity.BarNumber.StartsWith("5")) // if BarNumber length is 5 and starts with 5
                    {
                        var trickyBars = HandleTrickyBarNumbers(existedEntity);
                        trickyBars.ForEach(trickybar =>
                        {
                            var existedTrickyBar = existedEntities.FirstOrDefault(t => t.BarNumber == trickybar.BarNumber && t.Year == trickybar.Year);
                            if (existedTrickyBar == null)
                            {
                                entitiesToAdd.Add(trickybar);
                            }
                            else
                            {
                                Mapper.Map(trickybar, existedTrickyBar);
                                entitiesToUpdate.Add(existedTrickyBar);
                            }
                        });
                    }
                }
            }

            this.Add(entitiesToAdd);
            entitiesToUpdate.ForEach(t => _barRepository.Update(t));
            _uow.Commit();

            numOfAddedEntities = entitiesToAdd.Count;
            numOfUpdatedEntities = entitiesToUpdate.Count;
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
                //if (sourceBarObj.MapToBarNumber.EndsWith("*"))
                //{
                extraEntity.MapToBarNumber = "";
                for (int i = 0; i < 10; i++)
                {
                    extraEntity.MapToBarNumber = extraEntity.MapToBarNumber + (int.Parse(extraEntity.BarNumber) + i).ToString() + ",";
                }
                //}
                result.Add(extraEntity);
            }

            return result;
        }
    }
}