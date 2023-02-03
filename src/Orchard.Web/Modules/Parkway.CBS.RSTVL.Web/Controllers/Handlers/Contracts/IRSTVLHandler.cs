using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;

namespace Parkway.CBS.RSTVL.Web.Controllers.Handlers.Contracts
{
    public interface IRSTVLHandler : IDependency
    {
        /// <summary>
        /// Save licence details
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="invoiceDetails"></param>
        /// <param name="taxEntityId"></param>
        void SaveLicenceDetails(GenerateInvoiceStepsModel processStage, InvoiceGeneratedResponseExtn invoiceDetails, Int64 taxEntityId);

    }
}
