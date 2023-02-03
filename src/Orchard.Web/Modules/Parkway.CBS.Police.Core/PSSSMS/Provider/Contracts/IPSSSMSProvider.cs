using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.PSSSMS.Provider.Contracts
{
    public interface IPSSSMSProvider : IDependency
    {
        SMSProvider GetSMSNotificationProvider { get; }

        /// <summary>
        /// Send an approval notification for a particular request
        /// </summary>
        /// <param name="smsDetailVM"></param>
        /// <returns></returns>
        bool PSSRequestApproval(dynamic smsDetailVM, string tenantName);

        /// <summary>
        /// Send a reject notification for a particular request
        /// </summary>
        /// <param name="smsDetailVM"></param>
        /// <returns></returns>
        bool PSSRequestRejection(dynamic smsDetailVM, string tenantName);

        /// <summary>
        /// Send a notification to the person that should approve a pending request
        /// </summary>
        /// <param name="smsDetailVM"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        bool NotifyApprover(dynamic smsDetailVM, List<string> phoneNumbers, string tenantName);

        /// <summary>
        /// Send sms to a list of users
        /// </summary>
        /// <param name="phoneNumbers"></param>
        /// <param name="message"></param>
        /// <returns>bool</returns>
        bool SendSMS(List<string> phoneNumbers, string message, string tenantName);
    }
}
