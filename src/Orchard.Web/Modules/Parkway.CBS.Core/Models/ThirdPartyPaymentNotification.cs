using System;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Models
{
    public class ThirdPartyPaymentNotification : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TransactionLog TransactionLog { get; set; }

        /// <summary>
        /// I have no idea why this is here
        /// Update: maybe for web payments, yes?
        /// Update: Yes, for web payments. So lets say for example, eregistry makes a web payment,
        /// we might also want to send a notification to their API also to tell them about the payment. Bugzy suggested this,
        /// I think 
        /// </summary>
        public virtual PaymentNotificationChannel NotificationChannel { get; set; }

        //public virtual bool Sent { get; set; }

        //public virtual int RetryCount { get; set; }
    }
}