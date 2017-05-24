using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Infrastructure.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class ReportService : IReportService
    {
        private IAnnualReportsDbFundRepository _fundsRepository;
        private IBarService _barService;

        public ReportService(IAnnualReportsDbFundRepository fundsRepository, IBarService barService)
        {
            this._fundsRepository = fundsRepository;
            this._barService = barService;
        }

        public List<FundsReportDataItemDetails> GetFundsReportData(int year, string fundNumber = null, string barNumber = null)
        {
            var reportData = new List<FundsReportDataItemDetails>();
            // 1- get all possible combination of funds*bars
            var fundsReportData = _fundsRepository.GetFundsReportDataRows(year, fundNumber);
            // 2- get all possible/valid bars
            var bars = _barService.GetAllBars(year);
            // 3- generate report item detail.
            var groupByFundNumber = fundsReportData.GroupBy(t => new { t.PrimaryFundNumber, t.FundDisplayName, t.MCAG });
            foreach (var fundGroup in groupByFundNumber)
            {
                foreach (var bar in bars)
                {                    
                    var fundRows = fundGroup.Where(t => t.View_BarNumber.StartsWith(bar.BarNumber)).ToList();
                    decimal total;
                    var barNumberToValidate = (bar.MapToBarId == null) ? bar.BarNumber : bar.MapToBar.BarNumber;
                    if (barNumberToValidate.StartsWith("5") || barNumberToValidate.StartsWith("1"))
                        total = fundRows.Sum(t => t.Debit - t.Credit);
                    else
                        total = fundRows.Sum(t => t.Credit - t.Debit);

                    reportData.Add(new FundsReportDataItemDetails()
                    {
                        FundNumber = fundGroup.Key.PrimaryFundNumber,
                        FundDisplayName = fundGroup.Key.FundDisplayName,
                        BarNumber = bar.BarNumber,
                        BarDisplayName = bar.DisplayName,
                        Year = year,
                        MCAG = fundGroup.Key.MCAG,
                        Rows = fundRows,
                        Total = total
                    });
                }
            }
            return reportData;
        }
    }
}