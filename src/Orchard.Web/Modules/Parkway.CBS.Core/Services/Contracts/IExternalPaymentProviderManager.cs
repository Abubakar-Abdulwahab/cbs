using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IExternalPaymentProviderManager<ExternalPaymentProvider> : IDependency, IBaseManager<ExternalPaymentProvider>
    {
        /// <summary>
        /// Get a list of active payment provider
        /// </summary>
        /// <returns></returns>
        List<PaymentProviderVM> GetProviders();


        /// <summary>
        /// Get payment provider with the specified Id
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        IEnumerable<PaymentProviderVM> GetProvider(int providerId);


        /// <summary>
        /// Get payment provider with the specified client Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>ExternalPaymentProviderVM</returns>
        ExternalPaymentProviderVM GetProvider(string clientId);


        /// <summary>
        /// Get external payment provider with the specified Id
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>ExternalPaymentProviderVM</returns>
        ExternalPaymentProviderVM GetExternalProvider(int providerId);


        /// <summary>
        /// Get client secret by client Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        string GetClientSecretByClientId(string clientId);

    }
}
