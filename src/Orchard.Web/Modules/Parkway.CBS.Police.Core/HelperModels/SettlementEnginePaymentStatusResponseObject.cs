using Newtonsoft.Json;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEnginePaymentStatusResponseObject
    {
        [JsonProperty("ReferenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("Hmac")]
        public string Hmac { get; set; }

        [JsonProperty("Items")]
        public List<SettlementEnginePaymentStatusItem> Items { get; set; }
    }
}

