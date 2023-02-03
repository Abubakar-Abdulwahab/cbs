using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Notifications.Contracts
{
    public interface IPaymentNotifications : INotifications
    {

        /// <summary>
        /// Send a payment notification to the given callback URL with HangFire job
        /// <para>Would return true if 200, else false</para>
        /// </summary>
        /// <param name="transactionLog">TransactionLog</param>
        /// <param name="callBackURL"></param>
        /// <param name="siteName"></param>
        void SendPaymentNotification(TransactionLogVM transactionLog, string callBackURL, string siteName, string messageEncryptionKey, string requestReference);

        /// <summary>
        /// Send a payment notification to the given callback URL without HangFire job
        /// Would return true if 200, else false
        /// </summary>
        /// <param name="transactionLog">TransactionLog</param>
        /// <param name="callBackURL"></param>
        /// <param name="siteName"></param>
        void SendPaymentNotification(TransactionLogVM transactionLog, string callBackURL, string messageEncryptionKey, string requestReference);
    }
}
