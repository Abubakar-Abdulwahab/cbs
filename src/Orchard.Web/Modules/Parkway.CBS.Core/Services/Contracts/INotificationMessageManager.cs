using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface INotificationMessageManager<NotificationMessage> : IDependency, IBaseManager<NotificationMessage>
    {
    }
}
