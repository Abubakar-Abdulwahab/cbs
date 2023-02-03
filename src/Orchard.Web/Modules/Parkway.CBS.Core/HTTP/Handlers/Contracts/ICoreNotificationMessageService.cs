using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreNotificationMessageService
    {
        /// <summary>
        /// Save new notification
        /// </summary>
        /// <param name="notificationMessage"></param>
        /// <returns>bool</returns>
        bool SaveNotificationMessage(NotificationMessage notificationMessage);
    }
}
