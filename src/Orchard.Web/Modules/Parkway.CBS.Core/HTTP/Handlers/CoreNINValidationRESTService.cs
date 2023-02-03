using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreNINValidationRESTService : ICoreNINValidationService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly ININValidationResponseManager<NINValidationResponse> _ninValidationResponseRepo;
        public ILogger Logger { get; set; }


        public CoreNINValidationRESTService(IOrchardServices orchardServices, IMembershipService membershipService, ININValidationResponseManager<NINValidationResponse> ninValidationResponseRepo)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _ninValidationResponseRepo = ninValidationResponseRepo;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Validate National Identification Number
        /// </summary>
        /// <param name="nin"></param>
        /// <param name="errormessage"></param>
        /// <returns></returns>
        public dynamic ValidateNIN(string nin, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {

                string validationEndpoint = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.NINValidationRESTEndpoint)];
                string validationEndpointKey = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.NINValidationRESTEndpointKey)];
                string url = $"{validationEndpoint}?pickNIN={nin}&key={validationEndpointKey}";

                IRemoteClient _remoteClient = new RemoteClient.RemoteClient();
                Logger.Information($"Validating NIN {nin}");
                string response = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = null,
                    Model = null,
                    URL = url
                }, HttpMethod.Get, new Dictionary<string, string>());
                Logger.Information($"About to deserialize response from NIN validation. NIN {nin}");
                if (!string.IsNullOrEmpty(response) && response != "null")
                {
                    NINValidationResponse responseContentObj = JsonConvert.DeserializeObject<NINValidationResponse>(response);
                    responseContentObj.ResponseDump = response;
                    Logger.Information($"Response from NIN validation. NIN: {nin}. Fullname: {responseContentObj.FirstName} {responseContentObj.MiddleName} {responseContentObj.Surname}");
                    //successful NIN validation response
                    if (!_ninValidationResponseRepo.Save(responseContentObj))
                    {
                        Logger.Error("ValidateNIN: An error occured while saving the record");
                    }
                    return responseContentObj;
                }
                Logger.Information($"Response from NIN is null. NIN {nin}");
                return null; //returns NIN not found
            }
            catch (HttpRequestException exception)
            {
                throw new Exception(exception.InnerException.Message);
            }
            catch (AggregateException exception)
            {
                throw new Exception(exception.InnerExceptions.First().InnerException.Message);
            }
            catch (Exception exception) { Logger.Error(exception, $"Unable to validate NIN {nin}. Exception message --- {exception.Message}"); throw; }
        }
    }
}