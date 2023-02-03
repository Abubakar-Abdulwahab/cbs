using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IContactUsHandler : IDependency
    {
        /// <summary>
        /// Sends an email to the configured support email
        /// </summary>
        /// <param name="messageModel"></param>
        /// <returns></returns>
        bool SendContactUsRequest(dynamic messageModel);
    }
}
