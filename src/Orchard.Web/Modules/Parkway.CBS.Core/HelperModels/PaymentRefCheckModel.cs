using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentRefCheckModel
    {
        public Int64 InvoiceId { get; set; }

        public Int64 TaxEntityId { get; set; }
    }
}