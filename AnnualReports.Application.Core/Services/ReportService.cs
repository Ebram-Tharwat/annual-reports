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
        private const int _allPeriodsValue = 13;
        private const int _yearToExclude = 2020;
        private const string _barAccountToExclude = "211";

        private Lazy<List<string>> _periodZeroAllowedBars = new Lazy<List<string>>(
            () => Enumerable.Range(100, 100).Select(t => t.ToString()).ToList());

        public ReportService(
            IAnnualReportsDbFundRepository fundsRepository,
            IBarService barService)
        {
            this._fundsRepository = fundsRepository;
            this._barService = barService;
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
                var targetBarMappings = GetGcTargetBarMappings(dbBars, targetViewBar);
                if (targetBarMappings.Count == 0)
                    continue;

                foreach (var targetBarMapping in targetBarMappings)
                {
                    var fundRows = fundGroup.GroupData;
                    if (targetBarMapping.Period.HasValue)
                    {
                        if (Enumerable.Range(0, 13).Contains(targetBarMapping.Period.Value)) // 0, 13 == 0..12
                            fundRows = fundRows.Where(t => t.View_Period == targetBarMapping.Period.Value).ToList();
                        else if (targetBarMapping.Period.Value == _allPeriodsValue)
                            fundRows = fundRows.Where(t => Enumerable.Range(0, 13).Contains(t.View_Period)).ToList();
                    }
                    else
                    {
                        // If it is period 0, then include accounts that start with 100-199. Else, include everything.
                        fundRows = fundRows
                                   .Where(t => (t.View_Period == 0 &&
                                                _periodZeroAllowedBars.Value.Any(allowedBar => t.View_BarNumber.StartsWith(allowedBar)))
                                                || Enumerable.Range(1, 12).Contains(t.View_Period)).ToList();
                    }

                    fundRows = fundRows.Where(t => t.View_BarNumber == targetViewBar).ToList();

                    if (fundRows.Any())
                    {
                        barReportItems.Add(new BarAnnualReportItem
                        {
                            BarNumber = targetBarMapping.BarNumber,
                            BarDisplayName = targetBarMapping.DisplayName,
                            BarDbSource = targetBarMapping.DbSource,
                            Amount = GetGcBarTotalAmount(fundRows, targetBarMapping.BarNumber),
                            Rows = fundRows
                        });
                    }
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

                if (year >= _yearToExclude && targetViewBar.StartsWith(_barAccountToExclude))
                    continue;

                var fundRows = fundGroup.GroupData.Where(t => t.View_BarNumber == targetViewBar).ToList();
                if (fundRows.Any())
                {
                    foreach (var targetBarMapping in targetBarMappings)
                    {
                        var fundByPeriod = fundRows;

                        if (targetBarMapping.Period.HasValue)
                        {
                            if (Enumerable.Range(0, 13).Contains(targetBarMapping.Period.Value)) // 0, 13 == 0..12
                                fundByPeriod = fundByPeriod.Where(t => t.View_Period == targetBarMapping.Period.Value).ToList();
                            else if (targetBarMapping.Period.Value == _allPeriodsValue)
                                fundByPeriod = fundByPeriod.Where(t => Enumerable.Range(0, 13).Contains(t.View_Period)).ToList();
                        }
                        else
                        {
                            // If it is period 0, then include accounts that start with 100-199. Else, include everything.
                            fundByPeriod = fundByPeriod
                                       .Where(t => (t.View_Period == 0 &&
                                                    _periodZeroAllowedBars.Value.Any(allowedBar => t.View_BarNumber.StartsWith(allowedBar)))
                                                    || Enumerable.Range(1, 12).Contains(t.View_Period)).ToList();
                        }

                        if (fundByPeriod.Any())
                        {
                            barReportItems.Add(new BarAnnualReportItem
                            {
                                BarNumber = targetBarMapping.BarNumber,
                                BarDbSource = targetBarMapping.DbSource,
                                Amount = GetDistBarTotalAmount(fundByPeriod, targetBarMapping, targetViewBar),
                                Rows = fundByPeriod,
                                BarDisplayName = targetBarMapping.DisplayName
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
                BarDisplayName = gValue.FirstOrDefault()?.BarDisplayName,
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

        private List<Bar> GetGcTargetBarMappings(List<Bar> dbBars, string bar)
        {
            return dbBars.Where(t => (t.DbSource.HasValue && t.DbSource.Value == DbSource.GC)
                    && t.MapToBarNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim())
                    .Any(item => bar.StartsWith(item)))
                .ToList();
        }

        public List<Bar> GetDistTargetBarMappings(List<Bar> dbBars, string bar)
        {
            return dbBars.Where(t => (t.DbSource.HasValue && t.DbSource.Value == DbSource.DIST)
                    && t.MapToBarNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim())
                    .Any(item => bar.StartsWith(item)))
                .ToList();
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