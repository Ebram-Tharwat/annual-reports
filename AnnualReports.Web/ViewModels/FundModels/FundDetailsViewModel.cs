using AnnualReports.Domain.Core.AnnualReportsDbModels;

namespace AnnualReports.Web.ViewModels.FundModels
{
    public class FundDetailsViewModel
    {
        public int Id { get; set; }

        public string FundNumber { get; set; }

        public string DisplayName { get; set; }

        public int Year { get; set; }

        public string MCAG { get; set; }

        public DbSource DbSource { get; set; }

        public bool IsActive { get; set; }

        public string MapTo { get; set; }
    }
}