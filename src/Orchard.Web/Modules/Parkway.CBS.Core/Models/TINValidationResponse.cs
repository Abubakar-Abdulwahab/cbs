using System;
using Newtonsoft.Json;

namespace Parkway.CBS.Core.Models
{
    public class TINValidationResponse : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        [JsonProperty("TIN")]
        public virtual string TIN { get; set; }

        [JsonProperty("JTBTIN")]
        public virtual string JTBTIN { get; set; }

        [JsonProperty("TaxPayerName")]
        public virtual string TaxPayerName { get; set; }

        [JsonProperty("Address")]
        public virtual string Address { get; set; }

        [JsonProperty("TaxOfficeID")]
        public virtual string TaxOfficerId { get; set; }

        [JsonProperty("TaxOfficeName")]
        public virtual string TaxOfficeName { get; set; }

        [JsonProperty("TaxPayerType")]
        public virtual string TaxPayerType { get; set; }

        [JsonProperty("RCNumber")]
        public virtual string RCNumber { get; set; }

        [JsonProperty("Email")]
        public virtual string Email { get; set; }

        [JsonProperty("Phone")]
        public virtual string Phone { get; set; }

        public virtual string ResponseDump { get; set; }
    }
}