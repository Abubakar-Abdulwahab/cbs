using System;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSSettlementBatchAggregateVM
    {
        public int RetryCount { get; set; }

        public string SettlementEngineRequestJSON { get; set; }
    }
}
