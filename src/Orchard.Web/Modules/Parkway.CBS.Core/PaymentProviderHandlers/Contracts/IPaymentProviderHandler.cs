using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.PaymentProviderHandlers.Contracts
{
    public interface IPaymentProviderHandler : IDependency
    {

        /// <summary>
        /// Do validation
        /// </summary>
        /// <typeparam name="V">Validation model for the payment provider</typeparam>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        APIResponse DoValidation(ValidationRequest validationModel, ExternalPaymentProviderVM paymentProvider, dynamic parameters = null);

        /// <summary>
        /// Do payment sychronization
        /// </summary>
        /// <param name="model"></param>
        /// <param name="paymentProvider"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        APIResponse DoSynchronization(PaymentNotification model, ExternalPaymentProviderVM paymentProvider, dynamic parameters = null);

    }


    public interface IReadyCashPaymentProviderHandler : IDependency
    {
        APIResponse RequeryTransaction(PaymentNotification model, ExternalPaymentProviderVM paymentProvider);
    }
}
