using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentAcknowledgeMentModel
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public string NetPayTransactionRef { get; set; }

        public string PaymentRequestRef { get; set; }

        public string OrderId { get; set; }
        public decimal Amount { get; set; }

        public string Recepient { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string InvoiceAmount { get; set; }

        public string TIN { get; set; }

        public string MDAName { get; set; }

        public string RevenueHeadName { get; set; }

        public string ExternalRefNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string ReceiptNumber { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public string PayerId { get; set; }
    }
}