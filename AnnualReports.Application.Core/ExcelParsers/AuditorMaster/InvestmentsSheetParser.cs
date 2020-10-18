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
    public static class InvestmentsSheetParser
    {
        public static IEnumerable<InvestmentsSheetInputItem> Parse(Stream inputStream, int sheetIndex, List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            var results = new List<InvestmentsSheetInputItem>();
            var columnsToParse = new[] { "Name", "Fund", "Purchases", "Sales", "Interest" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            sheetData.AsEnumerable().ForEachWithIndex((row, index) =>
            {
                var fundResult = StringUtils.ApplyMonthlyImportExceptionRuleOnFund(row["Fund"].ToString(), exceptionRules);
                results.Add(new InvestmentsSheetInputItem()
                {
                    RowIndex = index + 2, // 2 => one for table header and one for zero-indexed loop
                    Name = row["Name"].ToString(),
                    FundId = fundResult.Item2,
                    IsExceptionRuleMatched = fundResult.Item1,
                    Purchases = StringUtils.ParseNegativeValue(row["Purchases"].ToString()),
                    Sales = StringUtils.ParseNegativeValue(row["Sales"].ToString()),
                    Interest = StringUtils.ParseNegativeValue(row["Interest"].ToString()),
                });
            });

            return results;
        }
    }
}