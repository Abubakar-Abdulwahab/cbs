using Orchard;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts
{
    public interface IEscortApprovalComposition : IDependency
    {

        /// <summary>
        /// Get details for the service revenue head associated with this request Id at the give process stage of the request
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        IEnumerable<PSServiceRequestInvoiceValidationDTO> GetServiceRequestDetailsWithRequestId(long requestId);


        /// <summary>
        /// Update the service request entry with the give status for the given invoice
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="oldDefinitionLevelId"></param>
        /// <param name="serviceId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="status"></param>
        /// <exception cref="Exception">Throws exception if update fails</exception>
        void UpdateServiceRequestStatusForThisRequestStage(long requestId, int oldDefinitionLevelId, int serviceId, PSSRequestStatus status, Int64 invoiceId);


        /// <summary>
        /// Set the fulfilled flag on the request status log to fulfilled
        /// when the request action has been met
        /// </summary>
        /// <param name="id"></param>
        /// <param name="definitionLevelId"></param>
        /// <param name="invoiceId"></param>
        void SetFulfilledFlagToTrue(long id, int definitionLevelId, long invoiceId);


        /// <summary>
        /// Get generic request details for this request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>GenericRequestDetailsVM</returns>
        GenericRequestDetailsVM GetServiceRequestDetailsForGenericWithRequestId(long requestId);


        /// <summary>
        /// Check if this request has any pending invoice confirmations
        /// </summary>
        /// <param name="entireServiceRequest"></param>
        /// <param name="invoiceNumberServiceRequestGrp"></param>
        bool CheckIfRequestHasPendingConfirmations(IEnumerable<PSServiceRequestInvoiceValidationDTO> entireServiceRequest, IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberServiceRequestGrp);


        /// <summary>
        /// Move request to the next defined level/stage
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <returns>RequestFlowVM</returns>
        RequestFlowVM MoveRequestToNextDefinitionLevel(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets);


        /// <summary>
        /// Roll back transaction
        /// </summary>
        void RollBackAllTransactions();


        /// <summary>
        /// Process escort approval for request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="adminUserId"></param>
        /// <returns>EscortApprovalMessage</returns>
        EscortApprovalMessage ProcessEscortRequestApproval(long requestId, int adminUserId);

    }
}
