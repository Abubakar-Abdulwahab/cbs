using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class SMSDetailVM
    {
        public string Name { get; set; }

        public string ApprovalNumber { get; set; }

        public string PhoneNumber { get; set; }

        public Int64 TaxEntityId { get; set; }

        public string PaymentLink { get; set; }

        public string InvoiceNumber { get; set; }

        public string RevenueHead { get; set; }

        public string Code { get; set; }

        public string Amount { get; set; }

        public string ReceiptNumber { get; set; }
    }
}