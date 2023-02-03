using InfoGRID.Pulse.SDK.DTO;

namespace Parkway.CBS.HangFireInterface.Notification.Contracts
{
    public interface ISMSNotification
    {
        /// <summary>
        /// Send SMS notification using Pulse notification system
        /// </summary>
        /// <param name="pulse"></param>
        /// <param name="pulseHeader"></param>
        /// <returns></returns>
        bool SendNotificationPulse(Pulse pulse, PulseHeader pulseHeader);
    }
}
