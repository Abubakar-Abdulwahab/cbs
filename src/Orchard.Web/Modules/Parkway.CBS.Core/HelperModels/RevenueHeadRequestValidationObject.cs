using System.Collections.Concurrent;

namespace Parkway.CBS.Core.HelperModels
{
    public class RevenueHeadRequestValidationObject
    {
        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        /// <summary>
        /// Invoice items
        /// </summary>
        public ConcurrentStack<Cashflow.Ng.Models.CashFlowCreateInvoice.CashFlowProductModel> PartInvoiceItems { get; set; }
    }
}