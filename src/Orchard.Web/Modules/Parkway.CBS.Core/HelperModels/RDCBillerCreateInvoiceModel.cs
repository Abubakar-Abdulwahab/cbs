using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class RDCBillerCreateInvoiceModel
    {
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("taxPayerCode")]
        public string TaxPayerCode { get; set; }

        [JsonProperty("revenueHeadId")]
        public int RevenueHeadId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("revenueHeadCode")]
        public string RevenueHeadCode { get; set; }
    }
}