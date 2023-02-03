using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.ThirdParty.Payment.Processor.Models;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIPaymentHandler : IDependency
    {
        PayDirectAPIResponseObj ProcessPaymentRequestForPayDirect(string requestStreamString, PayDirectConfigurations config, bool flatScheme = false);


        /// <summary>
        /// Payment notification for bank collect
        /// </summary>
        /// <param name="paymentController"></param>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse PaymentNotification(PaymentNotification model);

        /// <summary>
        /// Payment notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse PaymentNotification(PaymentNotification model, dynamic headerParams);


        /// <summary>
        /// Process paye payment notification
        /// </summary>
        /// <param name="model"></param>
        /// <param name="p"></param>
        /// <returns>APIResponse</returns>
        APIResponse PayePaymentNotification(PaymentNotification model, dynamic headerParams);


        /// <summary>
        /// Payment notification for NIBSS EBills pay
        /// </summary>
        /// <param name="requestStreamString"></param>
        /// <returns>APIResponse</returns>
        APIResponse NIBSSPaymentNotif(string requestStreamString);


        /// <summary>
        /// Payment notification for readycash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        APIResponse PaymentNotification(ReadyCashPaymentNotification model);

        /// <summary>
        /// Payment notification for NetPay card collection
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        APIResponse NetPayPaymentNotification(PaymentController callback, NetPayTransactionVM model);


        /// <summary>
        /// Get the transaction with the given refs
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <param name="channel"></param>
        /// <param name="paymentMethod"></param>
        /// <param name="exheaderParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetReadycashTransactionRequery(PaymentNotification model, dynamic headerParams);

    }
}
