using Orchard;
using Orchard.Logging;
using Parkway.CBS.Police.Client.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;
using System;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Police.Client.PSSServiceType
{
    public class EscortRegularizationDetails : IPSSServiceTypeDetails
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.EscortRegularization;
        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortDetailsManager;
        private readonly Lazy<IRequestStatusLogManager<RequestStatusLog>> _requestStatusLogManager;
        private readonly ICoreEscortService _coreEscortService;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        public EscortRegularizationDetails(IOrchardServices orchardServices, Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortDetailsManager, Lazy<IRequestStatusLogManager<RequestStatusLog>> requestStatusLogManager, ICoreEscortService coreEscortService)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _escortDetailsManager = escortDetailsManager;
            _requestStatusLogManager = requestStatusLogManager;
            _coreEscortService = coreEscortService;
        }


        /// <summary>
        /// Get next move after profile confirmation
        /// </summary>
        /// <returns>RouteNameAndStage</returns>
        public RouteNameAndStage GetDirectionAfterUserProfileConfirmation()
        {
            return new RouteNameAndStage { RouteName = "P.Escort.Request", Stage = PSSUserRequestGenerationStage.PSSRequest };
        }



        public dynamic GetRequestDetails(string fileRefNumber, long taxEntityId)
        {
            try
            {
                IEnumerable<EscortDetailsVM> escort = _escortDetailsManager.Value.GetEscortRequestViewDetails(fileRefNumber, taxEntityId);
                IEnumerable<PSSRequestStatusLogVM> requestStatusLog = _requestStatusLogManager.Value.GetRequestStatusLogVMByFileRefNumber(fileRefNumber);
                EscortDetailsVM escortDets = escort.FirstOrDefault();
                return new RequestDetailsVM { RequestStatusLog = requestStatusLog, ServiceName = escortDets.EscortInfo.ServiceName, ServiceVM = escortDets.EscortInfo, TaxEntity = escortDets.TaxEntity, ViewName = "EscortDetailsPartial", FileRefNumber = escortDets.EscortInfo.FileRefNumber, ApprovalNumber = escortDets.EscortInfo.ApprovalNumber, HasCertificate = !string.IsNullOrEmpty(escortDets.EscortInfo.ApprovalNumber), CertificateLabel = "Dispatch Note", Status = escortDets.RequestStatus };
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
                IEnumerable<EscortDetailsVM> escort = _escortDetailsManager.Value.GetEscortDocumentInfo(requestId);
                EscortDetailsVM escortDets = escort.FirstOrDefault();
                escortDets.ViewName = "EscortDocumentInfo";
                return escortDets;
            }
            catch (Exception) { throw; }
        }


        public CreateCertificateDocumentVM CreateCertificate(string fileRefNumber, long taxEntityId)
        {
            try
            {
                if (_coreEscortService.CheckIfApprovedEscortRequestExists(fileRefNumber, taxEntityId))
                {
                    return _coreEscortService.CreateDispatchNote(fileRefNumber, false);
                }
                else { throw new NoRecordFoundException("404 or request not yet approved for escort request. File Ref Number " + fileRefNumber); }
            }
            catch (Exception) { throw; }
        }
    }
}