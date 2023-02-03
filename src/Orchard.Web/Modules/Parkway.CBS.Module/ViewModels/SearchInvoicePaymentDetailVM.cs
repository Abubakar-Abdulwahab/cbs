using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class SearchInvoicePaymentVM
    {
        public string InvoiceNumber { get; set; }

        public string ErrorMessage { get; set; }

        public bool HasError { get; set; }
    }
}