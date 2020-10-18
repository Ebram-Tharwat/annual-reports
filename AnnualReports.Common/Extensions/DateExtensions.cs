using System;

namespace AnnualReports.Common.Extensions
{
    public static class DateExtensions
    {
        public static string GetTimeStamp(this DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }

        public static string ToMonthYear(this DateTime value, string format = "MM/yyyy")
        {
            return value.ToString(format);
        }
    }
}