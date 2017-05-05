using AnnualReports.Domain.Core.AnnualReportsDbModels;

namespace AnnualReports.Domain.Core.Contracts
{
    public class GPDynamicsFundDetails
    {
        public string Number { get; set; }

        public string Description { get; set; }

        public DbSource DbSource { get; set; }
    }
}