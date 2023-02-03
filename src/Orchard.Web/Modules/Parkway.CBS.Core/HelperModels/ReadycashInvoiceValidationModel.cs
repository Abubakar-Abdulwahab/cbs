using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReadycashInvoiceValidationModel
    {
        [JsonProperty("custid")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }
    }    
}