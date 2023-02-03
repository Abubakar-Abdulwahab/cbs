using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreThirdPartyPaymentNotificationService : IDependency
    {
        /// <summary>
        /// Persist payment notification for task to notify a third party system
        /// </summary>
        /// <param name="model"></param>
        void SaveNotification(ThirdPartyPaymentNotification model);
    }
}
