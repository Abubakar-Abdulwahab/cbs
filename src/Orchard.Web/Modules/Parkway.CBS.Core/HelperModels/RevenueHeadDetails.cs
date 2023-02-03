using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class RevenueHeadDetails
    {
        public RevenueHead RevenueHead { get; set; }

        public MDA Mda { get; set; }

        public BillingModel Billing { get; set; }

        /// <summary>
        /// if this field has a value redirect the user to the URL given
        /// this indicates that the generation on invoices should not happen on central billing
        /// </summary>
        public string InvoiceGenerationRedirectURL { get; set; }

        /// <summary>
        /// Set this value to true, if you want to redirect the user to the value in the InvoiceGenerationRedirectURL property
        /// </summary>
        public bool Redirect { get; set; }
        public string CallBackURL { get; set; }
    }


    public class RevenueHeadDetailsForInvoiceGeneration : RevenueHeadDetails
    {
        public ExpertSystemSettings ExpertSystem { get; set; }

        public TenantCBSSettings Tenant { get; set; }        
    }
}