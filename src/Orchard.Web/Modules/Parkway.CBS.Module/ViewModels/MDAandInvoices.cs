using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class MDAandInvoices
    {
        public string MDAName { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
        public string MDASlug { get; set; }
    }
}