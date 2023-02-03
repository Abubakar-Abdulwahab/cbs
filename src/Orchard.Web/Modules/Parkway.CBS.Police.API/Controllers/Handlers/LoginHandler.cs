using Orchard;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class LoginHandler : ILoginHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreBiometricAppActivityLogService _biometricAppLogService;
        private readonly IMembershipService _membershipService;

        public LoginHandler(ICoreBiometricAppActivityLogService biometricAppLogService, IOrchardServices orchardServices, IMembershipService membershipService)
        {
            _biometricAppLogService = biometricAppLogService;
            _orchardServices = orchardServices;
            _membershipService = membershipService;
        }

        public APIResponse ProcessLoginRequest(AuthenticationModel authenticationModel, HttpRequestMessage httpRequest)
        {
            Node usingVersionNode = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.UsingVersion)).FirstOrDefault();
            if (usingVersionNode != null && !string.IsNullOrEmpty(usingVersionNode.Value))
            {
                bool.TryParse(usingVersionNode.Value, out bool usingVersion);
                if (usingVersion)
                {
                    string lastestAppVersion = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.LatestBiometricAppVersion)).FirstOrDefault().Value;

                    _biometricAppLogService.LogBiometricAppDetail(httpRequest, HttpContext.Current.Request.UserHostAddress, out string appVersion);

                    if (appVersion != lastestAppVersion)
                    {
                        return new APIResponse { ResponseObject = "Unable to proceed, please contact Admin.", ErrorCode = ((int)PSSErrorCode.PSSBAIO).ToString(), StatusCode = HttpStatusCode.OK };
                    }
                } 
            }

            IUser ouser = _membershipService.ValidateUser(authenticationModel.Email, authenticationModel.Password);

            if (ouser == null)
            {
                return new APIResponse { Error = true, ResponseObject = "Invalid login credentials", StatusCode = HttpStatusCode.OK };
            }

            string token = Util.Encrypt(ouser.Id.ToString(), AppSettingsConfigurations.AESEncryptionSecret);

            return new APIResponse { ResponseObject = token, StatusCode = HttpStatusCode.OK };
        }



        private string GetClientIp(HttpRequestMessage request)
        {
            const string HttpContext = "MS_HttpContext";
            const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

            // Web-hosting
            if (request.Properties.ContainsKey(HttpContext))
            {
                HttpContextWrapper ctx =
                    (HttpContextWrapper)request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            // Self-hosting
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                RemoteEndpointMessageProperty remoteEndpoint =
                    (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }


            return null;
        }
    }
}