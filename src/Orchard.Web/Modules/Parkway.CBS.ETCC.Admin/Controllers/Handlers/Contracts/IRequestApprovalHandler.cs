using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts
{
    public interface IRequestApprovalHandler : IDependency
    {
        /// <summary>
        /// Save request approval details
        /// </summary>
        /// <param name="requestDetailVM"></param>
        /// <returns>bool</returns>
        bool ProcessRequestApproval(TCCRequestDetailVM requestDetailVM, ref List<ErrorModel> errors);

        /// <summary>
        /// Save request rejection details
        /// </summary>
        /// <param name="requestDetailVM"></param>
        /// <returns>bool</returns>
        bool ProcessRequestRejection(TCCRequestDetailVM requestDetailVM, ref List<ErrorModel> errors);

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="CanApproveTCCRequests"></param>
        void CheckForPermission(Permission CanApproveTCCRequests);

    }
}
