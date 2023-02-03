using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSCharacterCertificateDetailsManager : BaseManager<PSSCharacterCertificateDetails>, IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>
    {
        private readonly IRepository<PSSCharacterCertificateDetails> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        private readonly Lazy<IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob>> _characterCertificateDetailsBlobManager;
        private readonly IPSSCharacterCertificateDetailsLogManager<PSSCharacterCertificateDetailsLog> _pccLogManager;


        public PSSCharacterCertificateDetailsManager(IRepository<PSSCharacterCertificateDetails> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices, Lazy<IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob>> characterCertificateDetailsBlobManager, IPSSCharacterCertificateDetailsLogManager<PSSCharacterCertificateDetailsLog> pccLogManager) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            _user = user;
            _characterCertificateDetailsBlobManager = characterCertificateDetailsBlobManager;
            _pccLogManager = pccLogManager;
        }


        /// <summary>
        /// Gets character certificate view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public IEnumerable<CharacterCertificateDetailsVM> GetCharacterCertificateRequestViewDetails(string fileRefNumber, long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>().Where(x => x.Request.FileRefNumber == fileRefNumber && x.Request.TaxEntity == new CBS.Core.Models.TaxEntity { Id = taxEntityId }).Select(x => new CharacterCertificateDetailsVM
                {
                    TaxEntity = new CBS.Core.HelperModels.TaxEntityViewModel
                    {
                        Address = x.Request.TaxEntity.Address
                    },
                    CharacterCertificateInfo = new CharacterCertificateRequestVM
                    {
                        ReasonForInquiryValue = x.ReasonValue,
                        //TribeValue = x.TribeValue,
                        SelectedStateOfOriginValue = x.StateOfOriginValue,
                        PlaceOfBirth = x.PlaceOfBirth,
                        DateOfBirth = x.DateOfBirth.Value.ToString("dd/MM/yyyy"),
                        DestinationCountryValue = x.DestinationCountryValue,
                        PreviouslyConvicted = x.PreviouslyConvicted,
                        PreviousConvictionHistory = x.PreviousConvictionHistory,
                        ServiceName = x.Request.Service.Name,
                        SelectedCountryOfOriginValue = x.CountryOfOriginValue,
                        SelectedCountryOfPassportValue = x.CountryOfPassportValue,
                        PassportNumber = x.PassportNumber
                    },
                    FileRefNumber = x.Request.FileRefNumber,
                    RequestStatus = (Models.Enums.PSSRequestStatus)x.Request.Status,
                    ApprovalNumber = x.Request.ApprovalNumber
                }).ToFuture();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get character certificate document info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable<CharacterCertificateDetailsVM></returns>
        public IEnumerable<CharacterCertificateDetailsVM> GetCharacterCertificateDocumentInfo(long requestId)
        {
            return _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>()
            .Where(sr => sr.Request.Id == requestId)
            .Select(characterCertificate => new CharacterCertificateDetailsVM
            {
                TaxEntity = new CBS.Core.HelperModels.TaxEntityViewModel
                {
                    Recipient = characterCertificate.Request.TaxEntity.Recipient,
                },
                CharacterCertificateInfo = new CharacterCertificateRequestVM
                {
                    ReasonForInquiryValue = characterCertificate.ReasonValue,
                    //TribeValue = characterCertificate.TribeValue,
                    SelectedStateOfOriginValue = characterCertificate.StateOfOriginValue,
                    DestinationCountryValue = characterCertificate.DestinationCountryValue,
                    StateName = characterCertificate.Request.Command.State.Name,
                    LGAName = characterCertificate.Request.Command.LGA.Name,
                    CommandName = characterCertificate.Request.Command.Name,
                    CommandAddress = characterCertificate.Request.Command.Address,
                    ServiceName = characterCertificate.Request.Service.Name,
                    CommandStateName = characterCertificate.Request.Command.State.Name,
                    CommandLgaName = characterCertificate.Request.Command.LGA.Name,
                    DateOfIssuance = (characterCertificate.DateOfIssuance != null) ? characterCertificate.DateOfIssuance.Value.ToString("dd/MM/yyyy") : "",
                    RequestTypeValue = characterCertificate.RequestType.Name,
                    PassportNumber = characterCertificate.PassportNumber,
                    SelectedCountryOfPassportValue  = characterCertificate.CountryOfPassportValue,
                    PlaceOfIssuance = characterCertificate.PlaceOfIssuance
                },
                RequestDate = characterCertificate.Request.CreatedAtUtc,
                ApprovalDate = characterCertificate.Request.UpdatedAtUtc.Value,
                ApprovalNumber = characterCertificate.Request.ApprovalNumber,
                CbsUser = new CBSUserVM { Name = characterCertificate.Request.CBSUser.Name }
            }).ToFuture();
        }

        /// <summary>
        /// Get request details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>ExtractRequestDetailsVM</returns>
        public CharacterCertificateRequestDetailsVM GetRequestDetails(long requestId)
        {
            return _transactionManager.GetSession().Query<PSSCharacterCertificateDetailsBlob>()
            .Where(sr => sr.Request == new PSSRequest { Id = requestId })
            .Select(cc => new CharacterCertificateRequestDetailsVM
            {
                TaxEntity = new TaxEntityViewModel
                {
                    Recipient = cc.Request.TaxEntity.Recipient,
                    PhoneNumber = cc.Request.TaxEntity.PhoneNumber,
                    RCNumber = cc.Request.TaxEntity.RCNumber,
                    Address = cc.Request.TaxEntity.Address,
                    Email = cc.Request.TaxEntity.Email,
                    TaxPayerIdentificationNumber = cc.Request.TaxEntity.TaxPayerIdentificationNumber,
                    SelectedStateName = cc.Request.TaxEntity.StateLGA.State.Name,
                    SelectedLGAName = cc.Request.TaxEntity.StateLGA.Name
                },
                Reason = cc.PSSCharacterCertificateDetails.ReasonValue,
                CountryOfOrigin = cc.PSSCharacterCertificateDetails.CountryOfOriginValue,
                StateName = cc.Request.Command.State.Name,
                LGAName = cc.Request.Command.LGA.Name,
                CommandName = cc.Request.Command.Name,
                CommandAddress = cc.Request.Command.Address,
                RequestId = cc.Request.Id,
                ServiceTypeId = cc.Request.Service.ServiceType,
                ServiceName = cc.Request.Service.Name,
                FileRefNumber = cc.Request.FileRefNumber,
                Status = cc.Request.Status,
                //Tribe = characterCertificate.TribeValue,
                StateOfOrigin = cc.PSSCharacterCertificateDetails.StateOfOriginValue,
                DateOfBirth = cc.PSSCharacterCertificateDetails.DateOfBirth.Value.ToString("dd/MM/yyyy"),
                PlaceOfBirth = cc.PSSCharacterCertificateDetails.PlaceOfBirth,
                DestinationCountry = cc.PSSCharacterCertificateDetails.DestinationCountryValue,
                CountryOfPassport = cc.PSSCharacterCertificateDetails.CountryOfPassportValue,
                PassportNumber = cc.PSSCharacterCertificateDetails.PassportNumber,
                PlaceOfIssuance = cc.PSSCharacterCertificateDetails.PlaceOfIssuance,
                DateOfIssuance = cc.PSSCharacterCertificateDetails.DateOfIssuance.HasValue ? cc.PSSCharacterCertificateDetails.DateOfIssuance.Value.ToString("dd/MM/yyyy") : null,
                RefNumber = cc.PSSCharacterCertificateDetails.RefNumber,
                IsPreviouslyConvicted = cc.PSSCharacterCertificateDetails.PreviouslyConvicted ? "YES" : "NO",
                PreviousConvictionHistory = cc.PSSCharacterCertificateDetails.PreviousConvictionHistory,
                InternationalPassportDataPageFileName = cc.InternationalPassportDataPageOriginalFileName,
                PassportPhotographFileName = cc.PassportPhotographOriginalFileName,
                SignatureFileName = cc.SignatureOriginalFileName,
                InternationalPassportDataPageFilePath = cc.InternationalPassportDataPageFilePath,
                InternationalPassportDataPageContentType = cc.InternationalPassportDataPageContentType,
                InternationalPassportDataPageBlob = cc.InternationalPassportDataPageBlob,
                PassportPhotographFilePath = cc.PassportPhotographFilePath,
                PassportPhotographContentType = cc.PassportPhotographContentType,
                PassportPhotographBlob = cc.PassportPhotographBlob,
                //SignatureFilePath = blobDetails.SignatureFilePath,
                //SignatureContentType = blobDetails.SignatureContentType,
                //SignatureBlob = blobDetails.SignatureBlob,
                ApprovalButtonName = cc.Request.FlowDefinitionLevel.ApprovalButtonName,
                CanInviteApplicant = (cc.Request.FlowDefinitionLevel.ApprovalButtonName == "Invite For Capture" && !cc.PSSCharacterCertificateDetails.IsBiometricEnrolled) ? true : false,
                DefinitionId = cc.Request.FlowDefinitionLevel.Definition.Id,
                Position = cc.Request.FlowDefinitionLevel.Position,
                IsBiometricsEnrolled = cc.PSSCharacterCertificateDetails.IsBiometricEnrolled,
                ApprovalPartialName = cc.Request.FlowDefinitionLevel.PartialName,
                CbsUser = new CBSUserVM { Name = cc.Request.CBSUser.Name, PhoneNumber = cc.Request.CBSUser.PhoneNumber, Email = cc.Request.CBSUser.Email }
            }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Gets character certificate request ref number and workflow definition details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>CharacterCertificateRequestDetailsVM</returns>
        public CharacterCertificateRequestDetailsVM GetRefNumberAndWorkflowDetails(long requestId)
        {
            return _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>().Where(x => x.Request == new PSSRequest { Id = requestId })
                .Select(x => new CharacterCertificateRequestDetailsVM
                {
                    RefNumber = x.RefNumber,
                    DefinitionId = x.Request.FlowDefinitionLevel.Definition.Id,
                    Position = x.Request.FlowDefinitionLevel.Position
                }).SingleOrDefault();
        }


        /// <summary>
        /// Updates Ref Number for character certificate details with specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="action"></param>
        /// <param name="details"></param>
        public void UpdateCharacterCertificateRefNumber(long requestId, string refNumber)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSCharacterCertificateDetails).Name;
                string refNumberName = nameof(PSSCharacterCertificateDetails.RefNumber);
                string requestIdName = nameof(PSSCharacterCertificateDetails.Request) + "_Id";
                string updatedAtName = nameof(PSSCharacterCertificateDetails.UpdatedAtUtc);

                var queryText = $"UPDATE {tableName} SET {refNumberName} = :refNumber, {updatedAtName} = :updateDate WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("refNumber", refNumber);
                query.SetParameter("requestId", requestId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating ref number for character certificate details with request id {0}, Exception message {1}", requestId, exception.Message));
                throw;
            }
        }

        /// <summary>
        /// Updates <see cref="PSSCharacterCertificateDetails.IsBiometricEnrolled"/> to true using <paramref name="detailsId"/>
        /// </summary>
        /// <param name="detailsId"></param>
        public void UpdateCharacterCertificateIsBiometricsEnrolledStatus(long detailsId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSCharacterCertificateDetails).Name;
                string isEnrolledName = nameof(PSSCharacterCertificateDetails.IsBiometricEnrolled);
                string detailsIdName = nameof(PSSCharacterCertificateDetails.Id);
                string updatedAtName = nameof(PSSCharacterCertificateDetails.UpdatedAtUtc);

                var queryText = $"UPDATE {tableName} SET {isEnrolledName} = :isBiometricEnrolled, {updatedAtName} = :updateDate WHERE {detailsIdName} = :detailsId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("isBiometricEnrolled", true);
                query.SetParameter("detailsId", detailsId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating ref number for character certificate details with request id {0}, Exception message {1}", detailsId, exception.Message));
                throw;
            }
        }

        /// <summary>
        /// Gets details required for generating character certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public CharacterCertificateDocumentVM GetCharacterCertificateDocumentDetails(string fileRefNumber)
        {
            try
            {
                CharacterCertificateDocumentVM characterCertificateViewModel = _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>()
                    .Where(x => x.Request.FileRefNumber == fileRefNumber)
                    .Select(x => new CharacterCertificateDocumentVM
                    {
                        CharacterCertificateDetailsId = x.Id,
                        RequestId = x.Request.Id,
                        ApprovalNumber = x.Request.ApprovalNumber,
                        RefNumber = x.RefNumber,
                        DateOfApproval = x.Request.UpdatedAtUtc.Value,
                        DateOfRejection = x.Request.UpdatedAtUtc.Value,
                        CustomerName = x.Request.TaxEntity.Recipient,
                        CountryOfPassport = (x.CountryOfPassport == null) ? "" : x.CountryOfPassportValue,
                        PassportNumber = (x.PassportNumber == null) ? "" : x.PassportNumber,
                        PlaceOfIssuance = (x.PlaceOfIssuance == null) ? "" : x.PlaceOfIssuance,
                        DateOfIssuance = x.DateOfIssuance,
                        ReasonForInquiry = x.ReasonValue,
                        DestinationCountry = (x.DestinationCountryValue == null) ? "" : x.DestinationCountryValue,
                        CPCCRName = x.CPCCRName,
                        RequestType = x.RequestType.Name,
                        CPCCRRankCode = x.CPCCRRankCode,
                        CPCCRRankName = x.CPCCRRankName
                    }).SingleOrDefault();


                PSSCharacterCertificateDetailsBlobVM blobDetails = _characterCertificateDetailsBlobManager.Value.GetCharacterCertificateBlobDetails(characterCertificateViewModel.RequestId);
                characterCertificateViewModel.PassportPhotoContentType = blobDetails.PassportPhotographContentType;
                characterCertificateViewModel.PassportPhotoBlob = blobDetails.PassportPhotographBlob;
                return characterCertificateViewModel;

            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception trying to get character certificate details for request with file ref number {0}", fileRefNumber));
                throw;
            }
        }

        /// <summary>
        /// Gets pending character certificate details by file number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>CharacterCertificateDocumentVM</returns>
        public CharacterCertificateDocumentVM GetPendingCharacterCertificateDocumentDetails(string fileRefNumber)
        {
            try
            {
                var characterCertificateVM = _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>()
                    .Where(x => x.Request.FileRefNumber == fileRefNumber && x.Request.Status == (int)Models.Enums.PSSRequestStatus.PendingApproval)
                    .Select(x => new CharacterCertificateDocumentVM
                    {
                        CharacterCertificateDetailsId = x.Id,
                        ApprovalNumber = x.Request.ApprovalNumber,
                        RefNumber = x.Request.FileRefNumber,
                        DateOfApproval = x.Request.UpdatedAtUtc.Value,
                        CustomerName = x.Request.CBSUser.Name,
                        PassportNumber = x.PassportNumber,
                        PlaceOfIssuance = x.PlaceOfIssuance,
                        DateOfIssuance = x.DateOfIssuance,
                        Tribe = x.Tribe.Name,
                        DateOfBirth = x.DateOfBirth.Value.ToString("dd/MM/yyyy"),
                        PlaceOfBirth = x.PlaceOfBirth,
                        ReasonForInquiry = x.ReasonValue,
                        PreviouslyConvicted = x.PreviouslyConvicted,
                        RequestId = x.Request.Id,
                        IsBiometricsEnrolled = x.IsBiometricEnrolled,
                        DestinationCountry = x.DestinationCountryValue,
                        FlowDefinitionLevelId = x.Request.FlowDefinitionLevel.Id,
                        HasApplicantBeenInvitedForCapture = x.IsApplicantInvitedForCapture,
                        ServiceTypeId = x.Request.Service.Id,
                        BiometricCaptureDueDate = x.BiometricCaptureDueDate,
                        CountryOfPassport = x.CountryOfPassportValue,
                        CountryOfOrigin = x.CountryOfOriginValue,
                        StateOfOrigin = x.CountryOfOriginValue,
                        RequestType = x.RequestType.Name,
                    }).SingleOrDefault();

                if (characterCertificateVM != null)
                {
                    PSSCharacterCertificateDetailsBlobVM blobDetails = _characterCertificateDetailsBlobManager.Value.GetCharacterCertificateBlobDetails(characterCertificateVM.RequestId);
                    characterCertificateVM.PassportPhotoContentType = blobDetails.PassportPhotographContentType;
                    characterCertificateVM.PassportPhotoBlob = blobDetails.PassportPhotographBlob;
                }

                return characterCertificateVM;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception trying to get character certificate details for request with file ref number {0}", fileRefNumber));
                throw;
            }
        }

        /// <summary>
        /// Gets pending character certificate details by file number excluding the passport blob
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>CharacterCertificateDocumentVM</returns>
        public CharacterCertificateDocumentVM GetPendingCharacterCertificateDocumentDetailsWithoutPassport(string fileRefNumber)
        {
            try
            {
                var characterCertificateVM = _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>()
                    .Where(x => x.Request.FileRefNumber == fileRefNumber && x.Request.Status == (int)Models.Enums.PSSRequestStatus.PendingApproval)
                    .Select(x => new CharacterCertificateDocumentVM
                    {
                        CharacterCertificateDetailsId = x.Id,
                        RefNumber = x.Request.FileRefNumber,
                        CustomerName = x.Request.CBSUser.Name,
                        PassportNumber = x.PassportNumber,
                        PlaceOfIssuance = x.PlaceOfIssuance,
                        DateOfIssuance = x.DateOfIssuance,
                        DateOfBirth = x.DateOfBirth.Value.ToString("dd/MM/yyyy"),
                        PlaceOfBirth = x.PlaceOfBirth,
                        ReasonForInquiry = x.ReasonValue,
                        PreviouslyConvicted = x.PreviouslyConvicted,
                        DestinationCountry = x.DestinationCountryValue,
                        CountryOfPassport = x.CountryOfPassportValue,
                    }).SingleOrDefault();

                return characterCertificateVM;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception trying to get character certificate details for request with file ref number {0}", fileRefNumber));
                throw;
            }
        }

        /// <summary>
        /// Check if a reference number for a character certificate has been populated
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>bool</returns>
        public bool CheckReferenceNumber(string fileRefNumber)
        {
            return _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>()
            .Where(x => x.Request.FileRefNumber == fileRefNumber && x.RefNumber != null).Count() > 0;
        }

        /// <summary>
        /// Update an applicant biometric invitation date
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="biometricCaptureDueDate"></param>
        public void UpdateApplicantBiometricInviteDetails(long requestId, DateTime biometricCaptureDueDate)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSCharacterCertificateDetails).Name;
                string isApplicantInvitedName = nameof(PSSCharacterCertificateDetails.IsApplicantInvitedForCapture);
                string applicantInvitationDateName = nameof(PSSCharacterCertificateDetails.CaptureInvitationDate);
                string requestIdName = nameof(PSSCharacterCertificateDetails.Request) + "_Id";
                string updatedAtName = nameof(PSSCharacterCertificateDetails.UpdatedAtUtc);
                string biometricCaptureDueDateName = nameof(PSSCharacterCertificateDetails.BiometricCaptureDueDate);

                var queryText = $"UPDATE {tableName} SET {isApplicantInvitedName} = :isApplicantInvited, {applicantInvitationDateName} = :applicantInvitationDate, {biometricCaptureDueDateName} = :biometricCaptureDueDate, {updatedAtName} = :updateDate WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("applicantInvitationDate", DateTime.Now.ToLocalTime());
                query.SetParameter("isApplicantInvited", true);
                query.SetParameter("requestId", requestId);
                query.SetParameter("biometricCaptureDueDate", biometricCaptureDueDate.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating applicant biometric capture invitation details with request id {0}, Exception message {1}", requestId, exception.Message));
                throw;
            }
        }

        /// <summary>
        /// Get biometric invitation details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetBiometricInvitationDetails(long requestId)
        {
            return _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>()
            .Where(sr => sr.Request == new PSSRequest { Id = requestId })
            .Select(characterCertificate => new CharacterCertificateRequestDetailsVM
            {
                TaxEntity = new TaxEntityViewModel
                {
                    PhoneNumber = characterCertificate.Request.TaxEntity.PhoneNumber,
                    Recipient = characterCertificate.Request.TaxEntity.Recipient,
                    Email = characterCertificate.Request.TaxEntity.Email,
                },
                IsApplicantInvitedForCapture = characterCertificate.IsApplicantInvitedForCapture,
                CommandName = characterCertificate.Request.Command.Name,
                CommandAddress = characterCertificate.Request.Command.Address,
                FileRefNumber = characterCertificate.Request.FileRefNumber
            }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Updates character certificate details CPCCR Name and Service Number
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="serviceNumber"></param>
        /// <param name="name"></param>
        /// <param name="rankCode"></param>
        /// <param name="adminId"></param>
        public void UpdateCharacterCertificateCPCCRNameAndServiceNumber(long requestId, string serviceNumber, string name, string rankCode, string rankName, int adminId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSCharacterCertificateDetails).Name;
                string nameLabel = nameof(PSSCharacterCertificateDetails.CPCCRName);
                string rankCodeLabel = nameof(PSSCharacterCertificateDetails.CPCCRRankCode);
                string rankNameLabel = nameof(PSSCharacterCertificateDetails.CPCCRRankName);
                string serviceNumberLabel = nameof(PSSCharacterCertificateDetails.CPCCRServiceNumber);
                string requestIdName = nameof(PSSCharacterCertificateDetails.Request) + "_Id";
                string addedyByName = nameof(PSSCharacterCertificateDetails.CPCCRAddedBy) + "_Id";
                string updatedAtName = nameof(PSSCharacterCertificateDetails.UpdatedAtUtc);

                var queryText = $"UPDATE {tableName} SET {nameLabel} = :cpccrName, {rankCodeLabel} = :cpccrRankCode, {rankNameLabel} = :cpccrRankName, {serviceNumberLabel} = :cpccrServiceNumber, {addedyByName} = :adminId, {updatedAtName} = :updateDate WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("cpccrName", name);
                query.SetParameter("cpccrRankCode", rankCode);
                query.SetParameter("cpccrRankName", rankName);
                query.SetParameter("cpccrServiceNumber", serviceNumber);
                query.SetParameter("adminId", adminId);
                query.SetParameter("requestId", requestId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating CPCCR name, rank code and service number for character certificate details with request id {0}, Exception message {1}", requestId, exception.Message));
                throw;
            }
        }

        /// <summary>
        /// Gets details required for generating character certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>CharacterCertificateDetailsUpdateVM</returns>
        public CharacterCertificateDetailsUpdateVM GetCharacterCertificateDetailsForEdit(string fileRefNumber)
        {
            try
            {
                var characterCertificateVM = _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>()
                    .Where(x => x.Request.FileRefNumber == fileRefNumber && x.Request.Status == (int)Models.Enums.PSSRequestStatus.PendingApproval)
                    .Select(x => new CharacterCertificateDetailsUpdateVM
                    {
                        CharacterCertificateDetailsId = x.Id,
                        PassportNumber = x.PassportNumber,
                        DateOfIssuance = x.DateOfIssuance,
                        DestinationCountryId = x.DestinationCountry.Id,
                        DestinationCountry =x.DestinationCountryValue,
                        CustomerName = x.Request.CBSUser.Name,
                        ReasonForInquiry = x.ReasonValue,
                        FileNumber = x.Request.FileRefNumber,
                        DateOfBirth = x.DateOfBirth.Value.ToString("dd/MM/yyyy"),
                        PlaceOfBirth = x.PlaceOfBirth,
                        CountryOfPassport = x.CountryOfPassportValue,
                        CountryOfPassportId = x.CountryOfPassport.Id,
                        PlaceOfIssuance = x.PlaceOfIssuance
                    }).SingleOrDefault();

                return characterCertificateVM;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception trying to get character certificate details for request with file ref number {0}", fileRefNumber));
                throw;
            }
        }

        /// <summary>
        /// Update Destination Country, Passport Number and Passport Date of Issuance for character certificate details with specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="action"></param>
        /// <param name="details"></param>
        /// <returns>bool</returns>
        public bool UpdateCharacterCertificateDetails(CharacterCertificateDetailsUpdateVM model)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSCharacterCertificateDetails).Name;
                string destinationCountryName = nameof(PSSCharacterCertificateDetails.DestinationCountry)+"_Id";
                string destinationCountryValueName = nameof(PSSCharacterCertificateDetails.DestinationCountryValue);
                string characterCertificateIdName = nameof(PSSCharacterCertificateDetails.Id);
                string passportNumberName = nameof(PSSCharacterCertificateDetails.PassportNumber);
                string passportDateofIssuanceName = nameof(PSSCharacterCertificateDetails.DateOfIssuance);
                string updatedAtName = nameof(PSSCharacterCertificateDetails.UpdatedAtUtc);
                string placeOfIssuanceName = nameof(PSSCharacterCertificateDetails.PlaceOfIssuance);

                var queryText = $"UPDATE {tableName} SET {destinationCountryName} = :destinationCountryId, {destinationCountryValueName} = :destinationCountryValue, {passportNumberName} = :passportNumber, {passportDateofIssuanceName} = :passportDateofIssuance, {updatedAtName} = :updateDate, {placeOfIssuanceName} = :placeOfIssuance  WHERE {characterCertificateIdName} = :characterCertificateId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("destinationCountryId", model.DestinationCountryId);
                query.SetParameter("destinationCountryValue", model.DestinationCountry);
                query.SetParameter("characterCertificateId", model.CharacterCertificateDetailsId);
                query.SetParameter("passportNumber", model.PassportNumber);
                query.SetParameter("passportDateofIssuance", model.DateOfIssuance);
                query.SetParameter("placeOfIssuance", model.PlaceOfIssuance);
                query.ExecuteUpdate();

                query = _transactionManager.GetSession().CreateSQLQuery(_pccLogManager.LogNewEntryQueryStringValue(model.CharacterCertificateDetailsId));
                query.SetParameter("characterCertificateDetailsId", model.CharacterCertificateDetailsId);
                query.ExecuteUpdate();

                return true;
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating character certificate details with file number {0}, Exception message {1}", model.FileNumber, exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Gets character certificate details id with specified file ref number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns></returns>
        public long GetCharacterCertificateDetailsIdWithFileNumber(string fileNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSCharacterCertificateDetails>().Where(x => x.Request.FileRefNumber == fileNumber).Select(x => x.Id).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}