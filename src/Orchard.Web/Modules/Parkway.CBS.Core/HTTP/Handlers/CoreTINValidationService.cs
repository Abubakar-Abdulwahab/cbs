using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreTINValidationService : ICoreTINValidationService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly ITINValidationResponseManager<TINValidationResponse> _tinValidationResponseManager;
        public ILogger Logger { get; set; }

        public CoreTINValidationService(IOrchardServices orchardServices, IMembershipService membershipService, ITINValidationResponseManager<TINValidationResponse> tinValidationResponseManager)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _tinValidationResponseManager = tinValidationResponseManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Validate FIRS Tax Identification Number
        /// </summary>
        /// <param name="tin"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public string ValidateTIN(string tin, out string errorMessage)
        {
            errorMessage = string.Empty;
            HttpResponseMessage response;
            try
            {
                if (tin.Length < 11 || tin.Length > 14) { errorMessage = "Tax Identification Number length not valid."; throw new Exception("Invalid TIN specified"); }
                string TINValidationEndpoint = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.TINValidationEndpoint)];
                string username = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.TINValidationUsername)];
                string password = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.TINValidationPassword)];
                UriBuilder uri = new UriBuilder(string.Format("{0}/{1}", TINValidationEndpoint, tin));
                string authorizationHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

                using (var client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeaderValue);
                    Logger.Information($"Validating TIN {tin}");
                    response = client.SendAsync(request).Result;
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    Logger.Information($"Response from TIN validation ---- {responseContent}");
                    dynamic responseContentObj = JsonConvert.DeserializeObject(responseContent);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            //successful tin validation response
                            Logger.Information($"TIN validated successfully. Name - {responseContentObj.TaxPayerName}");
                            TINValidationResponse validationResponse = JsonConvert.DeserializeObject<TINValidationResponse>(responseContent);
                            validationResponse.ResponseDump = responseContent;
                            if (!_tinValidationResponseManager.Save(validationResponse))
                            {
                                Logger.Error("ValidateTIN: An error occured while saving the record");
                            }
                            break;
                        case HttpStatusCode.NotFound:
                            //tin not found response
                            errorMessage = responseContentObj.Message;
                            Logger.Error($"TIN {tin} not found. Response payload --- {responseContentObj}");
                            break;
                        case HttpStatusCode.InternalServerError:
                            //FIRS TIN validation service error
                            errorMessage = "Unable to validate TIN with the FIRS system. Please try again later.";
                            Logger.Error($"Unable to validate TIN {tin} with the FIRS system. Response payload --- {responseContentObj}");
                            break;
                        default:
                            //something went wrong
                            errorMessage = Lang.ErrorLang.genericexception().Text;
                            Logger.Error($"Response payload --- {responseContentObj}");
                            break;
                    }
                    return responseContent;
                };
            }
            catch (HttpRequestException exception)
            {
                throw new Exception(exception.InnerException.Message);
            }
            catch (AggregateException exception)
            {
                throw new Exception(exception.InnerExceptions.First().InnerException.Message);
            }
            catch (Exception exception) { Logger.Error(exception, $"Unable to validate TIN {tin}. Exception message --- {exception.Message}"); throw; }
        }
    }
}