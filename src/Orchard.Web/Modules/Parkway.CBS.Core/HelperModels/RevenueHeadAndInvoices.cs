using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class RevenueHeadAndInvoicesHelper
    {
        public string RevenueHeadName { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
    }
}