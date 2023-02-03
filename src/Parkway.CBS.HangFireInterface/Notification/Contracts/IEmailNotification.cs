using InfoGRID.Pulse.SDK.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.HangFireInterface.Notification.Contracts
{
    public interface IEmailNotification
    {
        /// <summary>
        /// Use notification using Gmail notification system
        /// </summary>
        /// <param name="messageVM"></param>
        /// <param name="emailSmtp"></param>
        /// <returns></returns>
        bool SendNotificationGmail(string messageVM, Dictionary<string, string> data, string templateName);

        /// <summary>
        /// Send email notification using Pulse notification system
        /// </summary>
        /// <param name="pulse"></param>
        /// <param name="pulseHeader"></param>
        /// <returns></returns>
        bool SendNotificationPulse(Pulse pulse, PulseHeader pulseHeader);

    }
}
