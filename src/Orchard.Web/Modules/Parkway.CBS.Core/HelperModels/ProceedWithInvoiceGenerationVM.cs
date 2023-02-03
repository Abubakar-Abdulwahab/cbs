using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class ProceedWithInvoiceGenerationVM : InvoiceGenerationDetailsModel
    {
        public TaxEntity Entity { get; set; }

        public bool FromTaxProfileSetup { get; set; }

        public string Message { get; set; }

        public bool HasMessage { get; set; }

        public ICollection<CollectionFormVM> AdditionalFormFields { get; set; }

        /// <summary>
        /// Set this value to true, if you want to redirect the user to the value in the InvoiceGenerationRedirectURL property
        /// </summary>
        public bool Redirect { get; set; }

        /// <summary>
        /// if this field has a value redirect the user to the URL given
        /// this indicates that the generation on invoices should not happen on central billing
        /// </summary>
        public string InvoiceGenerationRedirectURL { get; set; }

        public string MDANameAndCode { get; set; }

        public string RevenueHeadNameAndCode { get; set; }

        public string CallBackURL { get; set; }
        public int BillingId { get; set; }
    }
}