using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSRequestApprovalLogManager<PSSRequestApprovalLog> : IDependency, IBaseManager<PSSRequestApprovalLog>
    {
        /// <summary>
        /// Get approval logs for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        List<ApprovalLogVM> GetApprovalLogForRequestById(long requestId);
    }

   
}
