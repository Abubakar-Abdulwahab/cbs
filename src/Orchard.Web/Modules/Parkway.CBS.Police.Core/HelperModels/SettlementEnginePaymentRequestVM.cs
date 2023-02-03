using Newtonsoft.Json;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEnginePaymentRequestVM
    {
        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("sourceAccountNumber")]
        public string SourceAccountNumber { get; set; }

        [JsonProperty("sourceBankCode")]
        public string SourceBankCode { get; set; }

        [JsonProperty("callbackURL")]
        public string CallbackUrl { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }

        [JsonProperty("items")]
        public List<SettlementEnginePaymentItem> Items { get; set; }
    }
}
