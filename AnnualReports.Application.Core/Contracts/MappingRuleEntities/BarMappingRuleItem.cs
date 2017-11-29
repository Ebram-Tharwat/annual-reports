using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Collections.Generic;

namespace AnnualReports.Application.Core.Contracts.MappingRuleEntities
{
    public class BarMappingRuleItem
    {
        public BarMappingRuleItem()
        {
        }

        public BarMappingRuleItem(string targetBarNumber, MappingRule mappingRule)
        {
            TargetBarNumber = targetBarNumber;
            CreditsMappedBarNumber = mappingRule.CreditBarNumber;
            DebitsMappedBarNumber = mappingRule.DebitBarNumber;
            IsCustomMapping = true;
        }

        public BarMappingRuleItem(string targetBarNumber, string mapToBarNumber)
        {
            TargetBarNumber = targetBarNumber;
            CreditsMappedBarNumber = DebitsMappedBarNumber = mapToBarNumber;
        }

        public string TargetBarNumber { get; set; }

        public string CreditsMappedBarNumber { get; set; }

        public string DebitsMappedBarNumber { get; set; }

        public bool IsCustomMapping { get; set; }

        public IEnumerable<string> MappedToList
        {
            get
            {
                yield return this.CreditsMappedBarNumber;
                yield return this.DebitsMappedBarNumber;
            }
        }
    }
}