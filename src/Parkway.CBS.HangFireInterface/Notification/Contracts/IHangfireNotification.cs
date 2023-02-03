using System.Collections.Generic;

namespace Parkway.CBS.HangFireInterface.Notification.Contracts
{
    public interface IHangfireNotification
    {
        /// <summary>
        /// Send notification using Hangfire
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="model"></param>
        /// <param name="callBackURL"></param>
        /// <param name="queryStringParameters"></param>
        /// <returns></returns>
        string ScheduleNewNotification(string tenantName, string model, string callBackURL, Dictionary<string, string> queryStringParameters);

        /// <summary>
        /// Send notification without using Hangfire
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callBackURL"></param>
        /// <param name="queryStringParameters"></param>
        /// <returns></returns>
        bool ScheduleNewNotification(string model, string callBackURL, Dictionary<string, string> queryStringParameters);

    }
}
