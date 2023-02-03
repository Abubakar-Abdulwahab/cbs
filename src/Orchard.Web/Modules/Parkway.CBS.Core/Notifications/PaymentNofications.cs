using Newtonsoft.Json;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Notifications.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Notification;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Notifications
{
    public class PaymentNofications : IPaymentNotifications
    {
        private readonly ICoreThirdPartyPaymentNotificationService _coreThirdPartyPaymentNotificationService;
        public ILogger Logger { get; set; }
        private readonly IHangfireNotification _hangfireNotification;
        private readonly IHangfireJobReferenceManager<HangfireJobReference> _hangfireJobReferenceManager;


        public PaymentNofications(ICoreThirdPartyPaymentNotificationService coreThirdPartyPaymentNotificationService, IHangfireJobReferenceManager<HangfireJobReference> hangfireJobReferenceManager)
        {
            _coreThirdPartyPaymentNotificationService = coreThirdPartyPaymentNotificationService;
            Logger = NullLogger.Instance;
            _hangfireNotification = new APINotification();
            _hangfireJobReferenceManager = hangfireJobReferenceManager;
        }


        /// <summary>
        /// Send a payment notification to the given callback URL
        /// <para>Would return true if 200, else false</para>
        /// </summary>
        /// <param name="transactionLog"></param>
        /// <param name="callBackURL"></param>
        /// <param name="siteName"></param>
        public void SendPaymentNotification(TransactionLogVM transactionLog, string callBackURL, string siteName, string messageEncryptionKey, string requestReference)
        {
            string value = string.Format("{0}{1}{2}{3}", transactionLog.InvoiceNumber, transactionLog.PaymentReference, transactionLog.AmountPaid.ToString("F"), requestReference);
            var hashValue = Util.HMACHash256(value, messageEncryptionKey);

            PaymentNotification paymentNotification = new PaymentNotification
            {
                InvoiceNumber = transactionLog.InvoiceNumber,
                PaymentRef = transactionLog.PaymentReference,
                PaymentDate = transactionLog.PaymentDate.ToString("dd'/'MM'/'yyyy HH:mm:ss"),
                BankCode = transactionLog.BankCode,
                BankBranch = transactionLog.BankBranch,
                AmountPaid = Math.Round(transactionLog.AmountPaid, 2) + 0.00m,
                BankName = transactionLog.Bank,
                Channel = ((PaymentChannel)transactionLog.Channel).ToString(),
                TransactionDate = transactionLog.TransactionDate.Value.ToString("dd'/'MM'/'yyyy HH:mm:ss"),
                Mac = hashValue,
                ResponseCode = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.PaymentNotificationResponseCode),
                ResponseMessage = Lang.Lang.remitapaymentnotificationok.ToString(),
                RequestReference = requestReference,
                PaymentMethod = transactionLog.PaymentMethod,
                PaymentProvider = ((PaymentProvider)transactionLog.PaymentProvider).ToString(),
                IsReversal = transactionLog.IsReversal,                
            };
            string sModel = JsonConvert.SerializeObject(paymentNotification);
            Logger.Information("Sending payment notification for model : " + sModel);
            bool addedToTaskTable = false;
            //Queue the job to hangfire
            try
            {
                IHangfireNotification hangfireNotification = new APINotification();
                string scheduleJobId = hangfireNotification.ScheduleNewNotification(siteName, sModel, callBackURL, new Dictionary<string, string> { });

                if (!string.IsNullOrEmpty(scheduleJobId))
                {
                    if (!_hangfireJobReferenceManager.Save(new HangfireJobReference { HangfireJobId = scheduleJobId , JobReferenceNumber = transactionLog.InvoiceNumber }))
                    { Logger.Error(string.Format("Cannot save hangfire job reference details for invoice number {0} and job id {1}", transactionLog.InvoiceNumber, scheduleJobId)); }
                }

                addedToTaskTable = true;
                Logger.Information(String.Format("PAYMENT NOTIFICATION SENT {0}", addedToTaskTable.ToString()));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, callBackURL + " PAYMENT NOTIFICATION ERR: " + exception.Message);
                addedToTaskTable = false;
            }
        }



        /// <summary>
        /// Send a payment notification to the given callback URL without HangFire job
        /// Would return true if 200, else false
        /// </summary>
        /// <param name="transactionLog"></param>
        /// <param name="callBackURL"></param>
        /// <param name="messageEncryptionKey"></param>
        /// <param name="requestReference"></param>
        public void SendPaymentNotification(TransactionLogVM transactionLog, string callBackURL, string messageEncryptionKey, string requestReference)
        {
            string value = string.Format("{0}{1}{2}{3}", transactionLog.InvoiceNumber, transactionLog.PaymentReference, transactionLog.AmountPaid.ToString("F"), requestReference);
            var hashValue = Util.HMACHash256(value, messageEncryptionKey);

            Logger.Information(string.Format("Sending payment notification. url {0}, ref {1}, value {2}, hash val {3} ", callBackURL, requestReference, value, hashValue));

            PaymentNotification paymentNotification = new PaymentNotification
            {
                InvoiceNumber = transactionLog.InvoiceNumber,
                PaymentRef = transactionLog.PaymentReference,
                PaymentDate = transactionLog.PaymentDate.ToString("dd'/'MM'/'yyyy HH:mm:ss"),
                BankCode = transactionLog.BankCode,
                BankBranch = transactionLog.BankBranch,
                AmountPaid = Math.Round(transactionLog.AmountPaid, 2) + 0.00m,
                BankName = transactionLog.Bank,
                Channel = ((PaymentChannel)transactionLog.Channel).ToString(),
                TransactionDate = transactionLog.TransactionDate.Value.ToString("dd'/'MM'/'yyyy HH:mm:ss"),
                Mac = hashValue,
                ResponseCode = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.PaymentNotificationResponseCode),
                ResponseMessage = Lang.Lang.remitapaymentnotificationok.ToString(),
                RequestReference = requestReference,
                PaymentMethod = transactionLog.PaymentMethod,
                PaymentProvider = ((PaymentProvider)transactionLog.PaymentProvider).ToString(),
                IsReversal = transactionLog.IsReversal,
            };
            string sModel = JsonConvert.SerializeObject(paymentNotification);
            Logger.Information("Sending payment notification for model : " + sModel);

            IHangfireNotification hangfireNotification = new APINotification();
            bool isSent = hangfireNotification.ScheduleNewNotification(sModel, callBackURL, new Dictionary<string, string> { });
            Logger.Information("Payment notification response ::: Sent=> " + isSent);
        }

    }
}