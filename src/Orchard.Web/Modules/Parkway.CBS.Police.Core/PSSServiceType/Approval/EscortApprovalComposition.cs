﻿using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval
{
    public class EscortApprovalComposition : IEscortApprovalComposition
    {
        private readonly IPoliceServiceRequestManager<PoliceServiceRequest> _serviceRequestManager;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly IRequestStatusLogManager<RequestStatusLog> _reqStatusLogRepo;
        private readonly IPSSRequestApprovalLogManager<PSSRequestApprovalLog> _requestApprovalLogRepo;
        private readonly IRequestFlowHandler _requestFlowHandlerImpl;
        public ILogger Logger { get; set; }


        public EscortApprovalComposition(IPoliceServiceRequestManager<PoliceServiceRequest> serviceRequestManager, IRequestStatusLogManager<RequestStatusLog> reqStatusLogRepo, IPSSRequestApprovalLogManager<PSSRequestApprovalLog> requestApprovalLogRepo, IRequestFlowHandler requestFlowHandlerImpl, Lazy<IPSSRequestManager<PSSRequest>> requestManager)
        {
            _serviceRequestManager = serviceRequestManager;
            _reqStatusLogRepo = reqStatusLogRepo;
            _requestApprovalLogRepo = requestApprovalLogRepo;
            _requestFlowHandlerImpl = requestFlowHandlerImpl;
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
        }


        /// <summary>
        /// Get details for the service revenue head associated with this request Id at the give process stage of the request
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        public IEnumerable<PSServiceRequestInvoiceValidationDTO> GetServiceRequestDetailsWithRequestId(long requestId)
        {
            return _serviceRequestManager.GetServiceRequestDetailsWithRequestId(requestId);
        }


        /// <summary>
        /// Update the service request entry with the give status for the given invoice
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="oldDefinitionLevelId"></param>
        /// <param name="serviceId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="status"></param>
        /// <exception cref="Exception">Throws exception if update fails</exception>
        public void UpdateServiceRequestStatusForThisRequestStage(long requestId, int oldDefinitionLevelId, int serviceId, PSSRequestStatus status, long invoiceId)
        {
            _serviceRequestManager.UpdateServiceRequestsStatus(requestId, oldDefinitionLevelId, serviceId, status, invoiceId);
        }


        /// <summary>
        /// Set the fulfilled flag on the request status log to fulfilled
        /// when the request action has been met
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="definitionLevelId"></param>
        /// <param name="invoiceId"></param>
        public void SetFulfilledFlagToTrue(long requestId, int definitionLevelId, long invoiceId)
        {
            _reqStatusLogRepo.UpdateStatusToFulfilledAfterPayment(requestId, definitionLevelId, invoiceId);
        }


        /// <summary>
        /// Get generic request details for this request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>GenericRequestDetailsVM</returns>
        public GenericRequestDetailsVM GetServiceRequestDetailsForGenericWithRequestId(long requestId)
        {
            return _serviceRequestManager.GetServiceRequestDetailsForGenericWithRequestId(requestId);
        }


        /// <summary>
        /// Check if this request has any pending invoice confirmations
        /// </summary>
        /// <param name="entireServiceRequest"></param>
        /// <param name="invoiceNumberServiceRequestGrp"></param>
        public bool CheckIfRequestHasPendingConfirmations(IEnumerable<PSServiceRequestInvoiceValidationDTO> entireServiceRequest, IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberServiceRequestGrp)
        {
            if (invoiceNumberServiceRequestGrp.Count() == entireServiceRequest.Count())
                return false;
            var remainingGRP = entireServiceRequest.Where(g => g.InvoiceNumber != invoiceNumberServiceRequestGrp.First().InvoiceNumber);

            int hasUnconfirmedStatusCount = remainingGRP.Where(i => ((i.ServiceRequestStatus != (int)PSSRequestStatus.Confirmed) && (i.ServiceRequestStatus != (int)PSSRequestStatus.Approved))).Count();

            if (hasUnconfirmedStatusCount > 0) { return true; }

            return false;
        }


        /// <summary>
        /// Move request to the next defined level/stage
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <returns>RequestFlowVM</returns>
        public RequestFlowVM MoveRequestToNextDefinitionLevel(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets)
        {
            return _requestFlowHandlerImpl.MoveRequestToNextStage(requestDeets);
        }


        /// <summary>
        /// Roll back transaction
        /// </summary>
        public void RollBackAllTransactions()
        {
            _reqStatusLogRepo.RollBackAllTransactions();
        }


        /// <summary>
        /// Process escort approval for request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="adminUserId"></param>
        /// <returns>EscortApprovalMessage</returns>
        public EscortApprovalMessage ProcessEscortRequestApproval(long requestId, int adminUserId)
        {
            try
            {
                //now that validation has passed we need to set the essentials for this stage we are in
                //get the stage request details, set the request status and update/add the request log
                IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets = GetServiceRequestDetailsWithRequestId(requestId);
                if (requestDeets == null || !requestDeets.Any())
                {
                    throw new NoRecordFoundException("No service request details found for this request " + requestId);
                }

                //we need to update the service request
                UpdateServiceRequestStatusForThisRequestStage(requestDeets.First().Request.Id, requestDeets.First().DefinitionLevelId, requestDeets.First().ServiceId, PSSRequestStatus.Approved, requestDeets.First().InvoiceId);

                //we need to request status log
                SetFulfilledFlagToTrue(requestDeets.First().Request.Id, requestDeets.First().DefinitionLevelId, requestDeets.First().InvoiceId);

                RequestFlowVM returnValue = null;
                //check if request can move
                if (!CheckIfRequestHasPendingConfirmations(requestDeets, requestDeets))
                {
                    //move request to next level
                    returnValue = MoveRequestToNextDefinitionLevel(requestDeets);
                }
                else
                {
                    returnValue = new RequestFlowVM { };
                }

                return new EscortApprovalMessage
                {
                    Message = string.Format("{0} request with File Number {1} for {2} approved successfully. {3}", requestDeets.First().ServiceType.ToString(), requestDeets.First().Request.FileRefNumber, requestDeets.First().Recipient, returnValue.Message),
                    CanApproveRequest = true
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }
    }
}