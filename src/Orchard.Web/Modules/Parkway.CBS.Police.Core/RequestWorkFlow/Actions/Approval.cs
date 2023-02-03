using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.ApprovalImpl.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions
{
    public class AnotherApproval : IActionImpl
    {
        public RequestDirection GetRequestDirection => RequestDirection.Approval;

        private readonly ITypeImplComposer _typeImpl;
        private readonly IEnumerable<IServiceApprovalImpl> _approvalImpl;

        public AnotherApproval(ITypeImplComposer typeImpl, IEnumerable<IServiceApprovalImpl> approvalImpl)
        {
            _typeImpl = typeImpl;
            _approvalImpl = approvalImpl;
        }


        /// <summary>
        /// Move the request to the next stage for approval
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="nextDefinitionLevelId"></param>
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
                _typeImpl.SaveServiceRequest(new PSSRequest { Id = requestDeets.First().Request.Id }, serviceRevenueHeads, requestDeets.First().ServiceId, requestDeets.First().InvoiceId, nextDefinedLevel.Id, PSSRequestStatus.PendingApproval);
                //add log
                _typeImpl.AddRequestStatusLog(new RequestStatusLog
                {
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                    Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                    Status = (int)PSSRequestStatus.PendingApproval,
                    StatusDescription = nextDefinedLevel.PositionDescription,
                    Invoice = new CBS.Core.Models.Invoice { Id = requestDeets.First().InvoiceId }
                });

                //move the request to the next defined level
                _typeImpl.UpdateRequestDefinitionFlowLevel(requestDeets.First().Request.Id, nextDefinedLevel.Id, PSSRequestStatus.PendingApproval);

                //check for additional custom implementation
                foreach (var impl in _approvalImpl)
                {
                    if (impl.GetServiceType == requestDeets.First().ServiceType)
                    {
                        impl.DoServiceImplementationWorkForApproval(requestDeets, nextDefinedLevel);
                    }
                }

                return new RequestFlowVM
                {
                    Message = string.Format("This request has been moved to another approval level as required by the defined workflow.")
                };
            }
            catch (Exception)
            {
                _typeImpl.RollBackAllTransactions();
                throw;
            }

        }
    }
}