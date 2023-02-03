using Orchard.Logging;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreNotificationMessageService : ICoreNotificationMessageService
    {
        private readonly INotificationMessageManager<NotificationMessage> _notificationMessageManager;
        public ILogger Logger { get; set; }

        public CoreNotificationMessageService(INotificationMessageManager<NotificationMessage> notificationMessageManager)
        {
            _notificationMessageManager = notificationMessageManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Save new notification
        /// </summary>
        /// <param name="notificationMessage"></param>
        /// <returns></returns>
        public bool SaveNotificationMessage(NotificationMessage notificationMessage)
        {
            bool response = false;

            try
            {
                response = _notificationMessageManager.Save(notificationMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }

            return response;
        }
    }
}