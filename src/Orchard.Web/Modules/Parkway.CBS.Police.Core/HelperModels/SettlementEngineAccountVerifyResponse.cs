using Newtonsoft.Json;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEngineAccountVerifyResponse
    {
        [JsonProperty("AccountName")]
        public string AccountName { get; set; }

        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("BankCode")]
        public string BankCode { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}


