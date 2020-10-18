using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System;
using System.Collections.Generic;

namespace AnnualReports.Common.Utils
{
    public static class StringUtils
    {
        public static decimal ParseNegativeValue(string input)
        {
            if (input.StartsWith("("))
            {
                input = input.Replace("(", "").Replace(")", "");
                return decimal.Parse(input) * -1;
            }
            else return decimal.Parse(input);
        }

        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static Tuple<bool, string> ApplyMonthlyImportExceptionRuleOnFund(string fundId, List<MonthlyImportFundExceptionRule> monthlyImportFundExceptionRules)
        {
            string result = fundId.Trim();
            for (int i = 0; i < monthlyImportFundExceptionRules.Count; i++)
            {
                if (result.Contains(monthlyImportFundExceptionRules[i].OriginalFundId.Trim()))
                {
                    result = result.Replace(monthlyImportFundExceptionRules[i].OriginalFundId.Trim(), monthlyImportFundExceptionRules[i].NewFundId.Trim());
                    return new Tuple<bool, string>(true, result);
                }
            }
            return new Tuple<bool, string>(false, result);
        }
    }
}