using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HelperModels
{
    public class TransactionLogGroup
    {
        public string PaymentReference { get; set; }

        public string PaymentLogId { get; set; }

        public decimal TotalAmountPaid { get; set; }

        public string ThirdPartyReceiptNumber { get; set; }

        public Int64 InvoiceId { get; set; }

        public Int64 TaxEntityId { get; set; }

        public string ReceiptNumber { get; set; }

        public DateTime TransactionDate { get; set; }

        public string OriginalPaymentLogID { get; set; }

        public string OriginalPaymentReference { get; set; }

        public bool IsReversed { get; set; }

        public string RetrievalReferenceNumber { get; set; }
    }
}