using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreExternalPaymentProviderService : IDependency
    {

        /// <summary>
        /// Get and check if payment provider is active payment provider
        /// <para>Gets payment provider and returns only if the provider is active</para>
        /// </summary>
        /// <param name="clientId"></param>
        /// <exception cref="PaymentProvider404"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <returns>ExternalPaymentProviderVM</returns>
        ExternalPaymentProviderVM GetPaymentProvider(string clientId);


        /// <summary>
        /// Get and check if payment provider is active payment provider
        /// <para>Gets payment provider and returns only if the provider is active</para>
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="PaymentProvider404"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <returns>ExternalPaymentProviderVM</returns>
        ExternalPaymentProviderVM GetPaymentProvider(int id);

        /// <summary>
        /// Saves external payment provider
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="user"></param>
        /// <param name="errors"></param>
        void TrySaveExtPaymentProvider(ExternalPaymentProviderVM userInput, UserPartRecord user, List<ErrorModel> errors);


        /// <summary>
        /// Get payment provider 
        /// <para>ToFuture</para>
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>IEnumerable{PaymentProviderVM}</returns>
        IEnumerable<PaymentProviderVM> GetProvider(int providerId);


        /// <summary>
        /// Get payment provider client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>string</returns>
        string GetClientSecretByClientId(string clientId);


        /// <summary>
        /// Check if payment provider exists
        /// </summary>
        /// <param name="providerIdPased"></param>
        /// <returns>bool</returns>
        bool CheckIfPaymentProviderExists(int providerIdPased);

    }
}
