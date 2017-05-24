using System.Collections.Generic;

namespace AnnualReports.Application.Core.Contracts.FundEntities
{
    public class FundBasicInfo
    {
        public int Id { get; set; }

        public string FundNumber { get; set; }

        public string DisplayName { get; set; }

        public List<FundBasicInfo> ChildFunds { get; set; }
    }
}