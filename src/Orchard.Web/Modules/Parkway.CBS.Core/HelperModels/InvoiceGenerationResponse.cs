using Newtonsoft.Json;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceGenerationResponse : BaseInvoiceGenerationResponse
    {
        public Int64 CustomerPrimaryContactId { get; set; }

        public Int64 CustomerId { get; set; }

        public string Recipient { get; set; }

        public string PayerId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string TIN { get; set; }

        public string InvoiceTitle { get; set; }

        public string InvoiceDescription { get; set; }

        public string ExternalRefNumber { get; set; }

        public bool ShowRemitta { get; set; }

        public string PaymentURL { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public string CallBackURL { get; set; }

        [JsonIgnore]
        public HeaderObj HeaderObj { get; set; }

        [JsonIgnore]
        public string TenantName { get; set; }

        public string Message { get; set; }

        //[JsonIgnore]
        public InvoiceStatus InvoiceStatus { get; set; }

        [JsonIgnore]
        public int StatusValue { get; set; }

        /// <summary>
        /// returns true if the request reference is a duplicate
        /// </summary>
        public bool IsDuplicateRequestReference { get; set; }

        [JsonIgnore]
        public Int64 InvoiceId { get; set; }

        public List<InvoiceItemsSummary> InvoiceItemsSummaries { get; set; }
    }
}