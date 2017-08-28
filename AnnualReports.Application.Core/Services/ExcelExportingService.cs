﻿using AnnualReports.Application.Core.Contracts.Reports;
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

        public MemoryStream GetAnnualReportExcel(int year, int? fundId)
        {
            string excelTemplate = GetExcelTemplate(ReportType.AnnualReportTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateAnnualReportTemplate(package, _reportService.GetAnnualReportData(year, fundId), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        public MemoryStream GetDistExceptionReportExcel(int year)
        {
            string excelTemplate = GetExcelTemplate(ReportType.DistTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateDistExceptionReportTemplate(package, _reportService.GetDistExceptionReportData(year), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        public MemoryStream GetGcExceptionReportExcel(int year)
        {
            string excelTemplate = GetExcelTemplate(ReportType.GcTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateGcExceptionReportTemplate(package, _reportService.GetGcExceptionReportData(year), year);

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

        #region Exception Report
        private void GenerateDistExceptionReportTemplate(ExcelPackage excelPackage, IEnumerable<DistOrGcReportDataItemDetails> reportData, int year)
        {
            var firstSheet = excelPackage.Workbook.Worksheets[1];
            var firstSheetIndex = 2; // starting index.
            if (reportData != null)
            {
                foreach (var summeryItem in reportData)
                {
                    firstSheet.Cells["A" + firstSheetIndex].Value = summeryItem.AccountIndex;
                    firstSheet.Cells["B" + firstSheetIndex].Value = summeryItem.ActNum1;
                    firstSheet.Cells["C" + firstSheetIndex].Value = summeryItem.ActNum2;
                    firstSheet.Cells["D" + firstSheetIndex].Value = summeryItem.ActNum3;
                    firstSheet.Cells["E" + firstSheetIndex].Value = summeryItem.ActNum4;
                    firstSheet.Cells["F" + firstSheetIndex].Value = summeryItem.ActNum5;
                    firstSheet.Cells["G" + firstSheetIndex].Value = summeryItem.ActType;
                    firstSheet.Cells["H" + firstSheetIndex].Value = summeryItem.ActDesc;
                    firstSheetIndex++;
                }
            }
        }
        private void GenerateGcExceptionReportTemplate(ExcelPackage excelPackage, IEnumerable<DistOrGcReportDataItemDetails> reportData, int year)
        {
            var firstSheet = excelPackage.Workbook.Worksheets[1];
            var firstSheetIndex = 2; // starting index.
            if (reportData != null)
            {
                foreach (var summeryItem in reportData)
                {
                    firstSheet.Cells["A" + firstSheetIndex].Value = summeryItem.AccountIndex;
                    firstSheet.Cells["B" + firstSheetIndex].Value = summeryItem.ActNum1;
                    firstSheet.Cells["C" + firstSheetIndex].Value = summeryItem.ActNum2;
                    firstSheet.Cells["D" + firstSheetIndex].Value = summeryItem.ActNum3;
                    firstSheet.Cells["E" + firstSheetIndex].Value = summeryItem.ActNum4;
                    firstSheet.Cells["F" + firstSheetIndex].Value = summeryItem.ActNum5;
                    firstSheet.Cells["G" + firstSheetIndex].Value = summeryItem.ActType;
                    firstSheet.Cells["H" + firstSheetIndex].Value = summeryItem.ActDesc;
                    firstSheetIndex++;
                }
            }
        }
        #endregion

        #region Annual Report

        private void GenerateAnnualReportTemplate(ExcelPackage excelPackage, IEnumerable<AnnualReportDataItemDetails> reportData, int year)
        {
            var summerySheet = excelPackage.Workbook.Worksheets[1];
            var detailsSheet = excelPackage.Workbook.Worksheets[2];
            var summeryIndex = 2; // starting index.
            var detailsIndex = 2; // starting index.

            foreach (var summeryItem in reportData)
            {
                summerySheet.Cells["A" + summeryIndex].Value = year;
                summerySheet.Cells["B" + summeryIndex].Value = summeryItem.MCAG;
                summerySheet.Cells["C" + summeryIndex].Value = summeryItem.FundNumber;
                summerySheet.Cells["D" + summeryIndex].Value = summeryItem.FundDisplayName;
                summerySheet.Cells["E" + summeryIndex].Value = summeryItem.BarNumber;
                summerySheet.Cells["F" + summeryIndex].Value = summeryItem.BarDisplayName;
                //summerySheet.Cells["H" + summeryIndex].Value = summeryItem.Amount;
                summerySheet.Cells["G" + summeryIndex].Style.Font.Bold = true;
                summerySheet.Cells["G" + summeryIndex].Formula = $"=SUM(Details!$J{detailsIndex}:Details!$J{detailsIndex + summeryItem.Rows.Count - 1 })";
                summeryIndex++;

                foreach (var detailsItem in summeryItem.Rows)
                {
                    detailsSheet.Cells["A" + detailsIndex].Value = year;
                    detailsSheet.Cells["B" + detailsIndex].Value = detailsItem.AccountDescription;
                    detailsSheet.Cells["C" + detailsIndex].Value = detailsItem.ACTNUMBR_1;
                    detailsSheet.Cells["D" + detailsIndex].Value = detailsItem.ACTNUMBR_2;
                    detailsSheet.Cells["E" + detailsIndex].Value = detailsItem.ACTNUMBR_3;
                    detailsSheet.Cells["F" + detailsIndex].Value = detailsItem.ACTNUMBR_4;
                    detailsSheet.Cells["G" + detailsIndex].Value = detailsItem.ACTNUMBR_5;
                    //detailsSheet.Cells["H" + detailsIndex].Value = detailsItem.View_Period;
                    detailsSheet.Cells["H" + detailsIndex].Value = detailsItem.Debit;
                    detailsSheet.Cells["I" + detailsIndex].Value = detailsItem.Credit;
                    detailsSheet.Cells["J" + detailsIndex].Style.Font.Bold = true;
                    if (summeryItem.MapToBarNumber.StartsWith("5") || summeryItem.MapToBarNumber.StartsWith("1"))
                        detailsSheet.Cells["J" + detailsIndex].Formula = $"=H{detailsIndex}-I{detailsIndex}";
                    else
                        detailsSheet.Cells["J" + detailsIndex].Formula = $"=I{detailsIndex}-H{detailsIndex}";
                    detailsIndex++;
                }
            }

            summerySheet.Cells.AutoFitColumns();
            detailsSheet.Cells.AutoFitColumns();
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
                case ReportType.DistTemplate:
                    templatePath = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\ExcelTemplates\\DistTemplate.xlsx";
                    break;
                case ReportType.GcTemplate:
                    templatePath = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\ExcelTemplates\\GcTemplate.xlsx";
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