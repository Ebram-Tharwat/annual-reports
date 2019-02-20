using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Common.Utils;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelParsers.AuditorMaster
{
    public static class WarrantSheetParser
    {
        public static IEnumerable<WarrantsSheetInputItem> Parse(Stream inputStream)
        {
            const int WarrantSheetIndex = 1;
            var columnsToParse = new[] { "FundID", "Name", "Issues", "Presented", "Cancels" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, WarrantSheetIndex, columnsToParse);
            return sheetData.AsEnumerable().Select(row =>
            {
                return new WarrantsSheetInputItem()
                {
                    FundId = row["FundID"].ToString(),
                    Name = row["Name"].ToString(),
                    Issues = StringUtils.ParseNegativeValue(row["Issues"].ToString()),
                    Presented = StringUtils.ParseNegativeValue(row["Presented"].ToString()),
                    Cancels = StringUtils.ParseNegativeValue(row["Cancels"].ToString()),
                };
            }).ToList();
        }
    }
}