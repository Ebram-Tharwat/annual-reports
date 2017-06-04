using AnnualReports.Application.Core.Contracts.Reports;
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
        private readonly IReportService _reportService;

        public ExcelExportingService(IFundService fundService, IBarService barService, IReportService reportService)
        {
            _fundService = fundService;
            _barService = barService;
            _reportService = reportService;
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

        public MemoryStream GetBarsTemplate(int year)
        {
            string excelTemplate = GetExcelTemplate(ReportType.BarsTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateBarsTemplate(package, _barService.GetAllBars(year), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        public MemoryStream GetAnnualReportExcel(int year)
        {
            string excelTemplate = GetExcelTemplate(ReportType.AnnualReportTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateAnnualReportTemplate(package, _reportService.GetAnnualReportData(year), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        #region Funds Template

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

        #region Bars Template

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
                dataSheet.Cells["C" + index].Value = bar.MapToBarNumber;
                dataSheet.Cells["D" + index].Value = bar.DisplayName;
                dataSheet.Cells["G" + index].Value = bar.IsActive;
                index++;
            }
        }

        #endregion Bars Template

        #region Annual Report

        private void GenerateAnnualReportTemplate(ExcelPackage excelPackage, IEnumerable<AnnualReportDataItemDetails> reportData, int year)
        {
            var dataSheet = excelPackage.Workbook.Worksheets[1];
            var index = 2; // starting index.

            foreach (var item in reportData)
            {
                dataSheet.Cells["A" + index].Value = year;
                dataSheet.Cells["B" + index].Value = item.MCAG;
                dataSheet.Cells["C" + index].Value = item.FundNumber;
                dataSheet.Cells["D" + index].Value = item.FundDisplayName;
                dataSheet.Cells["E" + index].Value = item.BarNumber;
                dataSheet.Cells["F" + index].Value = item.BarDisplayName;
                dataSheet.Cells["G" + index].Value = item.Amount;
                index++;
            }

            dataSheet.Cells.AutoFitColumns();
        }

        #endregion Annual Report

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

                case ReportType.AnnualReportTemplate:
                    templatePath = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\ExcelTemplates\\AnnualReportTemplate.xlsx";
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