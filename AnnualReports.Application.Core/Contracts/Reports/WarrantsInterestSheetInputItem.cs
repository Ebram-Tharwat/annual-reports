using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class WarrantsInterestSheetInputItem
    {
        public string FundId { get; set; }
        public string Name { get; set; }
        public decimal WarrantInterest { get; set; }
    }
}
