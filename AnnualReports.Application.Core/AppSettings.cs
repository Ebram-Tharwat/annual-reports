using System.Configuration;

namespace AnnualReports.Application.Core
{
    public static class AppSettings
    {
        public static int PageSize
        {
            get { return int.Parse(ConfigurationManager.AppSettings["PageSize"]); }
        }
    }
}