using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class CustomerWalletViewModel
    {
        public string Name { get; set; }
        public string TIN { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string MDA { get; set; }
        public string RevenueHead { get; set; }
        public string InvoiceNumber { get; set; }

        public DateTime Date { get; set; }
    }    
}