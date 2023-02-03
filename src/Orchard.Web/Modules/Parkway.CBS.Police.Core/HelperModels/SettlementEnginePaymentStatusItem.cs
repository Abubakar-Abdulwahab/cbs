using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEnginePaymentStatusItem
    {
        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("StatusCode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentRequestStatus StatusCode { get; set; }

        [JsonProperty("StatusDescription")]
        public string StatusDescription { get; set; }
    }
}

