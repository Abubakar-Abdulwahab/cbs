using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HelperModels
{
    public class SettlementFrequencyModel
    {
        public FrequencyType FrequencyType { get; set; }

        public DurationModel Duration { get; set; }

        public FixedBillingModel FixedBill { get; set; }

        public bool EoDSettlement { get; set; }
    }
}