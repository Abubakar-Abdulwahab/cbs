using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts
{
    public interface IBridgeAPIPaymentHandler : IDependency
    {

        /// <summary>
        /// Do payment notification for readycash merchant payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse PaymentNotification(ReadyCashPaymentNotification model);


        /// <summary>
        /// Process payment notifications
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse ProcessPaymentNotification(PaymentNotification model, dynamic headerParams);


        /// <summary>
        /// Check for transaction with the ref and channel for this payment provider
        /// </summary>
        /// <param name="model"></param>
        /// <param name="p"></param>
        /// <returns>APIResponse</returns>
        APIResponse RequeryTransaction(PaymentNotification model, dynamic headerParams);

        /// <summary>
        /// This handles payment notification for all the tenants 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse GenericePaymentNotification(PaymentNotification model, dynamic headerParams);
    }
}
