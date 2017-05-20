using AnnualReports.Application.Core.Enums;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.Services
{
    public class ExcelExportingService : IExportingService
    {
        private readonly IFundService _fundService;
        private readonly IBarService _barService;

        public ExcelExportingService(IFundService fundService, IBarService barService)
        {
            _fundService = fundService;
            _barService = barService;
        }

        public MemoryStream GetFundsTemplate(int year)
        {
            string excelTemplate = GetExcelTemplate(ReportType.FundsTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateFundsTemplate(package, _fundService.GetAllFunds(year, DbSource.ALL), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        #region Funds Template

        #region Bars Template

        public MemoryStream GetBarsTemplate(int year)
        {
            string excelTemplate = GetExcelTemplate(ReportType.BarsTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateBarsTemplate(package, _barService.GetAllBars(year), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        #endregion Bars Template

        private void GenerateFundsTemplate(ExcelPackage excelPackage, IEnumerable<Fund> reportData, int year)
        {
            var distDataSheet = excelPackage.Workbook.Worksheets[1];
            var gcDataSheet = excelPackage.Workbook.Worksheets[2];

            var gcFunds = reportData.Where(t => t.DbSource == DbSource.GC).ToList();
            var distFunds = reportData.Where(t => t.DbSource == DbSource.DIST).ToList();

            FillTemplateWithFunds(distFunds, year, distDataSheet);
            FillTemplateWithFunds(gcFunds, year, gcDataSheet);

            distDataSheet.Cells.AutoFitColumns();
            gcDataSheet.Cells.AutoFitColumns();
        }

        private void GenerateBarsTemplate(ExcelPackage excelPackage, IEnumerable<Bar> reportData, int year)
        {
            var annualReportDataSheet = excelPackage.Workbook.Worksheets[1];
            //var gcDataSheet = excelPackage.Workbook.Worksheets[2];

            FillTemplateWithBars(reportData, year, annualReportDataSheet);

            annualReportDataSheet.Cells.AutoFitColumns();
            //gcDataSheet.Cells.AutoFitColumns();
        }

        private void FillTemplateWithBars(IEnumerable<Bar> bars, int year, ExcelWorksheet dataSheet)
        {
            var index = 2; // starting index.
            foreach (var bar in bars)
            {
                dataSheet.Cells["A" + index].Value = year;
                dataSheet.Cells["B" + index].Value = bar.BarNumber;
                dataSheet.Cells["C" + index].Value = bar.MapToBarId;
                dataSheet.Cells["D" + index].Value = bar.DisplayName;
                dataSheet.Cells["G" + index].Value = bar.IsActive;
                index++;
            }
        }

        private void FillTemplateWithFunds(IEnumerable<Fund> funds, int year, ExcelWorksheet dataSheet)
        {
            var index = 2; // starting index.
            foreach (var fund in funds)
            {
                dataSheet.Cells["A" + index].Value = year;
                dataSheet.Cells["B" + index].Value = fund.FundNumber;
                dataSheet.Cells["C" + index].Value = fund.GpDescription;
                dataSheet.Cells["D" + index].Value = fund.DisplayName;
                dataSheet.Cells["E" + index].Value = fund.MCAG;
                dataSheet.Cells["F" + index].Value = fund.MapToFund?.FundNumber;
                dataSheet.Cells["G" + index].Value = fund.IsActive;
                index++;
            }
        }

        #endregion Funds Template

        #region Private Methods

        private string GetExcelTemplate(ReportType type)
        {
            string templatePath = String.Empty;

            switch (type)
            {
                case ReportType.FundsTemplate:
                    templatePath = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\ExcelTemplates\\FundsTemplate.xlsx";
                    break;

                case ReportType.BarsTemplate:
                    templatePath = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\ExcelTemplates\\BarsTemplate.xlsx";
                    break;

                default:
                    templatePath = String.Empty;
                    break;
            }

            return templatePath;
        }

        #endregion Private Methods
    }
}