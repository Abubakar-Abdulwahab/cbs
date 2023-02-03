using InfoGRID.Pulse.SDK.DTO;
using InfoGRID.Pulse.SDK.Models;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Notification;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.SMS.Provider
{
    public class PulseProvider : ISMSProvider
    {
        public SMSProvider GetSMSNotificationProvider => SMSProvider.Pulse;
        private INotificationMessageManager<NotificationMessage> _notification;
        private INotificationMessageItemsManager<NotificationMessageItems> _notificationItems;
        private ISMSNotification _smsNotification;
        const string defaultCode = "+234";
        public ILogger Logger { get; set; }

        public PulseProvider(INotificationMessageItemsManager<NotificationMessageItems> notificationItems, INotificationMessageManager<NotificationMessage> notification)
        {
            _notification = notification;
            _notificationItems = notificationItems;
            _smsNotification = new SMSNotification();
            Logger = NullLogger.Instance;
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
                if(phoneNumbers.Count == 0 || string.IsNullOrEmpty(message))
                {
                    throw new Exception($"Phone number or message is empty");
                }

                List<RecipientInfo> recipients = new List<RecipientInfo>();
                foreach(string phoneNumber in phoneNumbers)
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