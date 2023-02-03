using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CollectionInvoiceGenerationResult
    {
        public decimal Amount { get; set; }

        public string PreviewPDFURL { get; set; }

        public string PreviewURL { get; set; }

        public string InvoiceNumber { get; set; }

        public string TaxIdentificationNumber { get; set; }

        public string Recipient { get; set; }

        public string PaymentURL { get; set; }
    }
}