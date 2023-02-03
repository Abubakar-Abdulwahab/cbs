using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class CharacterCertificateHandler : ICharacterCertificateHandler
    {
        private readonly IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> _repo;
        private readonly ICharacterCertificateBiometricsManager<CharacterCertificateBiometrics> _biometricsManagerRepo;
        public ILogger Logger { get; set; }
        private readonly IEnumerable<IPSSServiceTypeApprovalImpl> _serviceTypeApprovalImpl;
        private readonly IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> _serviceRequestFlowApprover;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreBiometricAppActivityLogService _biometricAppLogService;

        public CharacterCertificateHandler(IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> repo, ICharacterCertificateBiometricsManager<CharacterCertificateBiometrics> biometricsManagerRepo, IEnumerable<IPSSServiceTypeApprovalImpl> serviceTypeApprovalImpl, IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> serviceRequestFlowApprover, ICoreBiometricAppActivityLogService biometricAppLogService, IOrchardServices orchardServices)
        {
            _repo = repo;
            _biometricsManagerRepo = biometricsManagerRepo;
            Logger = NullLogger.Instance;
            _serviceTypeApprovalImpl = serviceTypeApprovalImpl;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
            _biometricAppLogService = biometricAppLogService;
            _orchardServices = orchardServices;
        }

        public APIResponse GetCharacterCertificateDetails(string fileNumber, string token, HttpRequestMessage httpRequest)
        {
            Node usingVersionNode = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.UsingVersion)).FirstOrDefault();
            if (usingVersionNode != null && !string.IsNullOrEmpty(usingVersionNode.Value))
            {
                bool.TryParse(usingVersionNode.Value, out bool usingVersion);
                if (usingVersion)
                {
                    string lastestAppVersion = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.LatestBiometricAppVersion)).FirstOrDefault().Value;

                    _biometricAppLogService.LogBiometricAppDetail(httpRequest, HttpContext.Current.Request.UserHostAddress, out string appVersion);

                    if (appVersion != lastestAppVersion)
                    {
                        return new APIResponse { ResponseObject = "Unable to proceed, please contact Admin.", ErrorCode = ((int)PSSErrorCode.PSSBAIO).ToString(), StatusCode = HttpStatusCode.OK };
                    }
                }
            }

            if (string.IsNullOrEmpty(fileNumber))
            {
                return new APIResponse { Error = true, ResponseObject = $"'{nameof(fileNumber)}' is required. ", StatusCode = System.Net.HttpStatusCode.OK };
            }

            bool parseResult = int.TryParse(Util.Decrypt(token, AppSettingsConfigurations.AESEncryptionSecret), out int userPartRecordId);

            if (!parseResult)
            {
                Logger.Error($"Unable to parse token: {token}");
                return new APIResponse { Error = true, ResponseObject = $"Unable to validate admin details, please contact admin ", StatusCode = System.Net.HttpStatusCode.OK };
            }

            CharacterCertificateDocumentVM result = _repo.GetPendingCharacterCertificateDocumentDetails(fileNumber);
            if (result == null)
            {
                return new APIResponse { Error = true, ResponseObject = $"No records found for file number: {fileNumber}", StatusCode = HttpStatusCode.OK };
            }

            //check admin user can capture biometric for this request
            bool adminCanApproveRequest = _serviceRequestFlowApprover.UserIsValidApproverForDefinitionLevel(userPartRecordId, result.FlowDefinitionLevelId);
            if (!adminCanApproveRequest)
            {
                return new APIResponse { Error = true, ResponseObject = PoliceErrorLang.usernotauthorized().ToString(), StatusCode = HttpStatusCode.OK };
            }

            if (!result.HasApplicantBeenInvitedForCapture)
            {
                return new APIResponse { Error = true, ResponseObject = $"User has not been invited for biometric capture. File number: {fileNumber}", StatusCode = HttpStatusCode.OK };
            }

            if (!result.BiometricCaptureDueDate.HasValue)
            {
                return new APIResponse { Error = true, ResponseObject = $"No biometric capture due date found for this request. File number: {fileNumber}", StatusCode = HttpStatusCode.OK };
            }

            if (DateTime.Now > result.BiometricCaptureDueDate.Value)
            {
                return new APIResponse { Error = true, ResponseObject = $"User can no longer capture for request with file number {fileNumber}. The capture due date was {result.BiometricCaptureDueDate.Value.ToString("dd/MM/yyyy")}", StatusCode = HttpStatusCode.OK };
            }

            if (result.IsBiometricsEnrolled)
            {
                return new APIResponse { Error = true, ResponseObject = $"User already enrolled for file number: {fileNumber}", StatusCode = HttpStatusCode.OK };
            }

            return new APIResponse { ResponseObject = result, StatusCode = System.Net.HttpStatusCode.OK };
        }

        /// <summary>
        /// Validates and saves <see cref="CharacterCertificateBiometrics"/> 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public APIResponse SaveCharacterCertificateBiometrics(CharacterCertificateBiometricRequestVM model, HttpRequestMessage httpRequest)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                Node usingVersionNode = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.UsingVersion)).FirstOrDefault();
                if (usingVersionNode != null && !string.IsNullOrEmpty(usingVersionNode.Value))
                {
                    bool.TryParse(usingVersionNode.Value, out bool usingVersion);
                    if (usingVersion)
                    {
                        string lastestAppVersion = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.LatestBiometricAppVersion)).FirstOrDefault().Value;

                        _biometricAppLogService.LogBiometricAppDetail(httpRequest, HttpContext.Current.Request.UserHostAddress, out string appVersion);

                        if (appVersion != lastestAppVersion)
                        {
                            return new APIResponse { ResponseObject = "Unable to proceed, please contact Admin.", ErrorCode = ((int)PSSErrorCode.PSSBAIO).ToString(), StatusCode = HttpStatusCode.OK };
                        }
                    }
                }

                CharacterCertificateDocumentVM characterCertificateModel = _repo.GetPendingCharacterCertificateDocumentDetails(model.FileNumber);

                if (characterCertificateModel == null)
                {
                    return new APIResponse { Error = true, ResponseObject = $"No records found for file number: {model.FileNumber}", StatusCode = System.Net.HttpStatusCode.OK };
                }

                if (characterCertificateModel.IsBiometricsEnrolled)
                {
                    return new APIResponse { Error = true, ResponseObject = $"User already enrolled for file number: {model.FileNumber}", StatusCode = System.Net.HttpStatusCode.OK };
                }

                bool parseResult = int.TryParse(Util.Decrypt(model.Token, AppSettingsConfigurations.AESEncryptionSecret), out int userPartRecordId);

                if (!parseResult)
                {
                    Logger.Error($"Unable to parse token: {model.Token}");
                    return new APIResponse { Error = true, ResponseObject = $"Unable to validate admin details, please contact admin ", StatusCode = System.Net.HttpStatusCode.OK };
                }

                bool result = _biometricsManagerRepo.Save(new CharacterCertificateBiometrics
                {
                    CharacterCertificateDetails = new PSSCharacterCertificateDetails { Id = characterCertificateModel.CharacterCertificateDetailsId },
                    Request = new PSSRequest { Id = characterCertificateModel.RequestId },
                    UserPartRecord = new Orchard.Users.Models.UserPartRecord { Id = userPartRecordId },
                    PassportImage = model.PassportImage,
                    RightIndex = model.RightIndex,
                    RegisteredAt = model.RegisteredDate,
                    RightMiddle = model.RightMiddle,
                    RightPinky = model.RightPinky,
                    RightRing = model.RightRing,
                    RightThumb = model.RightThumb,
                    LeftRing = model.LeftRing,
                    LeftIndex = model.LeftIndex,
                    LeftMiddle = model.LeftMiddle,
                    LeftPinky = model.LeftPinky,
                    LeftThumb = model.LeftThumb,
                });

                if (!result)
                {
                    throw new Exception($"Unable to save biometric details for request {characterCertificateModel.RequestId}");
                }

                _repo.UpdateCharacterCertificateIsBiometricsEnrolledStatus(characterCertificateModel.CharacterCertificateDetailsId);

                GenericRequestDetails requestDetails = new GenericRequestDetails { ApproverComment = model.Comment, RequestId = characterCertificateModel.RequestId, ServiceTypeId = characterCertificateModel.ServiceTypeId, TaxEntity = new TaxEntityViewModel { Recipient = characterCertificateModel.CustomerName } };

                foreach (var impl in _serviceTypeApprovalImpl)
                {
                    if (impl.GetServiceTypeDefinition == (PSSServiceTypeDefinition)requestDetails.ServiceTypeId)
                    {
                        impl.ValidatedAndProcessRequestApproval(requestDetails, ref errors, new CharacterCertificateRequestDetailsVM { ApproverId = userPartRecordId });
                        return new APIResponse { ResponseObject = "Successful", StatusCode = System.Net.HttpStatusCode.OK };
                    }
                }

                throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + characterCertificateModel.RequestId);
            }
            catch (Exception)
            {
                _repo.RollBackAllTransactions();
                throw;
            }
        }
    }
}