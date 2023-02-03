using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.Mail.Provider.Contracts
{
    public interface IPSSEmailProvider : IDependency
    {
        EmailProvider GetEmailNotificationProvider { get; }

        /// <summary>
        /// Sends an email with details filled by user on the contact us page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ContactUsNotification(dynamic model);

        /// <summary>
        /// Sends an email to admin user after registration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool PSSAdminUserRegistrationNotification(dynamic model);

        /// <summary>
        /// Sends an email containing random generated password to user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool PSSSubUserPasswordNotification(dynamic model);

        /// <summary>
        /// Send an approval notification for a particular request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool PSSRequestApproval(dynamic model);

        /// <summary>
        /// Send a reject notification for a particular request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool PSSRequestRejection(dynamic model);
    }
}
