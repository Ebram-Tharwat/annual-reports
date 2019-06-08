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
    public static class WarrantSheetParser
    {
        public static IEnumerable<WarrantsSheetInputItem> Parse(Stream inputStream, int sheetIndex,List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            var results = new List<WarrantsSheetInputItem>();
            var columnsToParse = new[] { "FundID", "Name", "Issues", "Presented", "Cancels" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            sheetData.AsEnumerable().ForEachWithIndex((row, index) =>
            {
                var fundResult = StringUtils.ApplyMonthlyImportExceptionRuleOnFund(row["FundID"].ToString(), exceptionRules);
                results.Add(new WarrantsSheetInputItem()
                {
                    RowIndex = index + 2, // 2 => one for table header and one for zero-indexed loop
                    FundId = fundResult.Item2,
                    IsExceptionRuleMatched = fundResult.Item1,
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