using AnnualReports.Application.Core.Contracts.Reports;
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AnnualReports.Application.Core.ExcelParsers.AuditorMaster
{
    public static class RemitSheetParser
    {
        public static IEnumerable<RemitsSheetInputItem> Parse(Stream inputStream, int sheetIndex, List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            var results = new List<RemitsSheetInputItem>();
            var columnsToParse = new[] { "Fund", "Name", "Remits" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            sheetData.AsEnumerable().ForEachWithIndex((row, index) =>
            {
                var fundResult = StringUtils.ApplyMonthlyImportExceptionRuleOnFund(row["Fund"].ToString(), exceptionRules);
                results.Add(new RemitsSheetInputItem()
                {
                    RowIndex = index + 2, // 2 => one for table header and one for zero-indexed loop
                    FundId = fundResult.Item2,
                    IsExceptionRuleMatched = fundResult.Item1,
                    Name = row["Name"].ToString(),
                    Remits = StringUtils.ParseNegativeValue(row["Remits"].ToString()),
                });
            });

            return results;
        }
    }
}