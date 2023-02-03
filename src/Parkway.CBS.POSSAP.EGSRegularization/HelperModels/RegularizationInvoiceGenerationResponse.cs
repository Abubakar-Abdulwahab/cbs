using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModels
{
    public class RegularizationInvoiceGenerationResponse
    {
        public List<GenerateInvoiceRequestModel> RequestModel { get; set; }

        public GenerateInvoiceRequestModel GroupDetails { get; set; }

        public CreateInvoiceHelper InvoiceHelper { get; set; }

        public CashFlowCreateCustomerAndInvoiceResponse InvoiceResponse { get; set; }
    }
}
