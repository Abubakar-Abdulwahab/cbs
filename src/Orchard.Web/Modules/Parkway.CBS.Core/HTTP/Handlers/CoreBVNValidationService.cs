using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreBVNValidationService : ICoreBVNValidationService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IBVNValidationResponseManager<BVNValidationResponse> _bvnValidationResponse;
        public ILogger Logger { get; set; }
        public CoreBVNValidationService(IOrchardServices orchardServices, IMembershipService membershipService, IBVNValidationResponseManager<BVNValidationResponse> bvnValidationResponse)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _bvnValidationResponse = bvnValidationResponse;
            Logger = NullLogger.Instance;
        }

        public string ValidateBVN(string bvn, out string errorMessage)
        {
            errorMessage = string.Empty;
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            HttpResponseMessage response;
            try
            {
                string BVNValidationEndpoint = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.BVNValidationEndpoint)];
                string ok = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.OK)];
                string internalError = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.InternalError)];
                UriBuilder uri = new UriBuilder(BVNValidationEndpoint);

                if (bvn.Length != 11) { errorMessage = "BVN length not valid. Must be 11 characters."; throw new Exception("Invalid BVN specified"); }
                using (var client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri.ToString());
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = new StringContent(JsonConvert.SerializeObject(new { bvn = bvn }), Encoding.UTF8, "application/json");
                    Logger.Information($"Validating BVN {bvn}");
                    response = client.SendAsync(request).Result;
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    Logger.Information($"Response from BVN validation ---- {responseContent}");
                    if (!response.IsSuccessStatusCode) { return null; }
                    BVNValidationResponseHelperModel responeContentObj = JsonConvert.DeserializeObject<BVNValidationResponseHelperModel>(responseContent);
                    if (responeContentObj.responseCode == ok)
                    {
                        //successful bvn validation response
                        Logger.Information($"BVN validated successfully. Name - {responeContentObj.responseObject.FirstName} Middle name - {responeContentObj.responseObject.MiddleName} Last name -{ responeContentObj.responseObject.LastName}");
                        BVNValidationResponse bvnResponse = responeContentObj.responseObject;
                        bvnResponse.ResponseDump = responseContent;
                        if (!_bvnValidationResponse.Save(bvnResponse))
                        {
                            Logger.Error("ValidateBVN: An error occured while saving the record");
                        }
                        return responseContent;
                    }
                    else if (responeContentObj.responseCode == internalError)
                    {
                        //exception error
                        return string.Empty; //returns BVN not found in this scenario
                    }
                    return null; //returns unable to validate BVN in this scenario
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
            catch (Exception exception) { Logger.Error(exception, $"Unable to validate BVN {bvn}. Exception message --- {exception.Message}"); throw; }
        }
    }
}