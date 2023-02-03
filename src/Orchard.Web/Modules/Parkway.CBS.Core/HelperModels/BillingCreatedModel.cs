using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class BillingCreatedModel
    {
        public int RevenueHeadId { get; set; }
        public string InvoicingServiceProductCode { get; set; }
        public string NextRunDate { get; set; }
    }
}