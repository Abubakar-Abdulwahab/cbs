using Newtonsoft.Json;


namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEnginePayRequestParty
    {
        [JsonProperty("ParticipantName")]
        public string ParticipantName { get; set; }

        [JsonProperty("ParticipantReference")]
        public string ParticipantReference { get; set; }

        [JsonProperty("Reference")]
        public string Reference { get; set; }

        [JsonProperty("Amount")]
        public long Amount { get; set; }

        [JsonProperty("Narration")]
        public string Narration { get; set; }

        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("BankCode")]
        public string BankCode { get; set; }
    }
}


