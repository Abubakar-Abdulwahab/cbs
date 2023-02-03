using Orchard;
using Orchard.Logging;
using Parkway.CBS.Police.Client.PSSServiceType.Contracts;
using Models = Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Police.Client.PSSServiceType
{
    public class ExtractDetails : IPSSServiceTypeDetails
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Extract;
        private readonly Lazy<IExtractDetailsManager<Models.ExtractDetails>> _extractDetailsManager;
        private readonly Lazy<IRequestStatusLogManager<Models.RequestStatusLog>> _requestStatusLogManager;
        private readonly ICoreExtractService _coreExtractService;
        private readonly IPSSRequestExtractDetailsCategoryManager<Models.PSSRequestExtractDetailsCategory> _pssRequestExtractDetailsCategoryManager;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;

        public ExtractDetails(IOrchardServices orchardServices, Lazy<IExtractDetailsManager<Models.ExtractDetails>> extractDetailsManager, Lazy<IRequestStatusLogManager<Models.RequestStatusLog>> requestStatusLogManager, ICoreExtractService coreExtractService, IPSSRequestExtractDetailsCategoryManager<Models.PSSRequestExtractDetailsCategory> pssRequestExtractDetailsCategoryManager)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _extractDetailsManager = extractDetailsManager;
            _requestStatusLogManager = requestStatusLogManager;
            _coreExtractService = coreExtractService;
            _pssRequestExtractDetailsCategoryManager = pssRequestExtractDetailsCategoryManager;
        }



        /// <summary>
        /// Get next move after profile confirmation
        /// </summary>
        /// <returns>RouteNameAndStage</returns>
        public RouteNameAndStage GetDirectionAfterUserProfileConfirmation()
        {
            return new RouteNameAndStage { RouteName = "P.ExtractRequest", Stage = PSSUserRequestGenerationStage.PSSExtractRequest };
        }



        public dynamic GetRequestDetails(string fileRefNumber, long taxEntityId)
        {
            try
            {
                IEnumerable<ExtractDetailsVM> extract = _extractDetailsManager.Value.GetExtractRequestViewDetails(fileRefNumber, taxEntityId);
                IEnumerable<PSSRequestStatusLogVM> requestStatusLog = _requestStatusLogManager.Value.GetRequestStatusLogVMByFileRefNumber(fileRefNumber);
                ExtractDetailsVM extractDets = extract.FirstOrDefault();
                extractDets.ExtractInfo.SelectedExtractCategories = _pssRequestExtractDetailsCategoryManager.GetExtractCategoriesForRequest(fileRefNumber);
                return new RequestDetailsVM { RequestStatusLog = requestStatusLog, ServiceName = extractDets.ExtractInfo.ServiceName, ServiceVM = extractDets.ExtractInfo, TaxEntity = extractDets.TaxEntity, ViewName = "ExtractDetailsPartial", FileRefNumber = extractDets.FileRefNumber, ApprovalNumber = extractDets.ApprovalNumber, HasCertificate = _coreExtractService.CheckIfExtractDocumentIsSigned(extractDets.ApprovalNumber), CertificateLabel = "Extract", Status = extractDets.RequestStatus };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public dynamic GetRequestInfo(long requestId)
        {
            try
            {
                IEnumerable<ExtractRequestDetailsVM> extract = _extractDetailsManager.Value.GetExtractDocumentInfo(requestId);
                ExtractRequestDetailsVM extractDets = extract.FirstOrDefault();
                extractDets.SelectedExtractCategories = _pssRequestExtractDetailsCategoryManager.GetExtractCategoriesForRequest(extractDets.FileRefNumber);
                extractDets.ViewName = "ExtractDocumentInfo";
                return extractDets;
            }
            catch (Exception) { throw; }
        }


        public CreateCertificateDocumentVM CreateCertificate(string fileRefNumber, long taxEntityId)
        {
            try {
                if (_coreExtractService.CheckIfApprovedExtractRequestExists(fileRefNumber, taxEntityId))
                {
                    return _coreExtractService.CreateExtractDocument(fileRefNumber, false);
                }
                else { throw new NoRecordFoundException("404 or request not yet approved for PSS Extract request. File Ref Number " + fileRefNumber); }
            }
            catch (Exception) { throw; }
        }
    }
}