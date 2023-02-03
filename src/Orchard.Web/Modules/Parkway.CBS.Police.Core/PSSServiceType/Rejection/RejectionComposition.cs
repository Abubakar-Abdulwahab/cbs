using System;
using System.Linq;
using Orchard.Logging;
using Orchard.Users.Models;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Rejection.Contracts;
using System.Dynamic;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;

namespace Parkway.CBS.Police.Core.PSSServiceType.Rejection
{
    public class RejectionComposition : IRejectionComposition
    {
        private readonly IPoliceServiceRequestManager<PoliceServiceRequest> _serviceRequestManager;
        private readonly IPSSRequestManager<PSSRequest> _requestManager;
        private readonly IPSSRequestApprovalLogManager<PSSRequestApprovalLog> _requestApprovalLogRepo;
        private readonly ITypeImplComposer _typeImpl;
        public ILogger Logger { get; set; }


        public RejectionComposition(IPoliceServiceRequestManager<PoliceServiceRequest> serviceRequestManager, IPSSRequestApprovalLogManager<PSSRequestApprovalLog> requestApprovalLogRepo, IPSSRequestManager<PSSRequest> requestManager, ITypeImplComposer typeImpl)
        {
            _serviceRequestManager = serviceRequestManager;
            _requestApprovalLogRepo = requestApprovalLogRepo;
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
            _typeImpl = typeImpl;
        }


        /// <summary>
        /// Process request rejections
        /// </summary>
        /// <param name="requestDetails"></param>
        /// <param name="adminUserId"></param>
        /// <returns>RequestApprovalResponse</returns>
        public RequestApprovalResponse ProcessRequestRejection(GenericRequestDetails requestDetails, int adminUserId)
        {
            try
            {
                //now that validation has passed we need to set the essentials for this stage we are in
                //get the stage request details, set the request status and update/add the request log
                IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets = GetServiceRequestDetailsWithRequestId(requestDetails.RequestId);
                if (requestDeets == null || !requestDeets.Any())
                {
                    throw new NoRecordFoundException("No service request details found for this request " + requestDetails.RequestId);
                }

                //we need to update the service request
                UpdateServiceRequestStatusForThisRequestStage(requestDeets.First().Request.Id, requestDeets.First().DefinitionLevelId, requestDeets.First().ServiceId, PSSRequestStatus.Rejected, requestDeets.First().InvoiceId);

                //add approval log
                PSSRequestApprovalLog approvalLog = new PSSRequestApprovalLog
                {
                    AddedByAdminUser = new UserPartRecord { Id = adminUserId },
                    Comment = requestDetails.ApproverComment,
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = requestDeets.First().DefinitionLevelId },
                    Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                    Status = (int)PSSRequestStatus.Rejected,
                };

                SaveRequestApprovalLog(approvalLog);

                SetRequestStatus(requestDeets.First().Request.Id, PSSRequestStatus.Rejected);

                //Send an email notification to the requester of the service
                SendEmailNotification(requestDeets.First(), requestDetails.ServiceRequests.First().ServiceName, requestDetails.ApproverComment);

                //Notify the requester of the service by sms
                SendSMSNotification(requestDeets.First(), requestDetails.ServiceRequests.First().ServiceName, requestDetails.ApproverComment);

                return new RequestApprovalResponse
                {
                    ServiceType = requestDeets.First().ServiceType.ToString(),
                    FileNumber = requestDeets.First().Request.FileRefNumber,
                    CustomerName = requestDetails.TaxEntity.Recipient,
                    NotificationMessage = string.Format("{0} request with File Number {1} for {2} has been rejected successfully.", requestDeets.First().ServiceType.ToString(), requestDeets.First().Request.FileRefNumber, requestDetails.TaxEntity.Recipient)
                };

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }

        private void SendSMSNotification(PSServiceRequestInvoiceValidationDTO requestDeets, string serviceName, string comments)
        {
            try
            {
                dynamic smsDetails = new ExpandoObject();
                smsDetails.PhoneNumber = requestDeets.PhoneNumber;
                smsDetails.RequestType = serviceName;
                smsDetails.Name = requestDeets.Recipient;
                smsDetails.TaxEntityId = requestDeets.TaxEntityId;
                smsDetails.FileNumber = requestDeets.Request.FileRefNumber;
                smsDetails.Comment = comments;
                _typeImpl.SendPSSRequestRejectionSMSNotification(smsDetails);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception sending SMS notification {0}", exception.Message));
            }
        }

        private void SendEmailNotification(PSServiceRequestInvoiceValidationDTO requestDeets, string serviceName, string comments)
        {
            try
            {
                dynamic emailDetails = new ExpandoObject();
                emailDetails.Email = requestDeets.Email;
                emailDetails.ApprovalStatus = (int)PSSRequestStatus.Rejected;
                emailDetails.Recipient = requestDeets.Recipient;
                emailDetails.taxEntityId = requestDeets.TaxEntityId;
                emailDetails.Subject = "Possap Rejection Notification";
                emailDetails.InvoiceNumber = requestDeets.InvoiceNumber;
                emailDetails.RequestType = serviceName;
                emailDetails.RequestDate = requestDeets.Request.CreatedAtUtc.ToString("dd MMM yyyy");
                emailDetails.Comment = comments;
                emailDetails.ServiceTypeId = (int)requestDeets.ServiceType;
                _typeImpl.SendEmailNotification(emailDetails);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception sending Email notification {0}", exception.Message));
            }
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
        /// Save request approval log
        /// </summary>
        /// <param name="approvalLog"></param>
        /// <exception cref="CouldNotSaveRecord">If insert fails</exception>
        public void SaveRequestApprovalLog(PSSRequestApprovalLog approvalLog)
        {
            if (!_requestApprovalLogRepo.Save(approvalLog))
            {
                throw new CouldNotSaveRecord();
            }
        }


        /// <summary>
        /// Roll back transaction
        /// </summary>
        public void RollBackAllTransactions()
        {
            _serviceRequestManager.RollBackAllTransactions();
        }


        /// <summary>
        /// Set request status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approved"></param>
        private void SetRequestStatus(long requestId, PSSRequestStatus status)
        {
            if (!_requestManager.SetRequestStatus(requestId, status))
            {
                throw new CouldNotSaveRecord();
            }
        }

    }
}