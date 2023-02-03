using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Net;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class Generic : IPSSServiceTypeUSSDApprovalImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.GenericPoliceServices;
        public ILogger Logger { get; set; }
        private readonly Lazy<IUSSDRequestApprovalHandler> _requestApprovalHandler;

        public Generic(Lazy<IUSSDRequestApprovalHandler> requestApprovalHandler)
        {
            Logger = NullLogger.Instance;
            _requestApprovalHandler = requestApprovalHandler;
        }

        /// <summary>
        /// This handles processing of a request that has to do with generic
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        public USSDAPIResponse ProcessRequest(USSDRequestModel model)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                string phoneNumber = model.PhoneNumber;
                int startIndex = 4;
                string trimPhoneNumber = $"0{phoneNumber.Substring(startIndex)}";
                string[] requestStage = model.Text.Split('|');
                int operationTypeId = 0;
                bool parsed = int.TryParse(requestStage[2], out operationTypeId);
                if (!parsed)
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                string comment = requestStage[requestStage.Length - 1];
                RequestApprovalResponse approvalResponse = null;
                if (operationTypeId == (int)USSDOperationType.Decline)
                {
                    approvalResponse = _requestApprovalHandler.Value.ProcessRequestRejection(requestStage[1], ref errors, comment, trimPhoneNumber);
                    return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = approvalResponse.NotificationMessage };
                }

                GenericRequestDetailsVM detailsVM = new GenericRequestDetailsVM
                {
                    Comment = comment
                };
                approvalResponse = _requestApprovalHandler.Value.ProcessRequestApproval(requestStage[1], ref errors, detailsVM, trimPhoneNumber);
                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = approvalResponse.NotificationMessage };
            }
            catch (DirtyFormDataException)
            {
                throw;
            }
            catch (NoRecordFoundException)
            {
                throw;
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}