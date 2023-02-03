using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class SettlementRuleLite
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string RuleIdentifier { get; set; }

        public DateTime NextScheduleDate { get; set; }
    }
}