using Newtonsoft.Json;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceGeneratedResponseExtn : InvoiceGeneratedResponse
    {
        public Int64 CustomerPrimaryContactId { get; set; }

        public Int64 CustomerId { get; set; }

        public string Recipient { get; set; }

        public string PayerId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string TIN { get; set; }

        public string MDAName { get; set; }

        public string RevenueHeadName { get; set; }

        [JsonIgnore]
        public int RevenueHeadID { get; set; }

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

        //[JsonIgnore]
        public Int64 InvoiceId { get; set; }

        public string MerchantKey { get; set; }

        public string PaymentRequestRef { get; set; }

        public string GatewayFee { get; set; }

        public string NetPayMode { get; set; }

        public string NetPayColorCode { get; set; }

        public string NetPayCurrencyCode { get; set; }

        public string InvoiceTitle { get; set; }

        public string InvoiceDesc { get; set; }

        public List<TransactionLogGroup> Transactions { get; set; }

        public DateTime DueDate { get; set; }

        public bool HasPaymentProviderValidationConstraint { get; set; }

        public int MDAId { get; set; }

        public string MDASettlementCode { get; set; }

        public string RevenueHeadSettlementCode { get; set; }

        public int MDASettlementType { get; set; }

        public int RevenueHeadSettlementType { get; set; }

        public string RevenueHeadServiceId { get; set; }

        public FlashObj FlashObj { get; set; }
    }
}