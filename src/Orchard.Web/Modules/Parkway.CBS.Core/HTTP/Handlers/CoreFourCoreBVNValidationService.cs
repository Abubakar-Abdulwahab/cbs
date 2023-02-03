using Newtonsoft.Json;
using Orchard.Logging;
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
    public class CoreFourCoreBVNValidationService : ICoreFourCoreBVNValidationService
    {
        private readonly IRemoteClient _remoteClient;
        private readonly IBVNValidationResponseManager<BVNValidationResponse> _bvnValidationResponse;
        public ILogger Logger { get; set; }

        public CoreFourCoreBVNValidationService(IBVNValidationResponseManager<BVNValidationResponse> bvnValidationResponse)
        {
            _bvnValidationResponse = bvnValidationResponse;
            _remoteClient = new RemoteClient.RemoteClient();
            Logger = NullLogger.Instance;
        }

        /// <summary>
        ///Validates Bank Verification Number
        /// </summary>
        /// <param name="bvn"></param>
        /// <returns></returns>
        public FourCoreBVNValidationResponse ValidateBVN(string bvn)
        {
            try
            {
                string BVNValidationEndpoint = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.FourCoreBVNValidationEndpoint)];
                string BVNValidationUsername = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.FourCoreBVNValidationUsername)];
                string BVNValidationSecret = System.Configuration.ConfigurationManager.AppSettings[nameof(AppSettingEnum.FourCoreBVNValidationSecret)];
                //compute the signature by generating a sha256Hash of the lowercase MD5 hash result (lowercase to match what is expected by the bvn validation endpoint)
                string signatureHash = Utilities.Util.Sha256Hash(Utilities.Util.CreateMD5Hash(string.Format("{0}::{1}{2}{3}", BVNValidationUsername, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).ToLower());
                UriBuilder uri = new UriBuilder(BVNValidationEndpoint);

                Logger.Information($"Calling Four Core BVN Validation Service to validate BVN - {bvn}");

                string responseObj = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = new Dictionary<string, dynamic> { { "Authorization", $"{BVNValidationSecret}" }, { "Signature", $"{signatureHash}" } },
                    Model = new { },
                    URL = BVNValidationEndpoint
                }, HttpMethod.Get, new Dictionary<string, string> { { "bvn", bvn } });

                FourCoreBVNValidationResponse bvnValidationResponse = JsonConvert.DeserializeObject<FourCoreBVNValidationResponse>(responseObj);

                Logger.Information($"BVN validation api call successful. BVN - {bvnValidationResponse.Data.BVN.BVN}, First Name - {bvnValidationResponse.Data.BVN.FirstName}, Middle Name - {bvnValidationResponse.Data.BVN.MiddleName}, Last Name - {bvnValidationResponse.Data.BVN.LastName}");


                if (bvnValidationResponse.Status && !string.IsNullOrEmpty(bvnValidationResponse.Data.BVN.LastName))
                {
                    if (!_bvnValidationResponse.Save(new BVNValidationResponse { 
                        FirstName = bvnValidationResponse.Data.BVN.FirstName, 
                        LastName = bvnValidationResponse.Data.BVN.LastName, 
                        Base64Image = bvnValidationResponse.Data.BVN.Image, 
                        PhoneNumber = bvnValidationResponse.Data.BVN.PhoneNumber, 
                        StateOfOrigin = bvnValidationResponse.Data.BVN.State, 
                        MiddleName = bvnValidationResponse.Data.BVN.MiddleName, 
                        EmailAddress = bvnValidationResponse.Data.BVN.Email, 
                        Gender = bvnValidationResponse.Data.BVN.Gender, 
                        PhoneNumber2 = bvnValidationResponse.Data.BVN.AltNumber, 
                        BVN = bvnValidationResponse.Data.BVN.BVN,
                        ResponseDump = responseObj
                    }))
                    {
                        Logger.Error("ValidateBVN: An error occured while saving the record");
                    }
                }

                return bvnValidationResponse;
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