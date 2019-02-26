using System;

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
    }
}