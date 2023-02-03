using System;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSSettlementRuleVM
    {
        public int PSSSettlementId { get; set; }

        public int SettlemntRuleId { get; set; }

        public string Name { get; set; }

        public string SettlementEngineRuleIdentifier { get; set; }

        public string CronExpression { get; set; }

        public DateTime NextScheduleDate { get; set; }

        public DateTime SettlementPeriodStartDate { get; set; }

        public DateTime SettlementPeriodEndDate { get; set; }

        public bool HasCommandSplits { get; set; }
    }
}
