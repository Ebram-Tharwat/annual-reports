using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Common.Utils;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelParsers.AuditorMaster
{
    public static class InvestmentsSheetParser
    {
        public static IEnumerable<InvestmentsSheetInputItem> Parse(Stream inputStream, int sheetIndex)
        {
            var columnsToParse = new[] { "Name", "Fund", "Purchases", "Sales", "Interest" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            return sheetData.AsEnumerable().Select(row =>
            {
                return new InvestmentsSheetInputItem()
                {
                    Name = row["Name"].ToString(),
                    FundId = row["Fund"].ToString(),
                    Purchases = StringUtils.ParseNegativeValue(row["Purchases"].ToString()),
                    Sales = StringUtils.ParseNegativeValue(row["Sales"].ToString()),
                    Interest = StringUtils.ParseNegativeValue(row["Interest"].ToString()),
                };
            }).ToList();
        }
    }
}