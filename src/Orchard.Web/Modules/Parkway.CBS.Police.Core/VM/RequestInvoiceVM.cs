using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class RequestInvoiceVM
    {
        public Int64 Id { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceUrl { get; set; }

        public string ServiceName { get; set; }

        public string FileRefNumber { get; set; }

        public decimal Amount { get; set; }

        public decimal AmountDue { get; set; }

        public int Status { get; set; }
    }
}