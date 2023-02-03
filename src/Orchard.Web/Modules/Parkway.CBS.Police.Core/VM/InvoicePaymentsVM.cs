using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class InvoicePaymentsVM
    {
        public Int64 TransactionLogId { get; set; }

        public decimal AmountPaid { get; set; }

        public string PaymentReference { get; set; }

        public string ReceiptNumber { get; set; }

        public int TypeID { get; set; }

        public string PaymentDate { get; set; }

        public string InvoiceDesc { get; set; }
    }
}