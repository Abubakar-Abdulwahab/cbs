using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.PSSServiceType.Rejection.Contracts
{
    public interface IRejectionComposition : IDependency
    {

        /// <summary>
        /// Process request rejections
        /// </summary>
        /// <param name="requestDetails"></param>
        /// <param name="adminUserId"></param>
        /// <returns>RequestApprovalResponse</returns>
        RequestApprovalResponse ProcessRequestRejection(GenericRequestDetails requestDetails, int adminUserId);

    }
}
