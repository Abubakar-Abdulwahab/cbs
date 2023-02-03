using System.Collections.Generic;

namespace Parkway.CBS.Payee.Models
{
    public class SettlementParty
    {
        public decimal Cap { get; set; }

        public decimal Percentage { get; set; }

        public string Name { get; set; }

        public Dictionary<int, string> DetailRows { get; set; }
    }
}