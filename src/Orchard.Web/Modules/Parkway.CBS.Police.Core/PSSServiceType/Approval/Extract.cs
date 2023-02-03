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
using System.Globalization;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval
{
    public class Extract : IPSSServiceTypeApprovalImpl, IPSSServiceTypeDocumentPreviewImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Extract;

        private readonly IApprovalComposition _approvalCompositionHandler;
        private readonly ICoreExtractService _coreExtractService;
        public ILogger Logger { get; set; }
        private readonly Lazy<IExtractDetailsManager<ExtractDetails>> _extractDetailsManager;
        private readonly IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog> _pssRequestApprovalDocumentPreviewLogManager;
        private readonly IPSSRequestExtractDetailsCategoryManager<PSSRequestExtractDetailsCategory> _pssRequestExtractDetailsCategoryManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _pssServiceRequestFlowDefinitionLevelManager;
        private readonly IPolicerOfficerLogManager<PolicerOfficerLog> _policerOfficerLogManager;
        private readonly IOrchardServices _orchardServices;


        public Extract(Lazy<IExtractDetailsManager<ExtractDetails>> extractDetailsManager, IApprovalComposition approvalCompositionHandler, ICoreExtractService coreExtractService, IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog> pssRequestApprovalDocumentPreviewLogManager, IOrchardServices orchardServices, IPSSRequestExtractDetailsCategoryManager<PSSRequestExtractDetailsCategory> pssRequestExtractDetailsCategoryManager, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> pssServiceRequestFlowDefinitionLevelManager, IPolicerOfficerLogManager<PolicerOfficerLog> policerOfficerLogManager)
        {
            _extractDetailsManager = extractDetailsManager;
            _coreExtractService = coreExtractService;
            _approvalCompositionHandler = approvalCompositionHandler;
            _pssRequestApprovalDocumentPreviewLogManager = pssRequestApprovalDocumentPreviewLogManager;
            _pssRequestExtractDetailsCategoryManager = pssRequestExtractDetailsCategoryManager;
            _pssServiceRequestFlowDefinitionLevelManager = pssServiceRequestFlowDefinitionLevelManager;
            _policerOfficerLogManager = policerOfficerLogManager;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }



        public ExtractRequestDetailsVM GetExtractViewDetails(long requestId)
        {
            ExtractRequestDetailsVM extract = _extractDetailsManager.Value.GetRequestDetails(requestId);
            extract.IsLastApprover = _pssServiceRequestFlowDefinitionLevelManager.CheckIfThisIsLastApprover(extract.DefinitionId, extract.Position);
            extract.ViewName = "PSSExtractDetails";
            extract.SelectedExtractCategories = _pssRequestExtractDetailsCategoryManager.GetExtractCategoriesForRequest(extract.FileRefNumber);
            return extract;
        }


        /// <summary>
        /// Get the Escort view details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetails(long requestId)
        {
            return GetExtractViewDetails(requestId);
        }


        /// <summary>
        /// Get the view details for approval using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="adminUserId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetailsForApproval(long requestId, int adminUserId)
        {
            ExtractRequestDetailsVM extract = GetExtractViewDetails(requestId);
            extract.DisplayDetailsForApproval = true;
            extract.SelectedExtractCategories = _pssRequestExtractDetailsCategoryManager.GetExtractCategoriesForRequest(extract.FileRefNumber);
            return extract;
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
                ExtractRequestDetailsVM objUserInput = (ExtractRequestDetailsVM)userInput;

                ExtractRequestDetailsVM extract = _extractDetailsManager.Value.GetRequestDetails(requestDetails.RequestId);
                if (extract == null)
                {
                    throw new NoRecordFoundException("No record found for extract with request Id " + requestDetails.RequestId);
                }

                if (string.IsNullOrEmpty(extract.Content) || string.IsNullOrEmpty(extract.DiarySerialNumber) || extract.IncidentDateAndTimeParsed == null)
                {
                    //do validation only if the content, diary serial number, incident date or time have not been filled previously by first approver
                    if (!string.IsNullOrEmpty(objUserInput.DiarySerialNumber))
                    {
                        if (objUserInput.DiarySerialNumber.Trim().Length == 0)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Diary serial number field cannot be empty", FieldName = "DiarySerialNumber" });
                            throw new DirtyFormDataException("Diary serial number field cannot be empty.");
                        }
                    }
                    else
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Diary serial number field cannot be empty", FieldName = "DiarySerialNumber" });
                        throw new DirtyFormDataException("Diary serial number field cannot be empty.");
                    }

                    if (!string.IsNullOrEmpty(objUserInput.Content))
                    {
                        if (objUserInput.Content.Trim().Length < 10 || objUserInput.Content.Trim().Length > 1000)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Content field value must be between 10 and 1000 characters", FieldName = "Content" });
                            throw new DirtyFormDataException("Content field value must be between 10 and 1000 characters.");
                        }
                    }
                    else
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Content field cannot be empty", FieldName = "Content" });
                        throw new DirtyFormDataException("Content field cannot be empty.");
                    }

                    if (string.IsNullOrEmpty(objUserInput.IncidentDate) || objUserInput.IncidentDate.Trim().Length == 0)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Incident date is required", FieldName = "IncidentDate" });
                        throw new DirtyFormDataException("Incident date is required.");
                    }

                    if (string.IsNullOrEmpty(objUserInput.IncidentTime) || objUserInput.IncidentTime.Trim().Length == 0)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Incident time is required", FieldName = "IncidentTime" });
                        throw new DirtyFormDataException("Incident time is required.");
                    }

                    if(!string.IsNullOrEmpty(objUserInput.CrossReferencing) && objUserInput.CrossReferencing.Trim().Length < 1 || !string.IsNullOrEmpty(objUserInput.CrossReferencing) && objUserInput.CrossReferencing.Trim().Length > 10)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Cross referencing field value must be between 1 and 10 characters", FieldName = "CrossReferencing" });
                        throw new DirtyFormDataException("Cross referencing field value must be between 1 and 10 characters.");
                    }

                    try
                    {
                        objUserInput.IncidentDateAndTimeParsed = DateTime.ParseExact(objUserInput.IncidentDate.Trim() + " " + objUserInput.IncidentTime.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Unable to parse incident date and time.");
                    }

                    if (objUserInput.IncidentDateAndTimeParsed > DateTime.Now)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Incident date cannot be in the future", FieldName = "IncidentDate" });
                        throw new DirtyFormDataException("Incident date cannot be in the future.");
                    }


                    _extractDetailsManager.Value.UpdateExtractDetailsContentAndDiaryInfo(requestDetails.RequestId, objUserInput);
                }

                if (_pssServiceRequestFlowDefinitionLevelManager.CheckIfThisIsLastApprover(extract.DefinitionId, extract.Position))
                {
                    if (objUserInput.SelectedDPO == null || !objUserInput.SelectedDPO.Any())
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "DPO not selected", FieldName = "SelectedDPO" });
                        throw new DirtyFormDataException("DPO not selected");
                    }
                    PoliceOfficerLogVM dpo = _policerOfficerLogManager.GetPoliceOfficerDetails(objUserInput.SelectedDPO.ElementAt(0).PoliceOfficerLogId);
                    if (dpo == null)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "DPO selected does not exist", FieldName = "SelectedDPO" });
                        throw new DirtyFormDataException("DPO selected does not exist");
                    }
                    _extractDetailsManager.Value.UpdateExtractDPONameAndServiceNumber(requestDetails.RequestId, dpo.IdentificationNumber, dpo.Name, dpo.RankCode, objUserInput.ApproverId);
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
                return _coreExtractService.CreateDefaultExtractDocument(fileRefNumber);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}