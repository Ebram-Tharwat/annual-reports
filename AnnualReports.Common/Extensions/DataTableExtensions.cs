using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Common.Extensions
{
    public static class DataTableExtensions
    {
        public static bool AreAllColumnsNullOrEmpty(this DataRow row)
        {
            if (row != null)
            {
                foreach (var value in row.ItemArray)
                {
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
