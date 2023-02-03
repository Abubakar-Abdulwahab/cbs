using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IApprovalAccessListStagingManager<ApprovalAccessListStaging> : IDependency, IBaseManager<ApprovalAccessListStaging>
    {
        /// <summary>
        /// Sets IsDeleted to true for the removed commands in ApprovalAccessList
        /// </summary>
        /// <param name="reference"></param>
        void UpdateApprovalAccessListFromStaging(string reference);
    }
}
    