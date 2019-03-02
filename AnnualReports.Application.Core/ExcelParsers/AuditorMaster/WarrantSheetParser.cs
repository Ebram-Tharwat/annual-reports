using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelParsers.AuditorMaster
{
    public static class WarrantSheetParser
    {
        public static IEnumerable<WarrantsSheetInputItem> Parse(Stream inputStream, int sheetIndex)
        {
            var results = new List<WarrantsSheetInputItem>();
            var columnsToParse = new[] { "FundID", "Name", "Issues", "Presented", "Cancels" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            sheetData.AsEnumerable().ForEachWithIndex((row, index) =>
            {
                results.Add(new WarrantsSheetInputItem()
                {
                    RowIndex = index + 2, // 2 => one for table header and one for zero-indexed loop
                    FundId = row["FundID"].ToString(),
                    Name = row["Name"].ToString(),
                    Issues = StringUtils.ParseNegativeValue(row["Issues"].ToString()),
                    Presented = StringUtils.ParseNegativeValue(row["Presented"].ToString()),
                    Cancels = StringUtils.ParseNegativeValue(row["Cancels"].ToString()),
                });
            });

            return results;
        }
    }
}