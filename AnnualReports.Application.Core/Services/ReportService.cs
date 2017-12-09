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
            var mappingRules = _mappingRuleRepository.Get(t => t.Year == year).ToList();
            // 4- generate report item detail.
            var groupByFundNumber = annualReportData.GroupBy(t => new { t.PrimaryFundNumber, t.FundDisplayName, t.MCAG, t.DbSource });
            foreach (var fundGroup in groupByFundNumber)
            {
                // 4.1 get mapping rules for the current fund, and ONLY when it's a DIST fund
                var currentFundMappingRules = mappingRules.Where(t => t.TargetFundNumber == fundGroup.Key.PrimaryFundNumber && fundGroup.Key.DbSource == DbSource.DIST).ToList();
                foreach (var bar in viewBars)
                {
                    var targetBar = dbBars.FirstOrDefault(t => t.BarNumber == bar);
                    if(targetBar == null)
                    {
                        targetBar = new Bar() { BarNumber = bar, MapToBarNumber = bar, Year = (short)year, DisplayName = "" };
                    }
                    IEnumerable<AnnualReportDataRow> fundRows = new List<AnnualReportDataRow>();
                    if (targetBar.Period.HasValue)
                        fundRows = fundGroup.Where(t => t.View_Period == targetBar.Period.Value);
                    else
                        fundRows = fundGroup;

                    var mapToBarList = GetBarNumberMappedItems(targetBar, currentFundMappingRules);
                    fundRows = fundRows.Where(t => mapToBarList.Any(mapToItem => t.View_BarNumber.StartsWith(mapToItem.CreditsMappedBarNumber)
                        || t.View_BarNumber.StartsWith(mapToItem.DebitsMappedBarNumber))).ToList();
                    if (fundRows.Any())
                    {
                        decimal total = 0;
                        foreach (var mapToItem in mapToBarList)
                        {
                            total += GetMapToBarItemAmount(fundRows, mapToItem);
                        }
                        reportData.Add(new AnnualReportDataItemDetails()
                        {
                            FundNumber = fundGroup.Key.PrimaryFundNumber,
                            FundDisplayName = fundGroup.Key.FundDisplayName,
                            BarNumber = targetBar.BarNumber,
                            BarDisplayName = targetBar.DisplayName,
                            MapToBarNumber = targetBar.MapToBarNumber,
                            Year = year,
                            MCAG = fundGroup.Key.MCAG,
                            Rows = fundRows.ToList(),
                            Amount = total
                        });
                    }
                }
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

        private List<BarMappingRuleItem> GetBarNumberMappedItems(Bar bar, List<MappingRule> mappingRules)
        {
            var mapToBarList = new List<BarMappingRuleItem>();
            MappingRule mappingRule = null;

            // 1- check if there is "Equal" rule for the specified bar.
            mappingRule = mappingRules.FirstOrDefault(t => t.Operator == MappingRuleOperator.Equal && bar.BarNumber == t.TargetBarNumber);
            if (mappingRule != null)
            {
                mapToBarList.Add(new BarMappingRuleItem(bar.BarNumber, mappingRule));
                return mapToBarList;
            }

            // 2- if not found, then check if there is "StartWith" rule for the specified bar.
            mappingRule = mappingRules.FirstOrDefault(t => t.Operator == MappingRuleOperator.StartWith && bar.BarNumber.StartsWith(t.TargetBarNumber));
            if (mappingRule != null)
            {
                mapToBarList.Add(new BarMappingRuleItem(bar.BarNumber, mappingRule));
                return mapToBarList;
            }

            // 3- if not found, then check if there is "EndWith" rule for the specified bar.
            mappingRule = mappingRules.FirstOrDefault(t => t.Operator == MappingRuleOperator.EndWith && bar.BarNumber.EndsWith(t.TargetBarNumber));
            if (mappingRule != null)
            {
                mapToBarList.Add(new BarMappingRuleItem(bar.BarNumber, mappingRule));
                return mapToBarList;
            }

            // 4- if not found, then use the "MapToBarNumber" column
            if (string.IsNullOrWhiteSpace(bar.MapToBarNumber))
            {
                mapToBarList.Add(new BarMappingRuleItem(bar.BarNumber, bar.BarNumber));
            }
            else
            {
                mapToBarList = bar.MapToBarNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => new BarMappingRuleItem(bar.BarNumber, item)).ToList();
            }

            return mapToBarList;
        }

        private decimal GetMapToBarItemAmount(IEnumerable<AnnualReportDataRow> fundRows, BarMappingRuleItem mapToItem)
        {
            if (mapToItem.IsCustomMapping)
            {
                decimal debitsAmount = fundRows.Where(t => t.View_BarNumber.StartsWith(mapToItem.DebitsMappedBarNumber)).Sum(t => t.Debit);
                decimal creditsAmount = fundRows.Where(t => t.View_BarNumber.StartsWith(mapToItem.CreditsMappedBarNumber)).Sum(t => t.Credit);
                return debitsAmount - creditsAmount;
            }
            else
            {
                if (mapToItem.TargetBarNumber.StartsWith("5") || mapToItem.TargetBarNumber.StartsWith("1"))
                    return fundRows.Where(t => t.View_BarNumber.StartsWith(mapToItem.TargetBarNumber)).Sum(t => t.Debit - t.Credit);
                else
                    return fundRows.Where(t => t.View_BarNumber.StartsWith(mapToItem.TargetBarNumber)).Sum(t => t.Credit - t.Debit);
            }
        }
    }
}