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
    public static class WarrantInterestSheetParser
    {
        public static IEnumerable<WarrantsInterestSheetInputItem> Parse(Stream inputStream, int sheetIndex, List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            var results = new List<WarrantsInterestSheetInputItem>();
            var columnsToParse = new[] { "Fund", "Name", "Warrant Int" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            sheetData.AsEnumerable().ForEachWithIndex((row, index) =>
            {
                var fundResult = StringUtils.ApplyMonthlyImportExceptionRuleOnFund(row["Fund"].ToString(), exceptionRules);
                results.Add(new WarrantsInterestSheetInputItem()
                {
                    RowIndex = index + 2, // 2 => one for table header and one for zero-indexed loop
                    FundId = fundResult.Item2,
                    IsExceptionRuleMatched = fundResult.Item1,
                    Name = row["Name"].ToString(),
                    WarrantInterest = StringUtils.ParseNegativeValue(row["Warrant Int"].ToString()),
                });
            });

            return results;
        }
    }
}