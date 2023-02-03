using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReadyCashPaymentNotificationResponseModel
    {
        [JsonProperty("responseCode")]
        public string ResponseCode { get; set; }

        [JsonProperty("responseDescription")]
        public string ResponseDescription { get; set; }

        [JsonProperty("payerName")]
        public string PayerName { get; set; }
    }
}