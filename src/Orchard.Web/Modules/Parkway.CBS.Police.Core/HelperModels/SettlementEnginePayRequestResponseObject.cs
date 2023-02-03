using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Parkway.CBS.Police.Core.HelperModels
{
    public class SettlementEnginePayRequestResponseObject
    {
        [JsonProperty("BatchReference")]
        public string BatchReference { get; set; }

        [JsonProperty("ReferenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("PaymentType")]
        public string PaymentType { get; set; }

        [JsonProperty("DebitMode")]
        public string DebitMode { get; set; }

        [JsonProperty("FromAccountNumber")]
        public string FromAccountNumber { get; set; }

        [JsonProperty("FromBankCode")]
        public string FromBankCode { get; set; }

        [JsonProperty("Narration")]
        public string Narration { get; set; }

        [JsonProperty("CorporateId")]
        public long CorporateId { get; set; }

        [JsonProperty("ProcessDate")]
        public DateTimeOffset ProcessDate { get; set; }

        [JsonProperty("Parties")]
        public List<SettlementEnginePayRequestParty> Parties { get; set; }
    }
}


