﻿using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Application.Core.Enums;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Extensions;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AnnualReports.Application.Core.Contracts.Reports.JournalVoucherMatchingResultBuilder;

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
            string excelTemplate = GetExcelTemplate(ReportType.DistExceptionReportTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateDistExceptionReportTemplate(package, _reportService.GetDistExceptionReportData(year), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        public MemoryStream GetGcExceptionReportExcel(int year)
        {
            string excelTemplate = GetExcelTemplate(ReportType.GcExceptionReportTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateGcExceptionReportTemplate(package, _reportService.GetGcExceptionReportData(year), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        public MemoryStream GetJournalVoucherReportExcel(
            IEnumerable<JournalVoucherReportOutputItem> reportData,
            IEnumerable<JournalVoucherUnamtchedResult> unamtchedResults)
        {
            string excelTemplate = GetExcelTemplate(ReportType.JournalVoucherReportTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateJournalVoucherReportTemplate(package, reportData, unamtchedResults);

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

        private void GenerateDistExceptionReportTemplate(ExcelPackage excelPackage, IEnumerable<ExceptionReportDataItemDetails> reportData, int year)
        {
            var dataSheet = excelPackage.Workbook.Worksheets[1];
            var index = 2; // starting index.
            if (reportData != null)
            {
                foreach (var summeryItem in reportData)
                {
                    dataSheet.Cells["A" + index].Value = summeryItem.AccountIndex;
                    dataSheet.Cells["B" + index].Value = summeryItem.ActNum1;
                    dataSheet.Cells["C" + index].Value = summeryItem.ActNum2;
                    dataSheet.Cells["D" + index].Value = summeryItem.ActNum3;
                    dataSheet.Cells["E" + index].Value = summeryItem.ActNum4;
                    dataSheet.Cells["F" + index].Value = summeryItem.ActNum5;
                    dataSheet.Cells["G" + index].Value = summeryItem.ActType;
                    dataSheet.Cells["H" + index].Value = summeryItem.ActDesc;
                    index++;
                }
            }

            dataSheet.Cells.AutoFitColumns();
        }

        private void GenerateGcExceptionReportTemplate(ExcelPackage excelPackage, IEnumerable<ExceptionReportDataItemDetails> reportData, int year)
        {
            var dataSheet = excelPackage.Workbook.Worksheets[1];
            var index = 2; // starting index.
            if (reportData != null)
            {
                foreach (var summeryItem in reportData)
                {
                    dataSheet.Cells["A" + index].Value = summeryItem.AccountIndex;
                    dataSheet.Cells["B" + index].Value = summeryItem.ActNum1;
                    dataSheet.Cells["C" + index].Value = summeryItem.ActNum2;
                    dataSheet.Cells["D" + index].Value = summeryItem.ActNum3;
                    dataSheet.Cells["E" + index].Value = summeryItem.ActNum4;
                    dataSheet.Cells["F" + index].Value = summeryItem.ActNum5;
                    dataSheet.Cells["G" + index].Value = summeryItem.ActType;
                    dataSheet.Cells["H" + index].Value = summeryItem.ActDesc;
                    index++;
                }
            }

            dataSheet.Cells.AutoFitColumns();
        }

        #endregion Exception Report

        #region Annual Report

        private void GenerateAnnualReportTemplate(ExcelPackage excelPackage, IEnumerable<AnnualReportDataItemDetails> reportData, int year)
        {
            var distBars = _barService.GetAllBars(year, null, null, true, DbSource.DIST);
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
                summerySheet.Cells["G" + summeryIndex].Style.Font.Bold = true;
                summerySheet.Cells["G" + summeryIndex].Formula = $"=SUM(Details!$K{detailsIndex}:Details!$K{detailsIndex + summeryItem.Rows.Count - 1 })";
                summeryIndex++;

                foreach (var detailsItem in summeryItem.Rows)
                {
                    detailsSheet.Cells["A" + detailsIndex].Value = year;
                    detailsSheet.Cells["B" + detailsIndex].Value = detailsItem.AccountDescription;
                    detailsSheet.Cells["C" + detailsIndex].Value = summeryItem.BarNumber;

                    detailsSheet.Cells["D" + detailsIndex].Value = detailsItem.ACTNUMBR_1;
                    detailsSheet.Cells["E" + detailsIndex].Value = detailsItem.ACTNUMBR_2;
                    detailsSheet.Cells["F" + detailsIndex].Value = detailsItem.ACTNUMBR_3;
                    detailsSheet.Cells["G" + detailsIndex].Value = detailsItem.ACTNUMBR_4;
                    detailsSheet.Cells["H" + detailsIndex].Value = detailsItem.ACTNUMBR_5;

                    detailsSheet.Cells["I" + detailsIndex].Value = detailsItem.Debit;
                    detailsSheet.Cells["J" + detailsIndex].Value = detailsItem.Credit;
                    detailsSheet.Cells["K" + detailsIndex].Style.Font.Bold = true;
                    // ToDo: move the following logic to centralized service.
                    if (summeryItem.FundDbSource == DbSource.GC)
                    {
                        if (summeryItem.BarNumber == "3081000" || summeryItem.BarNumber.StartsWith("5") || summeryItem.BarNumber.StartsWith("1"))
                            detailsSheet.Cells["K" + detailsIndex].Formula = $"=I{detailsIndex}-J{detailsIndex}";
                        else
                            detailsSheet.Cells["K" + detailsIndex].Formula = $"=J{detailsIndex}-I{detailsIndex}";
                    }
                    else if (summeryItem.FundDbSource == DbSource.DIST)
                    {
                        var targetBarMapping = _reportService.GetDistTargetBarMappings(distBars, detailsItem.View_BarNumber)
                            .FirstOrDefault(t => t.BarNumber == summeryItem.BarNumber);
                        var mapToBarNumbers = targetBarMapping.MapToBarNumber.Split(new char[] { ',' }
                                        , StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim());

                        if (mapToBarNumbers.Any(item => item == "3") && detailsItem.View_BarNumber.StartsWith("3"))
                            detailsSheet.Cells["K" + detailsIndex].Formula = $"=J{detailsIndex}-I{detailsIndex}";
                        else if (mapToBarNumbers.Any(item => item == "5") && detailsItem.View_BarNumber.StartsWith("5"))
                            detailsSheet.Cells["K" + detailsIndex].Formula = $"=I{detailsIndex}-J{detailsIndex}";
                        else if (targetBarMapping.BarTarget == null)
                            detailsSheet.Cells["K" + detailsIndex].Formula = $"=I{detailsIndex}-J{detailsIndex}";
                        else
                        {
                            if (targetBarMapping.BarTarget == BarNumberTarget.Credit)
                                detailsSheet.Cells["K" + detailsIndex].Formula = $"=J{detailsIndex}";
                            else
                                detailsSheet.Cells["K" + detailsIndex].Formula = $"=I{detailsIndex}";
                        }
                    }
                    detailsIndex++;
                }
            }

            summerySheet.Cells.AutoFitColumns();
            detailsSheet.Cells.AutoFitColumns();
        }

        #endregion Annual Report

        #region JournalVoucher Report

        private void GenerateJournalVoucherReportTemplate(
            ExcelPackage excelPackage,
            IEnumerable<JournalVoucherReportOutputItem> reportData,
            IEnumerable<JournalVoucherUnamtchedResult> unamtchedResults)
        {
            var journalvoucherSheetForGC = excelPackage.Workbook.Worksheets[1];
            var journalvoucherSheetForDist = excelPackage.Workbook.Worksheets[2];
            var exceptionsSheet = excelPackage.Workbook.Worksheets[3];

            var gcFunds = reportData.Where(t => t.DbSource == DbSource.GC).ToList();
            var distFunds = reportData.Where(t => t.DbSource == DbSource.DIST).ToList();

            FillJournalVoucherSheet(journalvoucherSheetForGC, gcFunds);
            FillJournalVoucherSheet(journalvoucherSheetForDist, distFunds);
            FillJournalVoucherExceptionsSheet(exceptionsSheet, unamtchedResults);

            journalvoucherSheetForGC.Cells.AutoFitColumns();
            journalvoucherSheetForDist.Cells.AutoFitColumns();
            exceptionsSheet.Cells.AutoFitColumns();
        }

        private void FillJournalVoucherSheet(ExcelWorksheet dataSheet, IEnumerable<JournalVoucherReportOutputItem> reportData)
        {
            var index = 2; // starting index.
            foreach (var item in reportData)
            {
                dataSheet.Cells["A" + index].Value = item.AccountNumber;
                dataSheet.Cells["B" + index].Value = item.Description;
                dataSheet.Cells["C" + index].Value = item.Debit;
                dataSheet.Cells["D" + index].Value = item.Credit;
                dataSheet.Cells["E" + index].Value = item.JournalVoucher.GetDescriptionName();

                index++;
            }
        }

        private void FillJournalVoucherExceptionsSheet(ExcelWorksheet dataSheet, IEnumerable<JournalVoucherUnamtchedResult> unamtchedResults)
        {
            var index = 2; // starting index.
            foreach (var item in unamtchedResults)
            {
                dataSheet.Cells["A" + index].Value = item.RowIndex;
                dataSheet.Cells["B" + index].Value = item.SheetName;
                dataSheet.Cells["C" + index].Value = item.FundId;
                dataSheet.Cells["D" + index].Value = item.Database;
                dataSheet.Cells["E" + index].Value = item.JournalVoucher.GetDescriptionName();
                dataSheet.Cells["F" + index].Value = item.JournalVoucherRuleAccount;

                index++;
            }
        }

        #endregion JournalVoucher Report

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

                case ReportType.DistExceptionReportTemplate:
                    templatePath = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\ExcelTemplates\\DistExceptionReportTemplate.xlsx";
                    break;

                case ReportType.GcExceptionReportTemplate:
                    templatePath = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\ExcelTemplates\\GcExceptionReportTemplate.xlsx";
                    break;

                case ReportType.JournalVoucherReportTemplate:
                    templatePath = System.AppDomain.CurrentDomain.BaseDirectory + "Content\\ExcelTemplates\\JournalVoucherReportTemplate.xlsx";
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