using InfoGRID.Pulse.SDK.DTO;
using InfoGRID.Pulse.SDK.Models;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Mail.Provider.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Notification;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Mail.Provider
{
    public class PulseProvider : IEmailProvider
    {
        public EmailProvider GetEmailNotificationProvider => EmailProvider.Pulse;
        private INotificationMessageManager<NotificationMessage> _notification;
        private INotificationMessageItemsManager<NotificationMessageItems> _notificationItems;
        private ICoreNotificationLoggingService _coreNotificationLoggingService;
        private IEmailNotification _hangfireNotification;
        public ILogger Logger { get; set; }

        public PulseProvider(INotificationMessageManager<NotificationMessage> notification, INotificationMessageItemsManager<NotificationMessageItems> notificationItems, ICoreNotificationLoggingService coreNotificationLoggingService)
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
                RecipientInfo recipientInfo = new RecipientInfo();
                recipientInfo.Value = verificationCodeObj.CBSUser.Email;
                recipientInfo.Type = RecipientType.Email;
                recipientInfo.Channel = NotificationChannel.Unknown;

                List<RecipientInfo> recipients = new List<RecipientInfo>();
                recipients.Add(recipientInfo);

                Dictionary<string, string> emailDetails = new Dictionary<string, string>();

                var dataParams = new { Name = verificationCodeObj.CBSUser.Name, Code = code };
                string sDataParams = JsonConvert.SerializeObject(dataParams);
                emailDetails.Add("Subject", verificationModel.Subject);
                emailDetails.Add("Sender", verificationModel.Sender);
                emailDetails.Add("Params", sDataParams);

                PulseData pulseData = new PulseData();
                pulseData.Recipients = recipients;
                pulseData.Params = emailDetails;

                Pulse pulse = new Pulse();
                pulse.Data = pulseData;
                pulse.Name = verificationModel.TemplateFileName;

                PulseHeader pulseHeader = new PulseHeader();
                pulseHeader.Type = RecipientType.Email.ToString();

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

                _hangfireNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }


        public bool SendEmail(EmailNotificationModel emailModel, bool logNotification = false)
        {
            try
            {
                Dictionary<string, string> emailDetails = new Dictionary<string, string>();

                string sDataParams = JsonConvert.SerializeObject(emailModel.Params);
                emailDetails.Add("Subject", emailModel.Subject);
                emailDetails.Add("Sender", emailModel.Sender);
                emailDetails.Add("Params", sDataParams);

                PulseData pulseData = new PulseData();
                pulseData.Recipients = new List<RecipientInfo> { 
                    new RecipientInfo
                    {
                        Value = emailModel.CBSUser.Email,
                        Type = RecipientType.Email,
                        Channel = NotificationChannel.Unknown
                    }
                };
                pulseData.Params = emailDetails;

                Pulse pulse = new Pulse();
                pulse.Data = pulseData;
                pulse.Name = emailModel.TemplateFileName;

                PulseHeader pulseHeader = new PulseHeader();
                pulseHeader.Type = nameof(RecipientType.Email);

                if (logNotification)
                {
                    _coreNotificationLoggingService.LogEmailNotification(emailModel, emailDetails);
                }

                _hangfireNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return false;
            }
        }
    }
}