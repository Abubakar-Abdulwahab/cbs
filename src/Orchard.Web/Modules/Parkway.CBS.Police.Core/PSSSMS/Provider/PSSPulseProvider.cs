using InfoGRID.Pulse.SDK.DTO;
using InfoGRID.Pulse.SDK.Models;
using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Notification;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using Parkway.CBS.Police.Core.PSSSMS.Provider.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.PSSSMS.Provider
{
    public class PSSPulseProvider : IPSSSMSProvider
    {
        public SMSProvider GetSMSNotificationProvider => SMSProvider.Pulse;
        private INotificationMessageManager<NotificationMessage> _notification;
        private INotificationMessageItemsManager<NotificationMessageItems> _notificationItems;
        private ISMSNotification _smsNotification;
        const string defaultCode = "+234";
        public ILogger Logger { get; set; }


        public PSSPulseProvider(INotificationMessageItemsManager<NotificationMessageItems> notificationItems, INotificationMessageManager<NotificationMessage> notification)
        {
            _notification = notification;
            _notificationItems = notificationItems;
            _smsNotification = new SMSNotification();
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Send an approval notification for a particular request
        /// </summary>
        /// <param name="smsDetailVM"></param>
        /// <returns></returns>
        public bool PSSRequestApproval(dynamic smsDetailVM, string tenantName)
        {
            try
            {
                RecipientInfo recipientInfo = new RecipientInfo();
                recipientInfo.Value = (string)smsDetailVM.PhoneNumber;
                recipientInfo.Type = RecipientType.SMS;
                recipientInfo.Channel = NotificationChannel.Unknown;

                List<RecipientInfo> recipients = new List<RecipientInfo>();
                recipients.Add(recipientInfo);

                Dictionary<string, string> smsDetails = new Dictionary<string, string>();
                string message = $"Dear {(string)smsDetailVM.Name}, your {(string)smsDetailVM.RequestType} request with File Number {(string)smsDetailVM.FileNumber} has been approved.";

                smsDetails.Add("Code", defaultCode);
                smsDetails.Add("Message", message);

                PulseData pulseData = new PulseData();
                pulseData.Recipients = recipients;
                pulseData.Params = smsDetails;

                Pulse pulse = new Pulse();
                pulse.Data = pulseData;
                pulse.Name = Util.GetPulseSMSTemplateName(tenantName);

                PulseHeader pulseHeader = new PulseHeader();
                pulseHeader.Type = RecipientType.SMS.ToString();

                var model = new NotificationMessage
                {
                    TaxPayer = new TaxEntity { Id = (Int64)smsDetailVM.TaxEntityId },
                    NotificationTypeId = (int)NotificationMessageType.SMS,
                    Recipient = (string)smsDetailVM.PhoneNumber,
                    DeliveryStatusId = 1,
                    SentDate = DateTime.Now
                };
                var resp = _notification.Save(model);

                //Save the items
                List<NotificationMessageItems> notificationMessageItems = new List<NotificationMessageItems>();
                foreach (var item in smsDetails)
                {
                    NotificationMessageItems messageItems = new NotificationMessageItems();
                    messageItems.KeyName = item.Key;
                    messageItems.Value = item.Value;
                    messageItems.NotificationMessage = new NotificationMessage { Id = model.Id };
                    notificationMessageItems.Add(messageItems);
                }
                var respp = _notificationItems.SaveBundle(notificationMessageItems);

                _smsNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send a reject notification for a particular request
        /// </summary>
        /// <param name="smsDetailVM"></param>
        /// <returns></returns>
        public bool PSSRequestRejection(dynamic smsDetailVM, string tenantName)
        {
            try
            {
                RecipientInfo recipientInfo = new RecipientInfo();
                recipientInfo.Value = (string)smsDetailVM.PhoneNumber;
                recipientInfo.Type = RecipientType.SMS;
                recipientInfo.Channel = NotificationChannel.Unknown;

                List<RecipientInfo> recipients = new List<RecipientInfo>();
                recipients.Add(recipientInfo);

                Dictionary<string, string> smsDetails = new Dictionary<string, string>();
                string message = $"Dear {(string)smsDetailVM.Name}, your {(string)smsDetailVM.RequestType} request with File Number {(string)smsDetailVM.FileNumber} has been rejected. Reason: {(string)smsDetailVM.Comment}";

                smsDetails.Add("Code", defaultCode);
                smsDetails.Add("Message", message);

                PulseData pulseData = new PulseData();
                pulseData.Recipients = recipients;
                pulseData.Params = smsDetails;

                Pulse pulse = new Pulse();
                pulse.Data = pulseData;
                pulse.Name = Util.GetPulseSMSTemplateName(tenantName);

                PulseHeader pulseHeader = new PulseHeader();
                pulseHeader.Type = RecipientType.SMS.ToString();

                var model = new NotificationMessage
                {
                    TaxPayer = new TaxEntity { Id = (Int64)smsDetailVM.TaxEntityId },
                    NotificationTypeId = (int)NotificationMessageType.SMS,
                    Recipient = (string)smsDetailVM.PhoneNumber,
                    DeliveryStatusId = 1,
                    SentDate = DateTime.Now
                };
                var resp = _notification.Save(model);

                //Save the items
                List<NotificationMessageItems> notificationMessageItems = new List<NotificationMessageItems>();
                foreach (var item in smsDetails)
                {
                    NotificationMessageItems messageItems = new NotificationMessageItems();
                    messageItems.KeyName = item.Key;
                    messageItems.Value = item.Value;
                    messageItems.NotificationMessage = new NotificationMessage { Id = model.Id };
                    notificationMessageItems.Add(messageItems);
                }
                var respp = _notificationItems.SaveBundle(notificationMessageItems);

                _smsNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send a notification to the person that should approve a pending request
        /// </summary>
        /// <param name="smsDetailVM"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public bool NotifyApprover(dynamic smsDetailVM, List<string> phoneNumbers, string tenantName)
        {
            try
            {
                if (phoneNumbers.Count == 0)
                {
                    throw new Exception($"Phone number is empty");
                }

                List<RecipientInfo> recipients = new List<RecipientInfo>();
                foreach (string phoneNumber in phoneNumbers)
                {
                    RecipientInfo recipientInfo = new RecipientInfo
                    {
                        Value = phoneNumber,
                        Type = RecipientType.SMS,
                        Channel = NotificationChannel.Unknown
                    };
                    recipients.Add(recipientInfo);
                }

                Dictionary<string, string> smsDetails = new Dictionary<string, string>();
                string message = $"Dear {(string)smsDetailVM.Name}, Please be informed that {(string)smsDetailVM.RequestType} with file number {(string)smsDetailVM.FileNumber} is awaiting your approval.";

                smsDetails.Add("Code", defaultCode);
                smsDetails.Add("Message", message);

                PulseData pulseData = new PulseData();
                pulseData.Recipients = recipients;
                pulseData.Params = smsDetails;

                Pulse pulse = new Pulse();
                pulse.Data = pulseData;
                pulse.Name = Util.GetPulseSMSTemplateName(tenantName);

                PulseHeader pulseHeader = new PulseHeader();
                pulseHeader.Type = RecipientType.SMS.ToString();

                _smsNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send sms to a list of users
        /// </summary>
        /// <param name="phoneNumbers"></param>
        /// <param name="message"></param>
        /// <returns>bool</returns>
        public bool SendSMS(List<string> phoneNumbers, string message, string tenantName)
        {
            try
            {
                if (phoneNumbers.Count == 0 || string.IsNullOrEmpty(message))
                {
                    throw new Exception($"Phone number or message is empty");
                }

                List<RecipientInfo> recipients = new List<RecipientInfo>();
                foreach (string phoneNumber in phoneNumbers)
                {
                    RecipientInfo recipientInfo = new RecipientInfo
                    {
                        Value = phoneNumber,
                        Type = RecipientType.SMS,
                        Channel = NotificationChannel.Unknown
                    };
                    recipients.Add(recipientInfo);
                }

                Dictionary<string, string> smsDetails = new Dictionary<string, string>();
                smsDetails.Add("Code", defaultCode);
                smsDetails.Add("Message", message);

                PulseData pulseData = new PulseData();
                pulseData.Recipients = recipients;
                pulseData.Params = smsDetails;

                Pulse pulse = new Pulse();
                pulse.Data = pulseData;
                pulse.Name = Util.GetPulseSMSTemplateName(tenantName);

                PulseHeader pulseHeader = new PulseHeader();
                pulseHeader.Type = RecipientType.SMS.ToString();

                _smsNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }
    }
}