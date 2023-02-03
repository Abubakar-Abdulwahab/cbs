using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReadyCashPaymentNotification
    {
        [JsonProperty("custid")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("ref")]
        public string PaymentReference { get; set; }

        /// <summary>
        /// yyyyMMddHHmmss
        /// </summary>
        [JsonProperty("date")]
        public string TransactionDate { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("tranId")]
        public string TranId { get; set; }

        [JsonProperty("mac_256")]
        public string Mac { get; set; }

        [JsonProperty("amt")]
        public decimal AmountPaid { get; set; }

        [JsonProperty("merchId")]
        public string MerchantId { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Request dump
        /// </summary>
        public string RequestDump { get; set; }
    }
}