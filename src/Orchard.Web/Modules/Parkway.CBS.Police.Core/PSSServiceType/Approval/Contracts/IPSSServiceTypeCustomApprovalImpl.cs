using Orchard;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts
{
    public interface IPSSServiceTypeCustomApprovalImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceTypeDefinition { get; }

        /// <summary>
        /// Performs approval logic
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userPartId"></param>
        EscortApprovalMessage EscortApproval(long requestId, int userPartId);
    }
}
