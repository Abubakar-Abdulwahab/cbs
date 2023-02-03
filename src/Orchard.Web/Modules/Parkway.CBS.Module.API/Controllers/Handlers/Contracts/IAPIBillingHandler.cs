using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIBillingHandler : IDependency
    {
        /// <summary>
        /// Create billing
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse CreateBilling(BillingController callback, BillingHelperModel model, dynamic headerParams);


        /// <summary>
        /// Edit Billing info
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse EditBilling(BillingController callback, BillingHelperModel model, dynamic headerParams);
    }
}
