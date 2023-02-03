using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreNotificationLoggingService : IDependency
    {
        /// <summary>
        /// Logs email notification
        /// </summary>
        /// <param name="emailModel"></param>
        /// <param name="emailDetails"></param>
        void LogEmailNotification(EmailNotificationModel emailModel, Dictionary<string, string> emailDetails);
    }
}
