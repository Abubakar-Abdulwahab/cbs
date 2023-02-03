using Orchard;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IRequestApprovalHandler : IDependency
    {

        /// <summary>
        /// Get the Extract and Escort view details using request id and service type id,
        /// It returns dynamic object
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="serviceTypeId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        PSSRequestDetailsVM GetServiceRequestViewDetailsForView(long requestId);


        /// <summary>
        /// Get the view details for approval using request id and service type id,
        /// It returns dynamic object
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="adminUserId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        PSSRequestDetailsVM GetServiceRequestViewDetailsForApproval(long requestId, int adminUserId);


        /// <summary>
        /// Process request approval details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>RequestApprovalResponse</returns>
        RequestApprovalResponse ProcessRequestApproval(long requestId, ref List<ErrorModel> errors, dynamic userInput);


        /// <summary>
        /// Get police rank details using the rank id
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPoliceRank(int rankId);


        /// <summary>
        /// Do process for request rejection
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="errors"></param>
        /// <param name="comment"></param>
        /// <param name="id"></param>
        /// <returns>RequestApprovalResponse</returns>
        RequestApprovalResponse ProcessRequestRejection(long requestId, ref List<ErrorModel> errors, string comment, int adminUserId);
    }
}
