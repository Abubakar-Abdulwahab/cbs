using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReadycashInvoiceValidationResponseModel
    {
        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("responseCode")]
        public string ResponseCode { get; set; }

        [JsonProperty("responseDesc")]
        public string ResponseDescription { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}