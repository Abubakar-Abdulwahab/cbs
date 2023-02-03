using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Mail.Provider.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.EbillsPay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class NIBSSEmailNotificationHandler : BaseAPIHandler, INIBSSEmailNotificationHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IEnumerable<Lazy<IEmailProvider>> _emailProvider;
        private readonly INibssIntegrationCredentialsManager<NibssIntegrationCredentials> _nibssIntegrationCredRepo;

        public NIBSSEmailNotificationHandler(IAdminSettingManager<ExpertSystemSettings> settingsRepository, IOrchardServices orchardServices, IEnumerable<Lazy<IEmailProvider>> emailProvider, INibssIntegrationCredentialsManager<NibssIntegrationCredentials> nibssIntegrationCredRepo) : base(settingsRepository)
        {
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _emailProvider = emailProvider;
            _nibssIntegrationCredRepo = nibssIntegrationCredRepo;
        }



        /// <summary>
        /// Send email containing IV and secret credentials to NIBSS email address 
        /// </summary>
        /// <returns>APIResponse</returns>
        public APIResponse SendNIBSSIntegrationCredentials()
        {
            try
            {
                StateConfig stateconfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);

                if (stateconfig == null)
                {
                    Logger.Error($"State config not found for sitename {_orchardServices.WorkContext.CurrentSite.SiteName}");
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception().Text };
                }

                Node NibssResetEmail = stateconfig.Node.Where(n => n.Key == nameof(EnvKeys.NibssResetEmail)).FirstOrDefault();

                Node baseUrl = stateconfig.Node.Where(n => n.Key == nameof(TenantConfigKeys.BaseURL)).FirstOrDefault();

                Node ebillsBillerName = stateconfig.Node.Where(n => n.Key == nameof(EnvKeys.EbillsBillerName)).FirstOrDefault();

                if (NibssResetEmail == null || string.IsNullOrEmpty(NibssResetEmail.Value))
                {
                    Logger.Error("NIBSS Ebills Reset: Reset Email not found");
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = "NIBSS Ebills Reset Email not found" };
                }

                if (baseUrl == null || string.IsNullOrEmpty(baseUrl.Value))
                {
                    Logger.Error("NIBSS Ebills Reset: Base URL not found");
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = "Base URL not found" };
                }

                if (ebillsBillerName == null || string.IsNullOrEmpty(ebillsBillerName.Value))
                {
                    Logger.Error("NIBSS Ebills Reset: Biller name not found");
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = "Biller name not found" };
                }

                Node emailNode = stateconfig.Node.Where(x => x.Key == nameof(TenantConfigKeys.IsEmailEnabled)).FirstOrDefault();
                if (emailNode != null && !string.IsNullOrEmpty(emailNode.Value))
                {
                    bool.TryParse(emailNode.Value, out bool isSettingsEnabled);
                    if (isSettingsEnabled)
                    {
                        NibssIntegrationCredentials credentials = _nibssIntegrationCredRepo.Get(c => c.IsActive);

                        if (credentials != null)
                        {
                            credentials.IsActive = false;
                            credentials.UpdatedAtUtc = DateTime.Now.ToLocalTime();
                        }

                        string IV = string.Join("", Util.StrongRandom().Take(16));
                        string secretKey = string.Join("", Util.StrongRandom().Take(16));

                        if (!_nibssIntegrationCredRepo.Save(new NibssIntegrationCredentials { IV = IV, SecretKey = secretKey, IsActive = true }))
                        {
                            throw new CouldNotSaveRecord("Could not save NIBSS Integration Credentials");
                        }

                        //send email
                        bool result = int.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.EmailProvider), out int providerId);

                        var nibssEmailNotifObj = new EmailNotificationModel
                        {
                            Params = new Dictionary<string, string>() { { "IV", IV }, { "SecretKey", secretKey }, { "Date", DateTime.Now.Year.ToString() }, { "BaseUrl", baseUrl.Value}, { "BillerName", ebillsBillerName.Value } },
                            CBSUser = new CBSUserVM { Email = NibssResetEmail.Value },
                            Sender = EmailTemplateFileNames.Sender.GetDescription(),
                            Subject = "INTEGRATION CREDENTIALS",
                            TemplateFileName = EmailTemplateFileNames.NibssSecretKeyAndIVNotification.GetDescription()
                        };

                        if (!result)
                        {
                            providerId = (int)EmailProvider.Pulse;
                        }
                        foreach (var impl in _emailProvider)
                        {
                            if ((EmailProvider)providerId == impl.Value.GetEmailNotificationProvider)
                            {
                                impl.Value.SendEmail(nibssEmailNotifObj);
                            }
                        }

                        //remove old credentials from cache if existing
                        string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
                        if (ObjectCacheProvider.GetCachedObject<NIBSSIntegrationCredentialVM>(tenant, $"{nameof(CachePrefix.NibssCredential)}") != null)
                        {
                            ObjectCacheProvider.RemoveCachedObject(tenant, $"{nameof(CachePrefix.NibssCredential)}");
                        }

                        Logger.Information("NIBSS Ebills credential reset successful");
                        return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = "Successful" };
                    }
                }
                Logger.Error("Email is not enabled for NIBSS IV and Secret key reset");
                return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.emailnotificationnotenable.Text };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception SendNIBSSIntegrationCredentials" + exception.Message);
                return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception().Text };
            }
        }
    }
}