using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Orchard.Logging;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Utilities;
using Orchard;
using Parkway.CBS.Core.StateConfig;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSCharacterCertificateRequestDetailsHandler : IPSSCharacterCertificateRequestDetailsHandler
    {
        private readonly ICoreCharacterCertificateService _coreCharacterCertificateService;
        private readonly Lazy<IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>> _characterCertificateDetailsManager;
        private readonly IPolicerOfficerLogManager<PolicerOfficerLog> _policerOfficerLogManager;
        private readonly IHandlerComposition _handlerComposition;

        ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ITypeImplComposer _typeImpl;

        public PSSCharacterCertificateRequestDetailsHandler(IOrchardServices orchardServices, ICoreCharacterCertificateService coreCharacterCertificateService, IHandlerComposition handlerComposition, ITypeImplComposer typeImpl, Lazy<IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>> characterCertificateDetailsManager, IPolicerOfficerLogManager<PolicerOfficerLog> policerOfficerLogManager)
        {
            _coreCharacterCertificateService = coreCharacterCertificateService;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _handlerComposition = handlerComposition;
            _typeImpl = typeImpl;
            _characterCertificateDetailsManager = characterCertificateDetailsManager;
            _policerOfficerLogManager = policerOfficerLogManager;
        }


        /// <summary>
        /// Generate character certificate document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateCharacterCertificateByteFile(string fileRefNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0) { throw new Exception("File ref number not specified"); }
                if (_coreCharacterCertificateService.CheckIfApprovedCharacterCertificateRequestExists(fileRefNumber))
                {
                    return _coreCharacterCertificateService.CreateCertificateDocument(fileRefNumber, false);
                }
                else { throw new NoRecordFoundException("404 or request not yet approved for PSS Character Certificate request. File Ref Number " + fileRefNumber); }
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Invite an applicant for biometric capture
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>string</returns>
        public string InviteApplicantForBiometricCapture(long requestId)
        {
            try
            {
                //Check if user has been invited before
                PSSRequestDetailsVM biometricInvitationDetail = _coreCharacterCertificateService.GetBiometricInvitationDetails(requestId);
                if(biometricInvitationDetail == null)
                {
                    throw new Exception($"No record found for request Id {requestId}");
                }

                if (biometricInvitationDetail.IsApplicantInvitedForCapture)
                {
                    return $"This applicant has been previously invited for biometric capture";
                }

                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PCCBiometricCaptureDueDay.ToString()).FirstOrDefault();
                if (node != null && !string.IsNullOrEmpty(node.Value))
                {
                    bool result = double.TryParse(node.Value, out double captureDueDay);
                    if (!result)
                    {
                        throw new Exception($"Unable to get PCC biometric capture due date for filenumber {biometricInvitationDetail.FileRefNumber}");
                    }

                    //Update biometric capture info, add a day to the due date to be able to set the time to 11:59:59PM on the deadline date
                    captureDueDay += 1;
                    DateTime captureDueDate = DateTime.Now.AddDays(captureDueDay).Date.AddMilliseconds(-1);
                    _coreCharacterCertificateService.UpdateApplicantBiometricInvitationDetails(requestId, captureDueDate);

                    string captureDueDateString = DateTime.Now.AddDays(captureDueDay).ToString("dd/MM/yyyy");
                    string message = $"Dear {biometricInvitationDetail.TaxEntity.Recipient}, please proceed to ({biometricInvitationDetail.CommandName}) for biometric capture in respect of application number {biometricInvitationDetail.FileRefNumber}. Address: {biometricInvitationDetail.CommandAddress}. Capture due date : {captureDueDateString}";
                    _typeImpl.SendGenericSMSNotification(new List<string> { biometricInvitationDetail.TaxEntity.PhoneNumber }, message);

                    return $"Applicant successfully invited for biometric capture";
                }

                throw new Exception($"Unable to get PCC biometric capture due date for filenumber {biometricInvitationDetail.FileRefNumber}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets Character Certificate Biometrics by <paramref name="requestId"/>
        /// </summary>
        /// <returns></returns>
        public CharacterCertificateBiometricsVM GetCharacterCertificateBiometrics(long requestId)
        {
            return _coreCharacterCertificateService.GetCharacterCertificateBiometric(requestId);
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        public void CheckForPermission(Permission canViewRequests)
        {
            _handlerComposition.IsAuthorized(canViewRequests);
        }

        /// <summary>
        /// Creates and saves the rejection certificate
        /// </summary>
        /// <param name="characterCertificateDetail"></param>
        /// <param name="errors"></param>
        public void CreateAndSaveRejectionCertificate(CharacterCertificateRequestDetailsVM characterCertificateDetail, ref List<ErrorModel> errors)
        {
            try
            {
                if (characterCertificateDetail.SelectedCPCCR != null && characterCertificateDetail.SelectedCPCCR.Any())
                {
                    PoliceOfficerLogVM cpccr = _policerOfficerLogManager.GetPoliceOfficerDetails(characterCertificateDetail.SelectedCPCCR.ElementAt(0).PoliceOfficerLogId);
                    if (cpccr == null)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "CPCCR selected does not exist", FieldName = "SelectedCPCCR" });
                        throw new DirtyFormDataException("CPCCR selected does not exist");
                    }
                    _characterCertificateDetailsManager.Value.UpdateCharacterCertificateCPCCRNameAndServiceNumber(characterCertificateDetail.RequestId, cpccr.IdentificationNumber, cpccr.Name, cpccr.RankCode,  cpccr.RankName, characterCertificateDetail.ApproverId);
                }

                _coreCharacterCertificateService.CreateAndSaveRejectionCertificateDocument(characterCertificateDetail.FileRefNumber);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _characterCertificateDetailsManager.Value.RollBackAllTransactions();
                throw;
            }
        }
    }
}