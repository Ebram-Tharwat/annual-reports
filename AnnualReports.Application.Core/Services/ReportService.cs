using AnnualReports.Application.Core.Contracts.MappingRuleEntities;
using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Infrastructure.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IAnnualReportsDbFundRepository _fundsRepository;
        private readonly IBarService _barService;
        private readonly IMappingRuleRepository _mappingRuleRepository;
        private const int AllPeriodsValue = 13;

        public ReportService(IAnnualReportsDbFundRepository fundsRepository, IBarService barService, IMappingRuleRepository mappingRuleRepository)
        {
            this._fundsRepository = fundsRepository;
            this._barService = barService;
            this._mappingRuleRepository = mappingRuleRepository;
        }

        public List<AnnualReportDataItemDetails> GetAnnualReportData(int year, int? fundId = null, string barNumber = null)
        {
            var reportData = new List<AnnualReportDataItemDetails>();
            // 1- get all possible combination of funds*bars
            var annualReportData = _fundsRepository.GetAnnualReportDataRows(year, fundId);
            // 2- get all possible/valid bars
            var dbBars = _barService.GetAllBars(year, null, null, true);
            var viewBars = annualReportData.Select(t => t.View_BarNumber).Distinct().ToList();
            // 3- get bar mapping rules to get right MapTo list.
            //var mappingRules = _mappingRuleRepository.Get(t => t.Year == year).ToList();
            // 4- generate report item detail.
            var groupByFundNumber = annualReportData.GroupBy(t => new { t.PrimaryFundNumber, t.FundDisplayName, t.MCAG, t.DbSource }
            , (gKey, gValue) => new AnnualReportDataItemGroup()
            {
                PrimaryFundNumber = gKey.PrimaryFundNumber,
                FundDisplayName = gKey.FundDisplayName,
                MCAG = gKey.MCAG,
                DbSource = gKey.DbSource,
                GroupData = gValue.ToList()
            }).ToList();

            foreach (var fundGroup in groupByFundNumber)
            {
                if (fundGroup.DbSource == DbSource.GC)
                    reportData.AddRange(GetGcAnnualReportItems(year, viewBars, dbBars, fundGroup));
                else
                    reportData.AddRange(GetDistAnnualReportItems(year, viewBars, dbBars, fundGroup));
            }
            return reportData;
        }

        public List<ExceptionReportDataItemDetails> GetDistExceptionReportData(int year)
        {
            var distExceptionBarByYear = _barService.GetDistExceptionByYear(year);
            return distExceptionBarByYear;
        }

        public List<ExceptionReportDataItemDetails> GetGcExceptionReportData(int year)
        {
            var distExceptionBarByYear = _barService.GetGcExceptionByYear(year);
            return distExceptionBarByYear;
        }

        #region Helpers

        private IEnumerable<AnnualReportDataItemDetails> GetGcAnnualReportItems(int year, List<string> viewBars, List<Bar> dbBars, AnnualReportDataItemGroup fundGroup)
        {
            var barReportItems = new List<BarAnnualReportItem>();
            foreach (var targetViewBar in viewBars)
            {
                var targetBar = GetGcTargetBarMappings(dbBars, targetViewBar);
                if (targetBar == null)
                    continue;

                var fundRows = fundGroup.GroupData;
                if (targetBar.Period.HasValue)
                {
                    if (Enumerable.Range(0, 13).Contains(targetBar.Period.Value)) // 0, 13 == 0..12
                        fundRows = fundRows.Where(t => t.View_Period == targetBar.Period.Value).ToList();
                    else if (targetBar.Period.Value == AllPeriodsValue)
                        fundRows = fundRows.Where(t => Enumerable.Range(0, 13).Contains(t.View_Period)).ToList();
                }
                else
                    fundRows = fundRows.Where(t => Enumerable.Range(1, 12).Contains(t.View_Period)).ToList();

                fundRows = fundRows.Where(t => t.View_BarNumber == targetViewBar).ToList();

                if (fundRows.Any())
                {
                    barReportItems.Add(new BarAnnualReportItem
                    {
                        BarNumber = targetBar.BarNumber,
                        BarDisplayName = targetBar.DisplayName,
                        BarDbSource = targetBar.DbSource,
                        Amount = GetGcBarTotalAmount(fundRows, targetBar.BarNumber),
                        Rows = fundRows
                    });
                }
            }

            return barReportItems.GroupBy(t => new { t.BarNumber, t.BarDisplayName }, (gKey, gValue) => new AnnualReportDataItemDetails()
            {
                FundNumber = fundGroup.PrimaryFundNumber,
                FundDisplayName = fundGroup.FundDisplayName,
                FundDbSource = fundGroup.DbSource,
                BarNumber = gKey.BarNumber,
                BarDisplayName = gKey.BarDisplayName,
                Year = year,
                MCAG = fundGroup.MCAG,
                Rows = gValue.SelectMany(t => t.Rows).ToList(),
                Amount = gValue.Sum(t => t.Amount)
            }).ToList();
        }

        private IEnumerable<AnnualReportDataItemDetails> GetDistAnnualReportItems(int year, List<string> viewBars, List<Bar> dbBars, AnnualReportDataItemGroup fundGroup)
        {
            var barReportItems = new List<BarAnnualReportItem>();
            foreach (var targetViewBar in viewBars)
            {
                var targetBarMappings = GetDistTargetBarMappings(dbBars, targetViewBar);
                if (targetBarMappings.Count == 0)
                    continue;

                var fundRows = fundGroup.GroupData.Where(t => t.View_BarNumber == targetViewBar).ToList();
                if (fundRows.Any())
                {
                    foreach (var targetBarMapping in targetBarMappings)
                    {
                        var fundPeriodsByPeriod = fundRows;

                        if (targetBarMapping.Period.HasValue)
                        {
                            if (Enumerable.Range(0, 13).Contains(targetBarMapping.Period.Value)) // 0, 13 == 0..12
                                fundPeriodsByPeriod = fundPeriodsByPeriod.Where(t => t.View_Period == targetBarMapping.Period.Value).ToList();
                            else if (targetBarMapping.Period.Value == AllPeriodsValue)
                                fundPeriodsByPeriod = fundPeriodsByPeriod.Where(t => Enumerable.Range(0, 13).Contains(t.View_Period)).ToList();
                        }
                        else
                            fundPeriodsByPeriod = fundPeriodsByPeriod.Where(t => Enumerable.Range(1, 12).Contains(t.View_Period)).ToList();

                        if (fundPeriodsByPeriod.Any())
                        {
                            barReportItems.Add(new BarAnnualReportItem
                            {
                                BarNumber = targetBarMapping.BarNumber,
                                BarDbSource = targetBarMapping.DbSource,
                                Amount = GetDistBarTotalAmount(fundPeriodsByPeriod, targetBarMapping, targetViewBar),
                                Rows = fundPeriodsByPeriod
                            });
                        }
                    }
                }
            }
            return barReportItems.GroupBy(t => new { t.BarNumber }, (gKey, gValue) => new AnnualReportDataItemDetails()
            {
                FundNumber = fundGroup.PrimaryFundNumber,
                FundDisplayName = fundGroup.FundDisplayName,
                FundDbSource = fundGroup.DbSource,
                BarNumber = gKey.BarNumber,
                //    BarDisplayName = targetBar.DisplayName,
                //    MapToBarNumber = targetBar.MapToBarNumber,
                Year = year,
                MCAG = fundGroup.MCAG,
                Rows = gValue.SelectMany(t => t.Rows).ToList(),
                Amount = gValue.Sum(t => t.Amount)
            }).ToList();
        }

        private List<BarMappingRuleItem> GetGcBarMappedItems(Bar targetBar)
        {
            var mapToBarList = new List<BarMappingRuleItem>();
            if (targetBar.DbSource == DbSource.GC)
            {
                if (string.IsNullOrWhiteSpace(targetBar.MapToBarNumber))
                {
                    mapToBarList.Add(new BarMappingRuleItem(targetBar.BarNumber, targetBar.BarNumber));
                }
                else
                {
                    mapToBarList = targetBar.MapToBarNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => item.Trim())
                        .Select(item => new BarMappingRuleItem(targetBar.BarNumber, item)).ToList();
                }
            }

            return mapToBarList;
        }

        private Bar GetGcTargetBarMappings(List<Bar> dbBars, string bar)
        {
            return dbBars.FirstOrDefault(t => (t.DbSource.HasValue && t.DbSource.Value == DbSource.GC)
                    && t.MapToBarNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim())
                    .Any(item => bar.StartsWith(item)));
        }

        public List<Bar> GetDistTargetBarMappings(List<Bar> dbBars, string bar)
        {
            return dbBars.Where(t => (t.DbSource.HasValue && t.DbSource.Value == DbSource.DIST)
                    && t.MapToBarNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim())
                    .Any(item => bar.StartsWith(item))).ToList();
        }

        private decimal GetGcBarTotalAmount(IEnumerable<AnnualReportDataRow> fundRows, string targetBarNumber)
        {
            if (targetBarNumber.StartsWith("5") || targetBarNumber.StartsWith("1"))
                return fundRows.Sum(t => t.Debit - t.Credit);
            else
                return fundRows.Sum(t => t.Credit - t.Debit);
        }

        private decimal GetDistBarTotalAmount(IEnumerable<AnnualReportDataRow> fundRows, Bar targetBarMapping, string targetViewBar)
        {
            var mapToBarNumbers = targetBarMapping.MapToBarNumber.Split(new char[] { ',' }
                                        , StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim());

            if (mapToBarNumbers.Any(item => item == "3") && targetViewBar.StartsWith("3"))
                return fundRows.Sum(t => t.Credit) - fundRows.Sum(t => t.Debit);
            else if (mapToBarNumbers.Any(item => item == "5") && targetViewBar.StartsWith("5"))
                return fundRows.Sum(t => t.Debit) - fundRows.Sum(t => t.Credit);
            else if (targetBarMapping.BarTarget == null)
                return fundRows.Sum(t => t.Debit) - fundRows.Sum(t => t.Credit);
            else
            {
                if (targetBarMapping.BarTarget == BarNumberTarget.Credit)
                    return fundRows.Sum(t => t.Credit);
                else
                    return fundRows.Sum(t => t.Debit);
            }
        }

        #endregion Helpers
    }
}