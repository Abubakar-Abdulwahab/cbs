using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Users.Events;

namespace Parkway.CBS.Core.Mail.Admin
{
    public interface IAdminMailService : IDependency
    {
        void SendActivationEmail(UserContext context);
    }
}
