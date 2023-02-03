using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalWorkFlow.Contracts
{
    public interface IEscortApprovalWorkFlow : IDependency
    {

        /// <summary>
        /// Do partial validation
        /// </summary>
        /// <param name="approverId"></param>
        /// <param name="commandTypeId"></param>
        /// <param name="requestId"></param>
        /// <returns>List{EscortViewRubricDTO}</returns>
        List<EscortViewRubricDTO> DoProcessLevelValidation(int approverId, int commandTypeId, long requestId);

        /// <summary>
        /// Get view permissions
        /// </summary>
        /// <param name="rubricPermissions"></param>
        /// <returns>List{EscortApprovalViewPermissions}</returns>
        List<EscortApprovalViewPermissions> GetPermissions(List<EscortViewRubricDTO> rubricPermissions);

    }
}
