using Parkway.CBS.Entities.DTO;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.ClientServices.Invoicing.Contracts
{
    public interface IInvoiceGenerationType
    {
        /// <summary>
        /// Invoice type
        /// </summary>
        BillingType InvoiceGenerationType { get; }

        CreateInvoiceHelper GetInvoiceHelperModel(RevenueHeadDetailsForInvoiceGenerationLite revenueHeadDetails);
    }
}
