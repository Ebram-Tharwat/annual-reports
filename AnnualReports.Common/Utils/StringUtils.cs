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
    }
}