using Orchard;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval
{
    public class CharacterCertificate : IPSSServiceTypeApprovalImpl, IPSSServiceTypeDocumentPreviewImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.CharacterCertificate;
        private readonly IApprovalComposition _approvalCompositionHandler;
        public ILogger Logger { get; set; }
        private readonly Lazy<IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>> _characterCertificateDetailsManager;
        private readonly ICoreCharacterCertificateService _coreCharacterCertificateService;
        private readonly IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog> _pssRequestApprovalDocumentPreviewLogManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _pssServiceRequestFlowDefinitionLevelRepo;
        private readonly IPolicerOfficerLogManager<PolicerOfficerLog> _policerOfficerLogManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _pssServiceRequestFlowDefinitionLevelManager;

        public CharacterCertificate(Lazy<IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>> characterCertificateDetailsManager, IApprovalComposition approvalCompositionHandler, ICoreCharacterCertificateService coreCharacterCertificateService, IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog> pssRequestApprovalDocumentPreviewLogManager, IOrchardServices orchardServices, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> pssServiceRequestFlowDefinitionLevelRepo, IPolicerOfficerLogManager<PolicerOfficerLog> policerOfficerLogManager, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> pssServiceRequestFlowDefinitionLevelManager)
        {
            _characterCertificateDetailsManager = characterCertificateDetailsManager;
            _coreCharacterCertificateService = coreCharacterCertificateService;
            _approvalCompositionHandler = approvalCompositionHandler;
            _pssRequestApprovalDocumentPreviewLogManager = pssRequestApprovalDocumentPreviewLogManager;
            _pssServiceRequestFlowDefinitionLevelRepo = pssServiceRequestFlowDefinitionLevelRepo;
            _policerOfficerLogManager = policerOfficerLogManager;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _pssServiceRequestFlowDefinitionLevelManager = pssServiceRequestFlowDefinitionLevelManager;
        }


        public CharacterCertificateRequestDetailsVM GetCharacterCertificateViewDetails(long requestId)
        {
            CharacterCertificateRequestDetailsVM characterCertificate = _characterCertificateDetailsManager.Value.GetRequestDetails(requestId);
            characterCertificate.IsLastApprover = _pssServiceRequestFlowDefinitionLevelRepo.CheckIfThisIsLastApprover(characterCertificate.DefinitionId, characterCertificate.Position);
            characterCertificate.ViewName = "PSSCharacterCertificateDetails";
            if (string.IsNullOrEmpty(characterCertificate.RefNumber))
            {
                //If the RefNumber has not been updated, check if this approval level(FlowDefinitionLevel) will be the one to populate it
                //The approval level(FlowDefinitionLevel) to populate the RefNumber is the level before the applicant will be invited for biometric capture
                //Then, we check if the next approval elevel(FlowDefinitionLevel) is biometric invitation level
                string nextApprovalButtonName = _coreCharacterCertificateService.CheckIfCanShowRefNumberForm(characterCertificate.DefinitionId, characterCertificate.Position);
                if (nextApprovalButtonName == "Invite For Capture")
                {
                    characterCertificate.ShowReferenceNumberForm = true;
                }
            }
            return characterCertificate;
        }


        /// <summary>
        /// Get the Escort view details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetails(long requestId)
        {
            return GetCharacterCertificateViewDetails(requestId);
        }


        /// <summary>
        /// Get the view details for approval using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="adminUserId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetailsForApproval(long requestId, int adminUserId)
        {
            CharacterCertificateRequestDetailsVM characterCertificate = GetCharacterCertificateViewDetails(requestId);
            characterCertificate.RequestStages = _pssServiceRequestFlowDefinitionLevelManager.GetApprovalDefinitionLevelsAfterPositionForDefinitionWithId(characterCertificate.DefinitionId, characterCertificate.Position);
            characterCertificate.DisplayDetailsForApproval = true;
            return characterCertificate;
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
                CharacterCertificateRequestDetailsVM objUserInput = (CharacterCertificateRequestDetailsVM)userInput;
                //do validation
                if (_characterCertificateDetailsManager.Value.Count(x => x.Request == new PSSRequest { Id = requestDetails.RequestId }) < 1)
                {
                    throw new NoRecordFoundException("No record found for character certificate with request Id " + requestDetails.RequestId);
                }

                CharacterCertificateRequestDetailsVM requestRefAndWorkflowDetails = _characterCertificateDetailsManager.Value.GetRefNumberAndWorkflowDetails(requestDetails.RequestId);
                if (string.IsNullOrEmpty(requestRefAndWorkflowDetails.RefNumber))
                {
                    if (string.IsNullOrEmpty(objUserInput.RefNumber) || objUserInput.RefNumber.Trim().Length < 3 || objUserInput.RefNumber.Trim().Length > 100)
                    {
                        //Check if this approval level supposed to provide the RefNumber
                        string nextApprovalButtonName = _coreCharacterCertificateService.CheckIfCanShowRefNumberForm(requestRefAndWorkflowDetails.DefinitionId, requestRefAndWorkflowDetails.Position);
                        if (nextApprovalButtonName == "Invite For Capture")
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Ref Number is required. Must be between 3 and 100 characters", FieldName = "RefNumber" });
                            throw new DirtyFormDataException("Ref Number is required. Must be between 3 and 100 characters");
                        }
                    }

                    _characterCertificateDetailsManager.Value.UpdateCharacterCertificateRefNumber(requestDetails.RequestId, objUserInput.RefNumber);
                }

                if(_pssServiceRequestFlowDefinitionLevelRepo.CheckIfThisIsLastApprover(requestRefAndWorkflowDetails.DefinitionId, requestRefAndWorkflowDetails.Position))
                {
                    if(objUserInput.SelectedCPCCR == null || !objUserInput.SelectedCPCCR.Any())
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "CPCCR not selected", FieldName = "SelectedCPCCR" });
                        throw new DirtyFormDataException("CPCCR not selected");
                    }
                    PoliceOfficerLogVM cpccr = _policerOfficerLogManager.GetPoliceOfficerDetails(objUserInput.SelectedCPCCR.ElementAt(0).PoliceOfficerLogId);
                    if(cpccr == null)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "CPCCR selected does not exist", FieldName = "SelectedCPCCR" });
                        throw new DirtyFormDataException("CPCCR selected does not exist");
                    }
                    _characterCertificateDetailsManager.Value.UpdateCharacterCertificateCPCCRNameAndServiceNumber(requestDetails.RequestId, cpccr.IdentificationNumber, cpccr.Name, cpccr.RankCode, cpccr.RankName, objUserInput.ApproverId);
                }

                return _approvalCompositionHandler.ProcessRequestApproval(requestDetails, objUserInput.ApproverId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _approvalCompositionHandler.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Generates a draft service document for preview before approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateDraftServiceDocumentByteFile(string fileRefNumber)
        {
            try
            {
                return _coreCharacterCertificateService.CreateDefaultCertificateDocument(fileRefNumber);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}