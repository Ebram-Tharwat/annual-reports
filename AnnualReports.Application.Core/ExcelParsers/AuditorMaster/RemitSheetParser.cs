using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelParsers.AuditorMaster
{
    public static class RemitSheetParser
    {
        public static IEnumerable<RemitsSheetInputItem> Parse(Stream inputStream, int sheetIndex)
        {
            var results = new List<RemitsSheetInputItem>();
            var columnsToParse = new[] { "Fund", "Name", "Remits" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            sheetData.AsEnumerable().ForEachWithIndex((row, index) =>
            {
                results.Add(new RemitsSheetInputItem()
                {
                    RowIndex = index + 2, // 2 => one for table header and one for zero-indexed loop
                    FundId = row["Fund"].ToString(),
                    Name = row["Name"].ToString(),
                    Remits = StringUtils.ParseNegativeValue(row["Remits"].ToString()),
                });
            });

            return results;
        }
    }
}