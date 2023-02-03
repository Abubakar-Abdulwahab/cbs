using System;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval
{
    public class Generic : IPSSServiceTypeApprovalImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.GenericPoliceServices;

        private readonly IApprovalComposition _approvalCompositionHandler;
        public ILogger Logger { get; set; }
        private readonly IPSSRequestManager<PSSRequest> _requestManager;

        public Generic(IApprovalComposition approvalCompositionHandler, IPSSRequestManager<PSSRequest> requestManager)
        {
            _approvalCompositionHandler = approvalCompositionHandler;
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
        }



        public GenericRequestDetailsVM GetGenericDetails(long requestId)
        {
            GenericRequestDetailsVM serviceDetails = _approvalCompositionHandler.GetServiceRequestDetailsForGenericWithRequestId(requestId);
            if (serviceDetails == null)
            { throw new NoInvoicesMatchingTheParametersFoundException("404 for PSS request. Request Id " + requestId); }
            serviceDetails.ViewName = "PSSGenericDetails";
            serviceDetails.FormControlValues = _requestManager.GetFormDetails(requestId).ToList();
            return serviceDetails;
        }


        /// <summary>
        /// Get the generic view details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetails(long requestId)
        {
            GenericRequestDetailsVM serviceDetails = GetGenericDetails(requestId);
            PSSRequestDetailsVM returnObj = serviceDetails;
            returnObj.ServiceVM = serviceDetails;

            return returnObj;
        }


        /// <summary>
        /// Get the generic view details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetailsForApproval(long requestId, int adminUserId)
        {
            GenericRequestDetailsVM serviceDetails = GetGenericDetails(requestId);
            serviceDetails.DisplayDetailsForApproval = true;
            PSSRequestDetailsVM returnObj = serviceDetails;
            returnObj.ServiceVM = serviceDetails;

            return returnObj;
        }


        /// <summary>
        /// First we validate, if validation is correct, then we process to process approval
        /// If validation fails we return with a list of errors in the errors model object
        /// </summary>
        /// <param name="requestDetails"></param>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>RequestApprovalResponse</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public RequestApprovalResponse ValidatedAndProcessRequestApproval(GenericRequestDetails requestDetails, ref List<ErrorModel> errors, dynamic userInput)
        {
            try
            {
                GenericRequestDetailsVM objUserInput = (GenericRequestDetailsVM)userInput;
                //do validation
                if (objUserInput == null)
                {
                    throw new NoRecordFoundException("No record found for extract with request Id " + requestDetails.RequestId);
                }
                return _approvalCompositionHandler.ProcessRequestApproval(requestDetails, objUserInput.ApproverId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

       
    }
}