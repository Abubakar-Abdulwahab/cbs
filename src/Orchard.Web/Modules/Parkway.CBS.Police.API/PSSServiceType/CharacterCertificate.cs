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
    public class CharacterCertificate : IPSSServiceTypeUSSDApprovalImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.CharacterCertificate;
        public ILogger Logger { get; set; }
        private readonly Lazy<IUSSDRequestApprovalHandler> _requestApprovalHandler;
        private readonly Lazy<IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>> _certificateDetailsManager;
        private static int ReferenceNumberStage = 4;

        public CharacterCertificate(Lazy<IUSSDRequestApprovalHandler> requestApprovalHandler, Lazy<IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>> certificateDetailsManager)
        {
            Logger = NullLogger.Instance;
            _requestApprovalHandler = requestApprovalHandler;
            _certificateDetailsManager = certificateDetailsManager;
        }


        /// <summary>
        /// This handles processing of a USSD aproval request that has to do with character certificate
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        public USSDAPIResponse ProcessRequest(USSDRequestModel model)
        {
            string[] requestStage = model.Text.Split('|');
            bool checkReferenceNumber = _certificateDetailsManager.Value.CheckReferenceNumber(requestStage[1]);

            if (requestStage.Length == (int)USSDProcessingStage.OperationType)
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
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Comment (Minimum of ten characters)." };
                }

                //If Reference Number has been populated, then show the user to proceed to the comment stage
                if (checkReferenceNumber)
                {
                    //Commented out the certificate draft view confirmaion because we were told to remove it.
                    //_requestApprovalHandler.Value.ConfirmAdminHasViewedDraftDocument(requestStage[1], $"0{model.PhoneNumber.Substring(PhoneTrimStartIndex)}");
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Comment (Minimum of ten characters)." };
                }

                return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Reference Number." };
            }

            //If this is true, it means Diary Number and Incident Date Time have been populated, then just go ahead to approve the request
            if (checkReferenceNumber)
            {
                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = ProcessRequestApproval(model, true) };
            }


            if (requestStage.Length == ReferenceNumberStage)
            {
                bool parsed = int.TryParse(requestStage[2], out int operationTypeId);
                if (operationTypeId == (int)USSDOperationType.Decline)
                {
                    return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = ProcessRequestApproval(model) };
                }

                if (string.IsNullOrEmpty(requestStage[3]))
                {
                    throw new DirtyFormDataException("Reference Number is required. Must be between 3 and 100 characters.");
                }

                //Commented out the certificate draft view confirmaion because we were told to remove it.
                //_requestApprovalHandler.Value.ConfirmAdminHasViewedDraftDocument(requestStage[1], $"0{model.PhoneNumber.Substring(PhoneTrimStartIndex)}");
                return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Comment (Minimum of ten characters)." };
            }

            return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = ProcessRequestApproval(model) };
        }

        /// <summary>
        /// Process approval or rejection request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessRequestApproval(USSDRequestModel model, bool isReferenceNumberUpdated = false)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
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
                    approvalResponse = _requestApprovalHandler.Value.ProcessRequestRejection(requestStage[1], ref errors, comment, model.PhoneNumber);
                    return $"{approvalResponse.NotificationMessage}\n";
                }

                CharacterCertificateRequestDetailsVM detailsVM = null;

                if (isReferenceNumberUpdated)
                {
                    detailsVM = new CharacterCertificateRequestDetailsVM
                    {
                        Comment = comment
                    };
                }
                else
                {
                    detailsVM = new CharacterCertificateRequestDetailsVM
                    {
                        Comment = comment,
                        RefNumber = requestStage[3]
                    };
                }

                approvalResponse = _requestApprovalHandler.Value.ProcessRequestApproval(requestStage[1], ref errors, detailsVM, model.PhoneNumber);
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