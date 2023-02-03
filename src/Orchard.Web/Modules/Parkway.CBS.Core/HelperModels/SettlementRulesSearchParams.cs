using Orchard.Users.Models;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class SettlementRulesSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public bool DontPageData { get; set; }

        public string Name { get; set; }

        public string RuleIdentifier { get; set; }

        public UserPartRecord Admin { get; set; }
    }
}