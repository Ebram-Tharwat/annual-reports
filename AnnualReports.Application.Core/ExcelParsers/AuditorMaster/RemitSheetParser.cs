using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Common.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Application.Core.ExcelParsers.AuditorMaster
{
    public static class RemitSheetParser
    {
        public static IEnumerable<RemitsSheetInputItem> Parse(Stream inputStream, int sheetIndex)
        {
            var columnsToParse = new[] { "Fund", "Name", "Remits" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            return sheetData.AsEnumerable().Select(row =>
            {
                return new RemitsSheetInputItem()
                {
                    FundId = row["Fund"].ToString(),
                    Name = row["Name"].ToString(),
                    Remits = StringUtils.ParseNegativeValue(row["Remits"].ToString()),
                };
            }).ToList();
        }
    }
}
