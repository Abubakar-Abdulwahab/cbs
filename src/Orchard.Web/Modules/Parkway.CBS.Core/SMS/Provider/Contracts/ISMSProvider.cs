using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.SMS.Provider.Contracts
{
    public interface ISMSProvider : IDependency
    {
        SMSProvider GetSMSNotificationProvider { get; }

        /// <summary>
        /// Send sms to a list of users
        /// </summary>
        /// <param name="phoneNumbers"></param>
        /// <param name="message"></param>
        /// <param name="tenantName"></param>
        /// <returns>bool</returns>
        bool SendSMS(List<string> phoneNumbers, string message, string tenantName);

    }
}
