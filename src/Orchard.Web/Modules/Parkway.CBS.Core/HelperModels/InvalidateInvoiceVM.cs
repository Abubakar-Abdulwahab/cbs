using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvalidateInvoiceVM
    {
        public string InvoiceNumber { get; set; }

        public bool IsInvalidated { get; set; }

    }
}