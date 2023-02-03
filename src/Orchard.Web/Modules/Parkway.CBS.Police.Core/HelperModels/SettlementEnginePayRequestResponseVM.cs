using Newtonsoft.Json;


namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEnginePayRequestResponseVM
    {
        [JsonProperty("Error")]
        public bool Error { get; set; }

        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("ResponseObject")]
        public SettlementEnginePayRequestResponseObject ResponseObject { get; set; }
    }
}


