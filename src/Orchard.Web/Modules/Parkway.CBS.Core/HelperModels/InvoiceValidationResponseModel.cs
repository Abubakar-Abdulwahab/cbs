using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceValidationModel
    {
        public string Recipient { get; set; }

        public decimal Amount { get; set; }

        public string InvoiceNumber { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PayerId { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseDescription { get; set; }

        public string SettlementCode { get; set; }

        public int SettlementType { get; set; }

        public string IssuerName { get; set; }

        public string ServiceId { get; set; }
    }

    public class InvoiceValidationResponseModel
    {
        public string PayerName { get; set; }

        public decimal Amount { get; set; }

        public string InvoiceNumber { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PayerId { get; set; }

        public Int64 InvoiceId { get; set; }

        public long ReceiptId { get; set; }

        public string ReceiptNumber { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseDescription { get; set; }

        public string SettlementCode { get; set; }

        public int SettlementType { get; set; }

        public string MDAName { get; set; }

        public string RevenueHead { get; set; }

    }

}