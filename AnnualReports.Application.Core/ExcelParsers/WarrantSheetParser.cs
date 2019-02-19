using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Common.Utils;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelParsers
{
    public static class WarrantSheetParser
    {
        public static IEnumerable<WarrantReportInputItem> Parse(Stream inputStream)
        {
            const int WarrantSheetIndex = 1;
            var columnsToParse = new[] { "FundID", "Name", "Issues", "Presented", "Cancels" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, WarrantSheetIndex, columnsToParse);
            return sheetData.AsEnumerable().Select(row =>
            {
                return new WarrantReportInputItem()
                {
                    FundId = row["FundID"].ToString(),
                    Name = row["Name"].ToString(),
                    Issues = ParseNegativeValue(row["Issues"].ToString()),
                    Presented = ParseNegativeValue(row["Presented"].ToString()),
                    Cancels = ParseNegativeValue(row["Cancels"].ToString()),
                };
            }).ToList();
        }

        private static decimal ParseNegativeValue(string input)
        {
            if (input.StartsWith("("))
            {
                input = input.Replace("(", "").Replace(")", "");
                return decimal.Parse(input) * -1;
            }
            else return decimal.Parse(input);
        }
    }
}