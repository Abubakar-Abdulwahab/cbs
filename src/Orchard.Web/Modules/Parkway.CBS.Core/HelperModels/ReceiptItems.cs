using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReceiptItems
    {
        public string PayerName { get; set; }

        public string AnnualEarnings { get; set; }

        public string Exemptions { get; set; }

        public string TaxValue { get; set; }

        public Int64 PayerId { get; set; }

        public bool PaymentStatus { get; set; }

        public string ReceiptNumber { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }
    }
}