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

        public ExcelExportingService(IFundService fundService)
        {
            _fundService = fundService;
        }

        public MemoryStream GetFundsTemplate(Int16 year)
        {
            string excelTemplate = GetExcelTemplate(ReportType.FundsTemplate);
            var templateFile = new FileInfo(excelTemplate);
            ExcelPackage package = new ExcelPackage(templateFile, true);

            GenerateFundsTemplate(package, _fundService.GetAllFunds(year, DbSource.ALL), year);

            var stream = new MemoryStream(package.GetAsByteArray());
            return stream;
        }

        #region Funds Template

        private void GenerateFundsTemplate(ExcelPackage excelPackage, IEnumerable<Fund> reportData, Int16 year)
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

        private void FillTemplateWithFunds(IEnumerable<Fund> funds, Int32 year, ExcelWorksheet dataSheet)
        {
            var index = 2; // starting index.
            foreach (var fund in funds)
            {
                dataSheet.Cells["A" + index].Value = year;
                dataSheet.Cells["B" + index].Value = fund.FundNumber;
                dataSheet.Cells["C" + index].Value = fund.GpDescription;
                dataSheet.Cells["D" + index].Value = fund.DisplayName;
                dataSheet.Cells["E" + index].Value = fund.MCAG;
                dataSheet.Cells["F" + index].Value = fund.MapToFundId;
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

                default:
                    templatePath = String.Empty;
                    break;
            }

            return templatePath;
        }

        #endregion Private Methods
    }
}