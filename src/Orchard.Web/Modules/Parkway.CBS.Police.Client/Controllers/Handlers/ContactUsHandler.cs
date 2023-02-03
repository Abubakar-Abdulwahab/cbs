using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Mail.Provider.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class ContactUsHandler : IContactUsHandler
    {
        private readonly IEnumerable<Lazy<IPSSEmailProvider>> _emailProvider;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }

        public ContactUsHandler(IEnumerable<Lazy<IPSSEmailProvider>> emailProvider, IOrchardServices orchardServices)
        {
            _emailProvider = emailProvider;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Sends an email to the configured support email
        /// </summary>
        /// <param name="messageModel"></param>
        /// <returns></returns>
        public bool SendContactUsRequest(dynamic messageModel)
        {
            try
            {
                if (PSSUtil.IsEmailEnabled(_orchardServices.WorkContext.CurrentSite.SiteName))
                {
                    bool result = int.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.EmailProvider), out int providerId);

                    if (!result)
                    {
                        providerId = (int)EmailProvider.Pulse;
                    }
                    foreach (var impl in _emailProvider)
                    {
                        if ((EmailProvider)providerId == impl.Value.GetEmailNotificationProvider)
                        {
                            return impl.Value.ContactUsNotification(messageModel);
                        }
                    }
                }
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); }
            return false;
        }
    }
}