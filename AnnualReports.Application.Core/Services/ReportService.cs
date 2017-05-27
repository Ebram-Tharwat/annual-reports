using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.Contracts;
using AnnualReports.Infrastructure.Core.Interfaces;
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

        public List<FundsReportDataItemDetails> GetFundsReportData(int year, int? fundId = null, string barNumber = null)
        {
            var reportData = new List<FundsReportDataItemDetails>();
            // 1- get all possible combination of funds*bars
            var fundsReportData = _fundsRepository.GetFundsReportDataRows(year, fundId);
            // 2- get all possible/valid bars
            var bars = _barService.GetAllBars(year);
            // 3- generate report item detail.
            var groupByFundNumber = fundsReportData.GroupBy(t => new { t.PrimaryFundNumber, t.FundDisplayName, t.MCAG });
            foreach (var fundGroup in groupByFundNumber)
            {
                foreach (var bar in bars)
                {
                    List<FundsReportDataRow> fundRows;
                    if (bar.Period.HasValue)
                        fundRows = fundGroup.Where(t => t.View_BarNumber.StartsWith(bar.BarNumber) && t.View_Period == bar.Period.Value).ToList();
                    else
                        fundRows = fundGroup.Where(t => t.View_BarNumber.StartsWith(bar.BarNumber)).ToList();

                    if (fundRows.Count > 0)
                    {
                        decimal total;
                        var barNumberToValidate = (string.IsNullOrEmpty(bar.MapToBarNumber)) ? bar.BarNumber : bar.MapToBarNumber;
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
                            Amount = total
                        });
                    }
                }
            }
            return reportData;
        }
    }
}