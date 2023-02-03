using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IRequestApprovalLogHandler : IDependency
    {
        /// <summary>
        /// Get request approval log vm for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        RequestApprovalLogVM GetRequestApprovalLogVMByRequestId(long requestId);
    }
}
