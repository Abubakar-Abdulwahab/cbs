using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts
{
    public interface IBillingTypeInvoiceConfirmation : IDependency
    {
        BillingType BillingType { get; }


        /// <summary>
        /// When the invoice has been confirmed and created
        /// We need to move the valid record entries into
        /// the PAYE records table and also move the valid items as well
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="invoiceId"></param>
        /// <returns>String | BatchRef</returns>
        string InvoiceHasBeenConfirmed(InvoiceConfirmedModel confirmationModel, long invoiceId);

    }
}
