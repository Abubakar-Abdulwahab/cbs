using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Models;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.IO;
using System.Linq;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreCharacterCertificateService : ICoreCharacterCertificateService
    {
        private readonly ICharacterCertificateBiometricsManager<CharacterCertificateBiometrics> _biometricsManagerRepo;
        private readonly IPSSCharacterCertificateManager<PSSCharacterCertificate> _certificateRepo;
        private readonly IPSSCharacterCertificateRejectionManager<PSSCharacterCertificateRejection> _certificateRejectionRepo;
        private readonly Lazy<IPDFExportEngine> _exportToPDF = new Lazy<IPDFExportEngine>(() =>
        {
            return new PdfEngine();
        });

        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _flowDefinitionLevelManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSAdminSignatureUploadManager<PSSAdminSignatureUpload> _pssAdminSignatureUploadManager;
        private readonly IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> _repo;
        private readonly IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob> _pccBlobManager;


        public ILogger Logger { get; set; }

        public CoreCharacterCertificateService(IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> repo, IOrchardServices orchardServices, IPSSCharacterCertificateManager<PSSCharacterCertificate> certificateRepo, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> flowDefinitionLevelManager, ICharacterCertificateBiometricsManager<CharacterCertificateBiometrics> biometricsManagerRepo, IPSSAdminSignatureUploadManager<PSSAdminSignatureUpload> pssAdminSignatureUploadManager, IPSSCharacterCertificateRejectionManager<PSSCharacterCertificateRejection> certificateRejectionRepo, IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob> pccBlobManager)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _certificateRepo = certificateRepo;
            Logger = NullLogger.Instance;
            _flowDefinitionLevelManager = flowDefinitionLevelManager;
            _biometricsManagerRepo = biometricsManagerRepo;
            _pssAdminSignatureUploadManager = pssAdminSignatureUploadManager;
            _certificateRejectionRepo = certificateRejectionRepo;
            _pccBlobManager = pccBlobManager;
        }

        /// <summary>
        /// Checks if there is a character certificate request with specified file ref number that has been approved
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public bool CheckIfApprovedCharacterCertificateRequestExists(string fileRefNumber)
        {
            return _repo.Count(x => x.Request.FileRefNumber == fileRefNumber && x.Request.Status == (int)PSSRequestStatus.Approved) > 0;
        }

        /// <summary>
        /// Checks if there is a character certificate request with specified file ref number that has been approved for the currently logged in user
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public bool CheckIfApprovedCharacterCertificateRequestExists(string fileRefNumber, long taxEntityId)
        {
            return _repo.Count(x => x.Request.FileRefNumber == fileRefNumber && x.Request.TaxEntity == new CBS.Core.Models.TaxEntity { Id = taxEntityId } && x.Request.Status == (int)PSSRequestStatus.Approved) > 0;
        }

        /// <summary>
        /// Checks if there is a character certificate request with specified file ref number that has been rejected
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public bool CheckIfRejectedCharacterCertificateRequestExists(string fileRefNumber, long taxEntityId)
        {
            return _repo.Count(x => x.Request.FileRefNumber == fileRefNumber && x.Request.TaxEntity == new CBS.Core.Models.TaxEntity { Id = taxEntityId } && x.Request.Status == (int)PSSRequestStatus.Rejected) > 0;
        }

        /// <summary>
        /// Checks if there is a character certificate request with specified file ref number that has been rejected
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public bool CheckIfRejectedCharacterCertificateRequestExists(string fileRefNumber)
        {
            return _repo.Count(x => x.Request.FileRefNumber == fileRefNumber && x.Request.Status == (int)PSSRequestStatus.Rejected) > 0;
        }

        /// <summary>
        /// Check if this definition level will be the one to enter the reference number. 
        /// This returns the next approval button name
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="position"></param>
        /// <returns>string</returns>
        public string CheckIfCanShowRefNumberForm(int definitionId, int position)
        {
            return _flowDefinitionLevelManager.CheckIfCanShowRefNumberForm(definitionId, position);
        }

        /// <summary>
        /// Checks if the character certificate with the specified approval number has a signature attached
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <returns></returns>
        public bool CheckIfCharacterCertificateIsSigned(string approvalNumber)
        {
            return _certificateRepo.Count(x => (x.ApprovalNumber == approvalNumber) && (x.CentralRegistrarSignatureBlob != null)) > 0;
        }

        /// <summary>
        /// Generates and saves character certificate for request with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        public void CreateAndSaveCertificateDocument(string fileRefNumber)
        {
            CharacterCertificateDocumentVM certificate = _repo.GetCharacterCertificateDocumentDetails(fileRefNumber);
            if (certificate == null) { throw new NoRecordFoundException("404 for PSS Character Certificate with File Ref Number " + fileRefNumber); }

            var template = TemplateUtil.RazorTemplateFor("CharacterCertificate", _orchardServices.WorkContext.CurrentSite.SiteName);
            PSSAdminSignatureUploadVM adminSignature = _pssAdminSignatureUploadManager.GetActiveAdminSignature(_orchardServices.WorkContext.CurrentUser.Id);
            PSSCharacterCertificate certificateModel = new PSSCharacterCertificate
            {
                PSSCharacterCertificateDetails = new PSSCharacterCertificateDetails { Id = certificate.CharacterCertificateDetailsId },
                ApprovalNumber = certificate.ApprovalNumber,
                PassportPhotoBlob = certificate.PassportPhotoBlob,
                PassportPhotoContentType = certificate.PassportPhotoContentType,
                RefNumber = certificate.RefNumber,
                DateOfApproval = certificate.DateOfApproval,
                CustomerName = certificate.CustomerName,
                PassportNumber = certificate.PassportNumber,
                PlaceOfIssuance = certificate.PlaceOfIssuance,
                DateOfIssuance = certificate.DateOfIssuance,
                ReasonForInquiry = certificate.ReasonForInquiry,
                DestinationCountry = certificate.DestinationCountry,
                CharacterCertificateTemplate = JsonConvert.SerializeObject(new CertificateTemplateVM { Template = template, TemplateName = "CharacterCertificate" }),
                NameOfCentralRegistrar = certificate.CPCCRName,
                CPCCRRankCode = certificate.CPCCRRankCode,
                CPCCRRankName = certificate.CPCCRRankName,
                CentralRegistrarSignatureBlob = adminSignature?.SignatureBlob,
                CentralRegistrarSignatureContentType = adminSignature?.SignatureContentType,
                CountryOfPassport = certificate.CountryOfPassport
            };

            if (!_certificateRepo.Save(certificateModel))
            {
                _certificateRepo.RollBackAllTransactions();
                throw new CouldNotSaveRecord($"Could not save character certificate for request with file ref number --- {fileRefNumber}");
            }
        }

        public void CreateAndSaveRejectionCertificateDocument(string fileRefNumber)
        {

            CharacterCertificateDocumentVM certificate = _repo.GetCharacterCertificateDocumentDetails(fileRefNumber);
            if (certificate == null) { throw new NoRecordFoundException("404 for PSS Character Certificate with File Ref Number " + fileRefNumber); }

            Template template = TemplateUtil.RazorTemplateFor("CharacterCertificateRejection", _orchardServices.WorkContext.CurrentSite.SiteName);

            PSSCharacterCertificateRejection certificateModel = new PSSCharacterCertificateRejection
            {
                PSSCharacterCertificateDetails = new PSSCharacterCertificateDetails { Id = certificate.CharacterCertificateDetailsId },
                PassportPhotoBlob = certificate.PassportPhotoBlob,
                PassportPhotoContentType = certificate.PassportPhotoContentType,
                RefNumber = certificate.RefNumber ?? "",
                CustomerName = certificate.CustomerName,
                PassportNumber = certificate.PassportNumber,
                PlaceOfIssuance = certificate.PlaceOfIssuance,
                DateOfIssuance = certificate.DateOfIssuance,
                ReasonForInquiry = certificate.ReasonForInquiry,
                DestinationCountry = certificate.DestinationCountry,
                DateOfRejection = certificate.DateOfRejection,
                CharacterCertificateRejectionTemplate = JsonConvert.SerializeObject(new CertificateTemplateVM { Template = template, TemplateName = "CharacterCertificateRejection" }), 
                NameOfCentralRegistrar = certificate.CPCCRName,
                CPCCRRankCode = certificate.CPCCRRankCode,
                CountryOfPassport = certificate.CountryOfPassport
            };

            if (!_certificateRejectionRepo.Save(certificateModel))
            {
                _certificateRepo.RollBackAllTransactions();
                throw new CouldNotSaveRecord($"Could not save rejection character certificate for request with file ref number --- {fileRefNumber}");
            }
        }

        /// <summary>
        /// Retrieves PSS Character Certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateCertificateDocument(string fileRefNumber, bool returnByte = false)
        {
            Template template = null;
            //fetch certificate details from certificate table
            CharacterCertificateDocumentVM certificate = _certificateRepo.GetCertificateDetails(fileRefNumber);
            if (certificate == null)
            {
                //fetch certificate details from the certificate details table if they do not exist in the certificate table
                certificate = _repo.GetCharacterCertificateDocumentDetails(fileRefNumber);
                template = TemplateUtil.RazorTemplateFor("CharacterCertificate", _orchardServices.WorkContext.CurrentSite.SiteName);
                if (certificate == null) { throw new NoRecordFoundException("404 for PSS Character Certificate with File Ref Number " + fileRefNumber); }
            }
            else
            {
                template = JsonConvert.DeserializeObject<CertificateTemplateVM>(certificate.Template).Template;
            }

            string fileName = fileRefNumber + ".pdf";

            certificate.LogoURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.PccPoliceLogo.GetDescription());
            certificate.PossapLogoUrl = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.PossapLogo.GetDescription());
            certificate.PSSCertificateStripURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.CertificateStrip.GetDescription()).Replace("C:\\", "\\");
            certificate.PSSCharacterCertificateBGPath = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.CharacterCertificateBG.GetDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
            certificate.PccEStampUrl = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.PccEStampUrl.GetDescription());
            string vSavingPath = template.SavingPath + "/" + _orchardServices.WorkContext.CurrentSite.SiteName + "/";
            string fileFullPath = System.Web.HttpContext.Current.Server.MapPath(vSavingPath + fileName);

            CBS.Core.StateConfig.StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            CBS.Core.StateConfig.Node node = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.ValidateDocumentURL))?.FirstOrDefault();
            if (node == null) { throw new Exception("Unable to fetch Validate Document URL from StateConfig in Core Character Certificate Service"); }
            certificate.ValidateDocumentUrl = $"{node.Value}";

            if (!returnByte)
            {
                if (File.Exists(fileFullPath))
                {
                    return new CreateCertificateDocumentVM { SavedPath = fileFullPath, FileName = fileName };
                }
            }

            var savedFileDir = System.Web.HttpContext.Current.Server.MapPath(vSavingPath);
            Directory.CreateDirectory(savedFileDir);
            certificate.PassportPhotoImageSrc = $"data:{certificate.PassportPhotoContentType};base64,{certificate.PassportPhotoBlob}";
            if (certificate.CPCCRSignatureBlob != null && certificate.CPCCRSignatureContentType != null) 
            { certificate.CPCCRSignatureImageSrc = $"data:{certificate.CPCCRSignatureContentType};base64,{certificate.CPCCRSignatureBlob}"; }
            //Check if the file already exist in the folder
            byte[] certificateByte = null;
            if (returnByte)
            {
                certificateByte = _exportToPDF.Value.DownloadAsPdfNRecoLibNoBorders(null, template.File, certificate, template.BasePath);
            }
            else
            {
                _exportToPDF.Value.SaveAsPdfNRecoLibNoBorders(null, template.File, certificate, fileName, vSavingPath, template.BasePath);
            }

            return new CreateCertificateDocumentVM { SavedPath = fileFullPath, FileName = fileName, DocByte = certificateByte };
        }

        /// <summary>
        /// Retrieves PSS Character Certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateRejectionCertificateDocument(string fileRefNumber, bool returnByte = false)
        {
            if (!CheckIfRejectedCharacterCertificateRequestExists(fileRefNumber))
            {
                throw new NoRecordFoundException("404 or request is not rejected for PSS Character Certificate request. File Ref Number " + fileRefNumber);
            }

            Template template = null;
            //fetch certificate details from certificate table
            CharacterCertificateDocumentVM certificate = _certificateRejectionRepo.GetCertificateDetails(fileRefNumber);
            if (certificate == null)
            {
                //fetch certificate details from the certificate details table if they do not exist in the certificate table
                certificate = _certificateRejectionRepo.GetCertificateDetails(fileRefNumber);
                if (certificate == null) { throw new NoRecordFoundException("404 for PSS Character Certificate with File Ref Number " + fileRefNumber); }
                template = TemplateUtil.RazorTemplateFor("CharacterCertificateRejection", _orchardServices.WorkContext.CurrentSite.SiteName);
            }
            else
            {
                template = JsonConvert.DeserializeObject<CertificateTemplateVM>(certificate.Template).Template;
            }

            string fileName = fileRefNumber + ".pdf";

            certificate.LogoURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.PccPoliceLogo.GetDescription());
            certificate.PossapLogoUrl = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.PossapLogo.GetDescription());
            certificate.PSSCertificateStripURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.CertificateStrip.GetDescription()).Replace("C:\\", "\\");
            certificate.PSSCharacterCertificateBGPath = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.RejectedCharacterCertificateBG.GetDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
            string vSavingPath = template.SavingPath + "/" + _orchardServices.WorkContext.CurrentSite.SiteName + "/";
            string fileFullPath = System.Web.HttpContext.Current.Server.MapPath(vSavingPath + fileName);

            CBS.Core.StateConfig.StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            CBS.Core.StateConfig.Node node = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.ValidateDocumentURL))?.FirstOrDefault();
            if (node == null) { throw new Exception("Unable to fetch Validate Document URL from StateConfig in Core Character Certificate Service"); }
            certificate.ValidateDocumentUrl = $"{node.Value}";

            if (!returnByte)
            {
                //Check if the file already exist in the folder
                if (File.Exists(fileFullPath))
                {
                    return new CreateCertificateDocumentVM { SavedPath = fileFullPath, FileName = fileName };
                }
            }

            var savedFileDir = System.Web.HttpContext.Current.Server.MapPath(vSavingPath);
            Directory.CreateDirectory(savedFileDir);
            certificate.PassportPhotoImageSrc = $"data:{certificate.PassportPhotoContentType};base64,{certificate.PassportPhotoBlob}";

            byte[] certificateByte = null;
            if (returnByte)
            {
                certificateByte = _exportToPDF.Value.DownloadAsPdfNRecoLibNoBorders(null, template.File, certificate, template.BasePath);
            }
            else
            {
                _exportToPDF.Value.SaveAsPdfNRecoLibNoBorders(null, template.File, certificate, fileName, vSavingPath, template.BasePath);
            }

            return new CreateCertificateDocumentVM { SavedPath = fileFullPath, FileName = fileName, DocByte = certificateByte };
        }

        /// <summary>
        /// Retrieves Default PSS Character Certificate before approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateDefaultCertificateDocument(string fileRefNumber)
        {
            CharacterCertificateDocumentVM certificate = _repo.GetCharacterCertificateDocumentDetails(fileRefNumber);
            if (certificate == null) { throw new NoRecordFoundException("404 for PSS Character Certificate with File Ref Number " + fileRefNumber); }

            var template = TemplateUtil.RazorTemplateFor("DraftCharacterCertificate", _orchardServices.WorkContext.CurrentSite.SiteName);

            certificate.LogoURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.PccPoliceLogo.GetDescription());
            certificate.PossapLogoUrl = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.PossapLogo.GetDescription());
            certificate.PSSCertificateStripURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.CertificateStrip.GetDescription()).Replace("C:\\", "\\");
            certificate.PSSCharacterCertificateBGPath = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.GetDescription() + PSSTenantConfigKeys.CharacterCertificateBG.GetDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
            certificate.PassportPhotoImageSrc = $"data:{certificate.PassportPhotoContentType};base64,{certificate.PassportPhotoBlob}";
            byte[] certificateByte = _exportToPDF.Value.DownloadAsPdfNRecoLibNoBorders(null, template.File, certificate, template.BasePath);

            return new CreateCertificateDocumentVM { DocByte = certificateByte };
        }

        /// <summary>
        /// Get biometric invitation details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetBiometricInvitationDetails(long requestId)
        {
            return _repo.GetBiometricInvitationDetails(requestId);
        }

        /// <summary>
        /// Gets Character Certificate Biometrics by <paramref name="requestId"/>
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>CharacterCertificateBiometricsVM</returns>
        public CharacterCertificateBiometricsVM GetCharacterCertificateBiometric(long requestId)
        {
            return _biometricsManagerRepo.GetCharacterCertificateBiometrics(requestId);
        }

        /// <summary>
        /// Update an applicant biometric invitation date
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="biometricCaptureDueDate"></param>
        public void UpdateApplicantBiometricInvitationDetails(long requestId, DateTime biometricCaptureDueDate)
        {
            try
            {
                _repo.UpdateApplicantBiometricInviteDetails(requestId, biometricCaptureDueDate);
            }
            catch (Exception)
            {
                _repo.RollBackAllTransactions();
                throw;
            }
        }


        public bool ChangePassportPhoto(string filePathName, CharacterCertificateDocumentVM certDeets)
        {
            return _pccBlobManager.UpdatePassportPhoto(filePathName, certDeets.CharacterCertificateDetailsId);
        }

    }
}