using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.NoActionImpl.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions
{
    public class NoAction : IActionImpl
    {
        public RequestDirection GetRequestDirection => RequestDirection.NoFurtherAction;

        private readonly ITypeImplComposer _typeImpl;
        private readonly IEnumerable<IServiceNoActionImpl> _noActionImppl;

        public NoAction(ITypeImplComposer typeImpl, IEnumerable<IServiceNoActionImpl> noActionImppl)
        {
            _typeImpl = typeImpl;
            _noActionImppl = noActionImppl;
        }


        public RequestFlowVM MoveToNextDefinitionLevel(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel)
        {
            try
            {
                IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads = _typeImpl.GetRevenueHeadDetails(requestDeets.First().ServiceId, nextDefinedLevel.Id);
                if (serviceRevenueHeads == null || !serviceRevenueHeads.Any())
                {
                    throw new NoBillingInformationFoundException("No billing info found for service Id " + requestDeets.First().ServiceId + " in approval action");
                }

                //save service request
                _typeImpl.SaveServiceRequest(new PSSRequest { Id = requestDeets.First().Request.Id }, serviceRevenueHeads, requestDeets.First().ServiceId, requestDeets.First().InvoiceId, nextDefinedLevel.Id, PSSRequestStatus.Approved);

                //add log
                _typeImpl.AddRequestStatusLog(new RequestStatusLog
                {
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                    Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                    Status = (int)PSSRequestStatus.Approved,
                    StatusDescription = nextDefinedLevel.PositionDescription,
                    Invoice = new Invoice { Id = requestDeets.First().InvoiceId },
                    Fulfilled = true,
                });

                //update request command workflow log
                _typeImpl.SetPreviousRequestCommandWorkflowLogsToInactive(requestDeets.First().Request.Id);

                //add request command workflow log
                _typeImpl.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog
                {
                    Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                    Command = new Command { Id = requestDeets.First().CommandId },
                    DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                    IsActive = true,
                    RequestPhaseId = (int)RequestPhase.New,
                    RequestPhaseName = nameof(RequestPhase.New)
                });

                //set approval number
                string approvalNumber = Util.ZeroPadUp(requestDeets.First().Request.Id.ToString(), 10, $"PSS{DateTime.Now.ToString("MMdd")}");
                _typeImpl.SetApprovalNumber(requestDeets.First().Request.Id, approvalNumber);

                //move the request to the next defined level
                _typeImpl.UpdateRequestDefinitionFlowLevel(requestDeets.First().Request.Id, nextDefinedLevel.Id, PSSRequestStatus.Approved);

                //check for additional custom implementation
                foreach (var impl in _noActionImppl)
                {
                    if (impl.GetServiceType == requestDeets.First().ServiceType)
                    {
                        RequestFlowVM requestFlowVM = impl.DoServiceImplementationWorkForNoAction(requestDeets, approvalNumber);
                        SendNofications(requestDeets, approvalNumber);
                        return requestFlowVM;
                    }
                }

                throw new NoBillingInformationFoundException("No action implementation info found for service Id " + requestDeets.First().ServiceId + " in no further action");
            }
            catch (Exception)
            {
                _typeImpl.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Send notifications
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="approvalNumber"></param>
        private void SendNofications(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, string approvalNumber)
        {
            try
            {
                //Notify the requester of the service by email
                SendEmailsNotification(requestDeets.First(), approvalNumber);
                //Notify the requester of the service by sms
                SendSMSNotification(requestDeets.First());
            }
            catch (Exception) { }
        }


        /// <summary>
        /// Send SMS notification
        /// </summary>
        /// <param name="requestDeets"></param>
        private void SendSMSNotification(PSServiceRequestInvoiceValidationDTO requestDeets)
        {
            dynamic smsDetails = new ExpandoObject();
            smsDetails.PhoneNumber = requestDeets.PhoneNumber;
            smsDetails.RequestType = requestDeets.ServiceName;
            smsDetails.Name = requestDeets.Recipient;
            smsDetails.TaxEntityId = requestDeets.TaxEntityId;
            smsDetails.FileNumber = requestDeets.Request.FileRefNumber;
            _typeImpl.SendPSSRequestApprovalSMSNotification(smsDetails);
        }


        /// <summary>
        /// Send Email notification
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="approvalNumber"></param>
        private void SendEmailsNotification(PSServiceRequestInvoiceValidationDTO requestDeets, string approvalNumber)
        {
            dynamic emailDetails = new ExpandoObject();
            emailDetails.Email = requestDeets.Email;
            emailDetails.ApprovalStatus = (int)PSSRequestStatus.Approved;
            emailDetails.Recipient = requestDeets.Recipient;
            emailDetails.taxEntityId = requestDeets.TaxEntityId;
            emailDetails.Subject = "Possap Approval Notification";
            emailDetails.InvoiceNumber = requestDeets.InvoiceNumber;
            emailDetails.RequestType = requestDeets.ServiceName;
            emailDetails.ServiceTypeId = (int)requestDeets.ServiceType;
            emailDetails.ApprovalNumber = approvalNumber;
            emailDetails.RequestDate = requestDeets.Request.CreatedAtUtc.ToString("dd MMM yyyy");
            _typeImpl.SendEmailNotification(emailDetails);
        }


    }
}