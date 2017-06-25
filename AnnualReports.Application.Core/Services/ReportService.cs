using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.Interfaces;
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

        public ReportService(IAnnualReportsDbFundRepository fundsRepository, IBarService barService)
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
            var bars = _barService.GetAllBars(year);
            // 3- generate report item detail.
            var groupByFundNumber = annualReportData.GroupBy(t => new { t.PrimaryFundNumber, t.FundDisplayName, t.MCAG });
            foreach (var fundGroup in groupByFundNumber)
            {
                foreach (var bar in bars)
                {
                    IEnumerable<AnnualReportDataRow> fundRows = new List<AnnualReportDataRow>();
                    if (bar.Period.HasValue)
                        fundRows = fundGroup.Where(t => t.View_Period == bar.Period.Value);
                    else
                        fundRows = fundGroup;

                    var mapToBarList = new List<string>();
                    if (string.IsNullOrWhiteSpace(bar.MapToBarNumber))
                        mapToBarList.Add(bar.BarNumber);
                    else
                        mapToBarList = bar.MapToBarNumber.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    fundRows = fundRows.Where(t => mapToBarList.Any(mapToItem => t.View_BarNumber.StartsWith(mapToItem))).ToList();
                    if (fundRows.Any())
                    {
                        decimal total = 0;
                        foreach (var mapToItem in mapToBarList)
                        {
                            if (mapToItem.StartsWith("5") || mapToItem.StartsWith("1"))
                                total += fundRows.Sum(t => t.Debit - t.Credit);
                            else
                                total += fundRows.Sum(t => t.Credit - t.Debit);
                        }
                        reportData.Add(new AnnualReportDataItemDetails()
                        {
                            FundNumber = fundGroup.Key.PrimaryFundNumber,
                            FundDisplayName = fundGroup.Key.FundDisplayName,
                            BarNumber = bar.BarNumber,
                            BarDisplayName = bar.DisplayName,
                            MapToBarNumber = bar.MapToBarNumber,
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
    }
}