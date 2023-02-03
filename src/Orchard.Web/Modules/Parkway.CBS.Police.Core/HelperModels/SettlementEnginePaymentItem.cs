using Newtonsoft.Json;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEnginePaymentItem
    {
        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("bankCode")]
        public string BankCode { get; set; }

        [JsonProperty("itemRef")]
        public string ItemRef { get; set; }
    }
}
