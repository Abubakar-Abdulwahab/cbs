using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IRDCAPICreateInvoiceHandler : IDependency
    {

        /// <summary>
        /// Generate invoice for readycash 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="exheaderParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse GenerateInvoice(RDCBillerCreateInvoiceModel model, dynamic exheaderParams);

    }
}
