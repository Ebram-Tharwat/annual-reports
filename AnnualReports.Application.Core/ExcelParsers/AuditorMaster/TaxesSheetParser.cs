using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Common.Utils;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelParsers.AuditorMaster
{
    public static class TaxesSheetParser
    {
        public static IEnumerable<TaxesSheetInputItem> Parse(Stream inputStream, int sheetIndex)
        {
            var columnsToParse = new[] { "Fund", "Name", "Taxes" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            return sheetData.AsEnumerable().Select(row =>
            {
                return new TaxesSheetInputItem()
                {
                    FundId = row["Fund"].ToString(),
                    Name = row["Name"].ToString(),
                    Taxes = StringUtils.ParseNegativeValue(row["Taxes"].ToString()),
                };
            }).ToList();
        }
    }
}