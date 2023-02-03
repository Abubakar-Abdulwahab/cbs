using System;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System.Security.Cryptography;
using Orchard.Users.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.Utilities;
using Orchard.Logging;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CorePaymentProviderService : ICoreExternalPaymentProviderService
    {
        private readonly IExternalPaymentProviderManager<ExternalPaymentProvider> _repo;
        private readonly ICoreSettingsService _settingsService;
        public ILogger Logger { get; set; }

        public CorePaymentProviderService(IExternalPaymentProviderManager<ExternalPaymentProvider> repo, ICoreSettingsService settingsService)
        {
            _repo = repo;
            _settingsService = settingsService;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get and check if payment provider is active payment provider
        /// <para>Gets payment provider and returns only if the provider is active</para>
        /// </summary>
        /// <param name="clientId"></param>
        /// <exception cref="PaymentProvider404"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <returns>ExternalPaymentProviderVM</returns>
        public ExternalPaymentProviderVM GetPaymentProvider(string clientId)
        {
            try
            {
                if (string.IsNullOrEmpty(clientId)) { throw new PaymentProvider404("Payment provider could not be found"); }
                clientId = clientId.Trim();
                ExternalPaymentProviderVM provider = _repo.GetProvider(clientId);

                if (provider == null) { throw new PaymentProvider404("Payment provider could not be found"); }
                if (!provider.IsActive) { throw new UserNotAuthorizedForThisActionException(ErrorLang.usernotauthorized().ToString()); }

                return provider;
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Get and check if payment provider is active payment provider
        /// <para>Gets payment provider and returns only if the provider is active</para>
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="PaymentProvider404"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <returns>ExternalPaymentProviderVM</returns>
        public ExternalPaymentProviderVM GetPaymentProvider(int id)
        {
            try
            {
                if (id <= 0) throw new PaymentProvider404("Payment provider could not be found");
                ExternalPaymentProviderVM provider = _repo.GetExternalProvider(id);

                if (provider == null) { throw new PaymentProvider404("Payment provider could not be found"); }
                if (!provider.IsActive) { throw new UserNotAuthorizedForThisActionException(ErrorLang.usernotauthorized().ToString()); }

                return provider;
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Saves external payment provider
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="user"></param>
        /// <param name="errors"></param>
        public void TrySaveExtPaymentProvider(ExternalPaymentProviderVM userInput, UserPartRecord user, List<ErrorModel> errors)
        {
            TenantCBSSettings tenantSettings = null;

            tenantSettings = _settingsService.HasTenantStateSettings();
            if (_repo.Count(x => x.Name == userInput.Name.Trim()) > 0)
            {
                errors.Add(new ErrorModel { FieldName = nameof(userInput.Name), ErrorMessage = "Payment provider with the same name already exists" });
                throw new DirtyFormDataException();
            }
            GenerateClientIdAndSecret(userInput, tenantSettings);

            ExternalPaymentProvider userModel = new ExternalPaymentProvider { Name = userInput.Name.Trim(), ClientID = userInput.ClientID, ClientSecret = userInput.ClientSecret, IsActive = true, AddedBy = user };

            if (!_repo.Save(userModel)) { throw new Exception($"Unable to create external payment provider {userModel}"); }
        }



        /// <summary>
        /// Generate Client ID and Client Secret for specified external payment provider model
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="tenantSettings"></param>
        private void GenerateClientIdAndSecret(ExternalPaymentProviderVM userInput, TenantCBSSettings tenantSettings)
        {
            try
            {
                userInput.ClientID = Util.OnWayHashThis(tenantSettings.Identifier + userInput.Name + DateTime.UtcNow.ToString(), tenantSettings.StateName);

                using (RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider())
                {
                    byte[] buffer = new byte[45];
                    cryptoRandomDataGenerator.GetBytes(buffer);
                    userInput.ClientSecret = Convert.ToBase64String(buffer);
                }
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Get payment provider 
        /// <para>ToFuture</para>
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>IEnumerable{PaymentProviderVM}</returns>
        public IEnumerable<PaymentProviderVM> GetProvider(int providerId)
        {
            return _repo.GetProvider(providerId);
        }


        /// <summary>
        /// Get payment provider client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>string</returns>
        public string GetClientSecretByClientId(string clientId)
        {
            return _repo.GetClientSecretByClientId(clientId);
        }


        /// <summary>
        /// Check if payment provider exists
        /// </summary>
        /// <param name="providerIdPased"></param>
        /// <returns>bool</returns>
        public bool CheckIfPaymentProviderExists(int providerIdPased)
        {
            return _repo.Count(x => x.Id == providerIdPased) != 1;
        }

    }
}