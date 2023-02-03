using Newtonsoft.Json;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEngineAccountVerifyRequestVM
    {
        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("BankCode")]
        public string BankCode { get; set; }
    }
}
