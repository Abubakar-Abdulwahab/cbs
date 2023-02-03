using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSServiceRequestFlowApproverStagingManager<PSServiceRequestFlowApproverStaging> : IDependency, IBaseManager<PSServiceRequestFlowApproverStaging>
    {
        /// <summary>
        /// Sets IsDeleted to true for the removed approver in PSServiceRequestFlowApprover
        /// </summary>
        /// <param name="reference"></param>
        void UpdateServiceRequestFlowApproverFromStaging(string reference);
    }
}
