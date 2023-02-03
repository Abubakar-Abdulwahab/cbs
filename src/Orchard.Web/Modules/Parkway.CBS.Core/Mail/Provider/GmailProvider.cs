using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Mail.Provider.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.HangFireInterface.Notification;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Mail.Provider
{
    public class GmailProvider : IEmailProvider
    {
        public EmailProvider GetEmailNotificationProvider => EmailProvider.Gmail;
        private INotificationMessageManager<NotificationMessage> _notification;
        private INotificationMessageItemsManager<NotificationMessageItems> _notificationItems;
        private ICoreNotificationLoggingService _coreNotificationLoggingService;
        private IEmailNotification _hangfireNotification;
        ILogger Logger { get; set; }
        public GmailProvider(INotificationMessageManager<NotificationMessage> notification, INotificationMessageItemsManager<NotificationMessageItems> notificationItems, ICoreNotificationLoggingService coreNotificationLoggingService)
        {
            _notification = notification;
            _notificationItems = notificationItems;
            _coreNotificationLoggingService = coreNotificationLoggingService;
            _hangfireNotification = new EmailNotification();
            Logger = NullLogger.Instance;
        }

        public bool AccountVerification(VerificationCode verificationCodeObj, AccountVerificationEmailNotificationModel verificationModel, string code)
        {
            try
            {
                Dictionary<string, string> emailDetails = new Dictionary<string, string>();
                Dictionary<string, string> data = new Dictionary<string, string>();
                var dataParams = new { Code = code, Name = verificationCodeObj.CBSUser.Name };
                data.Add("Name", dataParams.Name);
                data.Add("Code", dataParams.Code);
                string sDataParams = JsonConvert.SerializeObject(dataParams);
                emailDetails.Add("Subject", verificationModel.Subject);
                emailDetails.Add("Sender", verificationModel.Sender);
                emailDetails.Add("Params", sDataParams);

                var model = new NotificationMessage
                {
                    TaxPayer = new TaxEntity { Id = verificationCodeObj.CBSUser.TaxEntity.Id },
                    NotificationTypeId = (int)NotificationMessageType.Email,
                    Recipient = verificationCodeObj.CBSUser.Email,
                    DeliveryStatusId = 1,
                    SentDate = DateTime.Now
                };
                var resp = _notification.Save(model);

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
                var respp = _notificationItems.SaveBundle(notificationMessageItems);

                string sModel = JsonConvert.SerializeObject(new { model.Recipient, Subject = verificationModel.Subject });
                _hangfireNotification.SendNotificationGmail(sModel, data, verificationModel.TemplateFileName);

                return true;
            }
            catch (Exception ex)
            {
                //Log error here
                return false;
            }
        }


        public bool SendEmail(EmailNotificationModel emailModel, bool logNotification = false)
        {
            try
            {
                LogNotificaion(emailModel, logNotification);
                string sModel = JsonConvert.SerializeObject(new { Recipient = emailModel.CBSUser.Email, Subject = emailModel.Subject });
                _hangfireNotification.SendNotificationGmail(sModel, emailModel.Params, $"{emailModel.TemplateFileName}.html");

                return true;
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return false;
            }
        }


        private void LogNotificaion(EmailNotificationModel emailModel, bool logNotification)
        {
            if (logNotification)
            {
                Dictionary<string, string> emailDetails = new Dictionary<string, string>();
                string sDataParams = JsonConvert.SerializeObject(emailModel.Params);
                emailDetails.Add("Subject", emailModel.Subject);
                emailDetails.Add("Sender", emailModel.Sender);
                emailDetails.Add("Params", sDataParams);

                _coreNotificationLoggingService.LogEmailNotification(emailModel, emailDetails);
            }
        }
    }
}