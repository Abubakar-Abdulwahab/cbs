using System;
namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceGeneratedResponseLite : InvoiceGeneratedResponse
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

        public string ExternalRefNumber { get; set; }

        public string PaymentURL { get; set; }

        public string Description { get; set; }

        public string RequestReference { get; set; }

        /// <summary>
        /// returns true if the request reference was used
        /// </summary>
        public bool IsDuplicateRequestReference { get; set; }

    }
}