using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Net;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class Escort : IPSSServiceTypeUSSDApprovalImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Escort;
        public ILogger Logger { get; set; }
        private readonly Lazy<IUSSDRequestApprovalHandler> _requestApprovalHandler;
        private readonly Lazy<IPoliceOfficerManager<PoliceOfficer>> _policeOfficerRepo;

        public Escort(Lazy<IUSSDRequestApprovalHandler> requestApprovalHandler, Lazy<IPoliceOfficerManager<PoliceOfficer>> policeOfficerRepo)
        {
            Logger = NullLogger.Instance;
            _requestApprovalHandler = requestApprovalHandler;
            _policeOfficerRepo = policeOfficerRepo;
        }

        /// <summary>
        /// This handles processing of a request that has to do with escort
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        public USSDAPIResponse ProcessRequest(USSDRequestModel model)
        {
            string[] requestStage = model.Text.Split('|');

            if(requestStage.Length == (int)USSDProcessingStage.OperationType)
            {
                bool parsed = int.TryParse(requestStage[2], out int operationTypeId);
                if (!parsed)
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                parsed = Enum.IsDefined(typeof(USSDOperationType), operationTypeId);
                if (!parsed)
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                if (operationTypeId == (int)USSDOperationType.Decline)
                {
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Comment (Minimum of ten characters)\n" };
                }
                return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = EscortOfficerSelection(model) };
            }

            return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = ProcessRequestApproval(model) };
        }

        /// <summary>
        /// This handles checking to determine what next for escort service approval
        /// This admin can either enter the officer number for assignment or enter comment in the case if he/she doesn't have right to do the assignment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string EscortOfficerSelection(USSDRequestModel model)
        {
            string phoneNumber = model.PhoneNumber;
            int startIndex = 4;
            string trimPhoneNumber = $"0{phoneNumber.Substring(startIndex)}";
            string[] requestStage = model.Text.Split('|');

            if (_requestApprovalHandler.Value.CanAdminAssignOfficers(requestStage[1], trimPhoneNumber))
            {
                return "Enter officer number (Seperated by comma if more than one)\n";
            }

            return "Enter Comment (Minimum of ten characters)\n";
        }

        /// <summary>
        /// Process approval or rejection request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessRequestApproval(USSDRequestModel model)
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

                string comment = string.Empty;
                RequestApprovalResponse approvalResponse = null;
                if (operationTypeId == (int)USSDOperationType.Decline)
                {
                    comment = requestStage[requestStage.Length - 1];
                    approvalResponse = _requestApprovalHandler.Value.ProcessRequestRejection(requestStage[1], ref errors, comment, trimPhoneNumber);
                    return $"{approvalResponse.NotificationMessage}\n";
                }

                string officer = string.Empty;
                if (requestStage.Length == 4)
                {
                    if (_requestApprovalHandler.Value.CanAdminAssignOfficers(requestStage[1], trimPhoneNumber))
                    {
                        officer = requestStage[3];
                        if (!string.IsNullOrEmpty(officer))
                        {
                            return "Enter Comment (Minimum of ten characters)\n";
                        }
                        throw new DirtyFormDataException("Officer number is required, try again");
                    }

                    comment = requestStage[requestStage.Length - 1];
                    EscortRequestDetailsVM detVM = new EscortRequestDetailsVM
                    {
                        Comment = comment
                    };
                    approvalResponse = _requestApprovalHandler.Value.ProcessRequestApproval(requestStage[1], ref errors, detVM, trimPhoneNumber);
                    return $"{approvalResponse.NotificationMessage}\n";
                }

                officer = requestStage[3];
                string[] officers = officer.Split(',');
                comment = requestStage[requestStage.Length - 1];
                List<ProposedEscortOffficerVM> OfficersSelections = new List<ProposedEscortOffficerVM>();
                Dictionary<string, string> trackDuplicateOfficerId = new Dictionary<string, string>();
                foreach (string item in officers)
                {
                    if (trackDuplicateOfficerId.ContainsKey(item))
                    {
                        throw new DirtyFormDataException($"Duplicate officer id number {item} found");
                    }
                    trackDuplicateOfficerId.Add(item, item);
                    OfficersSelections.Add(new ProposedEscortOffficerVM { OfficerId = _policeOfficerRepo.Value.GetPoliceOfficerId(item) });
                }

                EscortRequestDetailsVM detailsVM = new EscortRequestDetailsVM
                {
                    Comment = comment,
                    OfficersSelection = OfficersSelections
                };
                approvalResponse = _requestApprovalHandler.Value.ProcessRequestApproval(requestStage[1], ref errors, detailsVM, trimPhoneNumber);
                return $"{approvalResponse.NotificationMessage}\n";
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