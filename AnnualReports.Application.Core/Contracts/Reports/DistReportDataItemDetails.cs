using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Application.Core.Contracts.Reports
{
    public class DistOrGcReportDataItemDetails
    {
        public int AccountIndex { get; set; }
        public string ActNum1 { get; set; }
        public string ActNum2 { get; set; }
        public string ActNum3 { get; set; }
        public string ActNum4 { get; set; }
        public string ActNum5 { get; set; }
        public short ActType { get; set; }
        public string ActDesc { get; set; }

    }
}
