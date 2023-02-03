using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Client.PSSServiceType
{
    public class CharacterCertificateDetails : IPSSServiceTypeDetails
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.CharacterCertificate;
        private readonly Lazy<IPSSCharacterCertificateDetailsManager<Core.Models.PSSCharacterCertificateDetails>> _characterCertificateDetailsManager;
        private readonly Lazy<IRequestStatusLogManager<Core.Models.RequestStatusLog>> _requestStatusLogManager;
        private readonly ICoreCharacterCertificateService _coreCharacterCertificateService;
        public ILogger Logger { get; set; }



        private readonly IOrchardServices _orchardServices;

        public CharacterCertificateDetails(IOrchardServices orchardServices, Lazy<IPSSCharacterCertificateDetailsManager<Core.Models.PSSCharacterCertificateDetails>> characterCertificateDetailsManager, Lazy<IRequestStatusLogManager<Core.Models.RequestStatusLog>> requestStatusLogManager, ICoreCharacterCertificateService coreCharacterCertificateService)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _characterCertificateDetailsManager = characterCertificateDetailsManager;
            _requestStatusLogManager = requestStatusLogManager;
            _coreCharacterCertificateService = coreCharacterCertificateService;
        }


        /// <summary>
        /// After profile deets have been sorted out
        /// we need this service to determine the flow of this request
        /// </summary>
        /// <returns>RouteNameAndStage</returns>
        public RouteNameAndStage GetDirectionAfterUserProfileConfirmation()
        {
            if (AppSettingsConfigurations.GetSettingsValue("DoPCCOption") == null)
            {
                return new RouteNameAndStage { RouteName = "P.CharacterCertificateRequest", Stage = PSSUserRequestGenerationStage.PSSCharacterCertificateRequest };
            }
            return new RouteNameAndStage { RouteName = RouteName.ServiceOptions.SelectOption, Stage = PSSUserRequestGenerationStage.ServiceOptions };
        }


        public dynamic GetRequestDetails(string fileRefNumber, long taxEntityId)
        {
            try
            {
                IEnumerable<CharacterCertificateDetailsVM> characterCertificateDetails = _characterCertificateDetailsManager.Value.GetCharacterCertificateRequestViewDetails(fileRefNumber, taxEntityId);
                IEnumerable<PSSRequestStatusLogVM> requestStatusLog = _requestStatusLogManager.Value.GetRequestStatusLogVMByFileRefNumber(fileRefNumber);
                CharacterCertificateDetailsVM characterCertificateDets = characterCertificateDetails.FirstOrDefault();
                return new RequestDetailsVM { RequestStatusLog = requestStatusLog, ServiceName = characterCertificateDets.CharacterCertificateInfo.ServiceName, ServiceVM = characterCertificateDets.CharacterCertificateInfo, TaxEntity = characterCertificateDets.TaxEntity, ViewName = "CharacterCertificateDetailsPartial", FileRefNumber = characterCertificateDets.FileRefNumber, ApprovalNumber = characterCertificateDets.ApprovalNumber, HasCertificate = !string.IsNullOrEmpty(characterCertificateDets.ApprovalNumber) || characterCertificateDets.RequestStatus == PSSRequestStatus.Rejected, CertificateLabel = "Certificate", Status = (int)characterCertificateDets.RequestStatus };

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
                IEnumerable<CharacterCertificateDetailsVM> characterCertificateDetails = _characterCertificateDetailsManager.Value.GetCharacterCertificateDocumentInfo(requestId);
                CharacterCertificateDetailsVM characterCertificateDets = characterCertificateDetails.FirstOrDefault();
                characterCertificateDets.ViewName = "CharacterCertificateDocumentInfo";
                return characterCertificateDets;
            }
            catch (Exception) { throw; }
        }

        public CreateCertificateDocumentVM CreateCertificate(string fileRefNumber, long taxEntityId)
        {
            try
            {

                if (_coreCharacterCertificateService.CheckIfRejectedCharacterCertificateRequestExists(fileRefNumber, taxEntityId))
                {
                    return _coreCharacterCertificateService.CreateRejectionCertificateDocument(fileRefNumber, false);

                }
                else if (_coreCharacterCertificateService.CheckIfApprovedCharacterCertificateRequestExists(fileRefNumber, taxEntityId))
                {
                    return _coreCharacterCertificateService.CreateCertificateDocument(fileRefNumber, false);
                }
                else { throw new NoRecordFoundException("404 or request not yet approved or rejected for PSS Character Certificate request. File Ref Number " + fileRefNumber); }
            }
            catch (Exception) { throw; }
        }
    }
}