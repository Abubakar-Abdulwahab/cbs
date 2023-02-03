using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentOptions
    {
        public InvoiceStatus PaymentStatus { get; set; }
        public string TINText { get; set; }
        public string InvoiceNumber { get; set; }
    }
}