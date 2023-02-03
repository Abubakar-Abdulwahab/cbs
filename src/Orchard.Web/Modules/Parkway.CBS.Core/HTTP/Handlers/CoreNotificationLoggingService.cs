using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreNotificationLoggingService : ICoreNotificationLoggingService
    {
        private INotificationMessageManager<NotificationMessage> _notification;
        private INotificationMessageItemsManager<NotificationMessageItems> _notificationItems;
        ILogger Logger { get; set; }
        public CoreNotificationLoggingService(INotificationMessageManager<NotificationMessage> notification, INotificationMessageItemsManager<NotificationMessageItems> notificationItems)
        {
            _notification = notification;
            _notificationItems = notificationItems;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Logs email notification
        /// </summary>
        /// <param name="emailModel"></param>
        /// <param name="emailDetails"></param>
        public void LogEmailNotification(EmailNotificationModel emailModel ,Dictionary<string, string> emailDetails)
        {
            try
            {
                var model = new NotificationMessage
                {
                    TaxPayer = new TaxEntity { Id = emailModel.CBSUser.TaxEntity.Id },
                    NotificationTypeId = (int)NotificationMessageType.Email,
                    Recipient = emailModel.CBSUser.Email,
                    DeliveryStatusId = 1,
                    SentDate = DateTime.Now
                };
                _notification.Save(model);

                //Save the items
                List<NotificationMessageItems> notificationMessageItems = new List<NotificationMessageItems>();
                foreach (var item in emailDetails)
                {
                    NotificationMessageItems messageItems = new NotificationMessageItems();
                    messageItems.KeyName = item.Key;
                    messageItems.Value = item.Value;
                    messageItems.NotificationMessage = new NotificationMessage { Id = model.Id };
                    notificationMessageItems.Add(messageItems);
                }
                _notificationItems.SaveBundle(notificationMessageItems);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
        }
    }
}