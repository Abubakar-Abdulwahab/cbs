using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;
using Parkway.CBS.RemoteClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem
{
    public class ExternalDataOfficers : IExternalDataOfficers
    {
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;

        public ExternalDataOfficers(IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Get policer officer details from the HR external data source. If the reponse has Error to be true, ResponseObject will contain List<PersonnelErrorResponseModel>. If the reponse has Error to be false, ResponseObject will contain PersonnelResponseModel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns>RootPersonnelResponse</returns>
        public RootPersonnelResponse GetPoliceOfficer(PersonnelRequestModel requestModel)
        {
            try
            {
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);

                var hrSystemBaseURL = siteConfig.Node.FirstOrDefault(x => x.Key == TenantConfigKeys.HRSystemBaseURL.ToString()).Value;
                var hrSystemUsername = siteConfig.Node.FirstOrDefault(x => x.Key == TenantConfigKeys.HRSystemUsername.ToString()).Value;
                var hrSystemKey = siteConfig.Node.FirstOrDefault(x => x.Key == TenantConfigKeys.HRSystemKey.ToString()).Value;

                string[] requiredParameters = { hrSystemBaseURL, hrSystemUsername, hrSystemKey };

                if (requiredParameters.Any(x => string.IsNullOrEmpty(x)))
                {
                    //throw exception
                    throw new Exception("Required parameter(s) for HR system not found");
                }

                var body = BuildRequestModel(requestModel);
                var sb = new System.Text.StringBuilder($"{hrSystemUsername}{hrSystemKey}:");
                foreach (var item in body)
                {
                    sb.Append($":{item.Value}");
                }

                string signature = sb.ToString();
                string encodedSignature = Util.SHA256ManagedHash(signature);
                string url = $"{hrSystemBaseURL}/personnel/{hrSystemUsername}/{encodedSignature}";


                IRemoteClient _remoteClient = new RemoteClient.RemoteClient();
                string response = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = null,
                    Model = body,
                    URL = url
                }, HttpMethod.Post, new Dictionary<string, string>(), isFormData: true);

                return JsonConvert.DeserializeObject<RootPersonnelResponse>(response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Build key value pair for HR form-data content type
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns>List<KeyValuePair<string, string>></returns>
        private static List<KeyValuePair<string, string>> BuildRequestModel(PersonnelRequestModel requestModel)
        {
            var body = new List<KeyValuePair<string, string>>();
            Type t = requestModel.GetType();
            foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
            {
                string value = pi.GetValue(requestModel, null)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    body.Add(new KeyValuePair<string, string>(pi.Name, value));
                }
            }

            return body;
        }
    }

}