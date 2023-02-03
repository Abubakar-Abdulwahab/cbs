using Newtonsoft.Json;
using Orchard;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using Parkway.CBS.RemoteClient;
using Parkway.CBS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Parkway.CBS.Police.Core.PSSIdentification
{
    public class AccountNumberValidation : IAccountNumberValidation
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRemoteClient _remoteClient;

        public AccountNumberValidation(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            _remoteClient = new RemoteClient.RemoteClient();

        }

        /// <summary>
        /// Validates and verify the account number
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="bankCode"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>Account Name</returns>
        public string ValidateAccountNumber(string accountNumber, string bankCode)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                throw new ArgumentException($"'{nameof(accountNumber)}' cannot be null or empty.", nameof(accountNumber));
            }

            if (string.IsNullOrEmpty(bankCode))
            {
                throw new ArgumentException($"'{nameof(bankCode)}' cannot be null or empty.", nameof(bankCode));
            }


            string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            var clientCode = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.POSSAPSettlementClientCode.ToString()).FirstOrDefault().Value;
            var secret = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.POSSAPSettlementSecret.ToString()).FirstOrDefault().Value;
            var settlementAuthTokenURL = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.SettlementEngineAuthTokenURL.ToString()).FirstOrDefault().Value;
            var settlementAccountVerifyURL = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.SettlementEngineAccountVerifyURL.ToString()).FirstOrDefault().Value;

            if (string.IsNullOrEmpty(clientCode) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(settlementAuthTokenURL) || string.IsNullOrEmpty(settlementAccountVerifyURL))
            {
                throw new Exception("Unable to get details for calling settlement engine");
            }

            SettlementEngineAuthVM authRequest = new SettlementEngineAuthVM { ClientCode = clientCode, hmac = Util.HMACHash256(clientCode, secret) };
            string stoken = _remoteClient.SendRequest(new RequestModel
            {
                Headers = new Dictionary<string, dynamic> { },
                Model = authRequest,
                URL = settlementAuthTokenURL
            }, HttpMethod.Post, new Dictionary<string, string> { });

            SettlementEngineAuthResponseVM authtoken = JsonConvert.DeserializeObject<SettlementEngineAuthResponseVM>(stoken);
            var requestModel = new SettlementEngineAccountVerifyRequestVM
            {
                AccountNumber = accountNumber,
                BankCode = bankCode
            };

            string response = _remoteClient.SendRequest(new RequestModel
            {
                Headers = new Dictionary<string, dynamic> { { "Authorization", $"Bearer {authtoken.token}" } },
                Model = requestModel,
                URL = settlementAccountVerifyURL
            }, HttpMethod.Post, new Dictionary<string, string> { });

            return JsonConvert.DeserializeObject<SettlementEngineAccountVerifyResponseVM>(response).ResponseObject.AccountName;

        }
    }
}