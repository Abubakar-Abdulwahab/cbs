using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSRequestInvoiceVM
    {
        public ICollection<RequestInvoiceVM> Invoices { get; set; }
        
        public HeaderObj HeaderObj { get; set; }
    }
}