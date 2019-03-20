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
    public static class TaxesSheetParser
    {
        public static IEnumerable<TaxesSheetInputItem> Parse(Stream inputStream, int sheetIndex,List<MonthlyImportFundExceptionRule> exceptionRules)
        {
            var results = new List<TaxesSheetInputItem>();
            var columnsToParse = new[] { "Fund", "Name", "Taxes" };
            var sheetData = ImportUtils.ImportXlsxToDataTable(inputStream, sheetIndex, columnsToParse);
            sheetData.AsEnumerable().ForEachWithIndex((row, index) =>
            {
                results.Add(new TaxesSheetInputItem()
                {
                    RowIndex = index + 2, // 2 => one for table header and one for zero-indexed loop
                    FundId = StringUtils.ApplyMonthlyImportExceptionRuleOnFund(row["Fund"].ToString(), exceptionRules),
                    Name = row["Name"].ToString(),
                    Taxes = StringUtils.ParseNegativeValue(row["Taxes"].ToString()),
                });
            });

            return results;
        }
    }
}