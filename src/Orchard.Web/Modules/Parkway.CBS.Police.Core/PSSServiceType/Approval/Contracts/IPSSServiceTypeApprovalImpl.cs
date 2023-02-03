using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts
{
    public interface IPSSServiceTypeApprovalImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceTypeDefinition { get; }


        /// <summary>
        /// Get the view details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        PSSRequestDetailsVM GetServiceRequestViewDetails(long requestId);


        /// <summary>
        /// Get the view details for approval using request id for approvals
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="adminUserId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        PSSRequestDetailsVM GetServiceRequestViewDetailsForApproval(long requestId, int adminUserId);


        /// <summary>
        /// First we validate, if validation is correct, then we process to process approval
        /// If validation fails we return with a list of errors in the errors model object
        /// </summary>
        /// <param name="requestDetails"></param>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>RequestApprovalResponse</returns>
        RequestApprovalResponse ValidatedAndProcessRequestApproval(GenericRequestDetails requestDetails, ref List<ErrorModel> errors, dynamic userInput);
    }
}
