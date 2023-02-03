using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.IO;
using System.Linq;
using Parkway.CBS.Core.Models;
using Newtonsoft.Json;
using Parkway.DataExporter.Implementations.Models;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.CacheProvider;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreExtractService : ICoreExtractService
    {
        private readonly IExtractDetailsManager<ExtractDetails> _repo;
        private readonly IPSSRequestExtractDetailsCategoryManager<PSSRequestExtractDetailsCategory> _pssReqExtractDetailsCategoryRepo;
        private readonly IPSSExtractDocumentManager<PSSExtractDocument> _pssExtractDocumentRepo;
        private readonly IPSSAdminSignatureUploadManager<PSSAdminSignatureUpload> _pssAdminSignatureUploadManager;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<IPDFExportEngine> _exportToPDF = new Lazy<IPDFExportEngine>(() =>
        {
            return new PdfEngine();
        });

        public CoreExtractService(IExtractDetailsManager<ExtractDetails> repo, IOrchardServices orchardServices, IPSSRequestExtractDetailsCategoryManager<PSSRequestExtractDetailsCategory> pssReqExtractDetailsCategoryRepo, IPSSExtractDocumentManager<PSSExtractDocument> pssExtractDocumentRepo, IPSSAdminSignatureUploadManager<PSSAdminSignatureUpload> pssAdminSignatureUploadManager)
        {
            _repo = repo;
            _orchardServices = orchardServices;
            _pssReqExtractDetailsCategoryRepo = pssReqExtractDetailsCategoryRepo;
            _pssExtractDocumentRepo = pssExtractDocumentRepo;
            _pssAdminSignatureUploadManager = pssAdminSignatureUploadManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Retrieves PSS Extract Document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateExtractDocument(string fileRefNumber, bool returnByte = false)
        {
            Template template = null;
            ExtractDocumentVM extractDocument = _pssExtractDocumentRepo.GetExtractDocumentDetails(fileRefNumber);
            if (extractDocument == null)
            {
                extractDocument = _repo.GetExtractDocumentDetails(fileRefNumber);
                if (extractDocument == null) { throw new NoRecordFoundException("404 for PSS Extract with File Ref Number " + fileRefNumber); }
                template = TemplateUtil.RazorTemplateFor("ExtractDocument", _orchardServices.WorkContext.CurrentSite.SiteName);
                extractDocument.ExtractCategoriesConcat = string.Join(", ", _pssReqExtractDetailsCategoryRepo.GetExtractCategoriesForExtractDocument(fileRefNumber).Distinct());
            }
            else
            {
                template = JsonConvert.DeserializeObject<CertificateTemplateVM>(extractDocument.Template).Template;
            }

            extractDocument.IncidentDate = extractDocument.IncidenDateAndTimeParsed.Value.ToString("dd/MM/yyyy");
            extractDocument.IncidentTime = extractDocument.IncidenDateAndTimeParsed.Value.ToString("HH:mm");

            string fileName = fileRefNumber + ".pdf";

            extractDocument.LogoURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.PoliceLogo.ToDescription());
            extractDocument.PossapLogoUrl = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.PossapLogo.ToDescription());
            extractDocument.PSSExtractDocumentStripURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.CertificateStrip.ToDescription()).Replace("C:\\", "\\");
            extractDocument.PSSExtractDocumentBGPath = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.ExtractDocumentBG.ToDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
            string vSavingPath = template.SavingPath + "/" + _orchardServices.WorkContext.CurrentSite.SiteName + "/";
            string fileFullPath = System.Web.HttpContext.Current.Server.MapPath(vSavingPath + fileName);

            CBS.Core.StateConfig.StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            CBS.Core.StateConfig.Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.BaseURL.ToString())?.FirstOrDefault();
            if (node == null) { throw new Exception("Unable to fetch Base URL from StateConfig in Core Extract Service"); }
            extractDocument.ValidateDocumentUrl = $"{node.Value}/p/validate-document";

            if (!returnByte)
            {
                if (File.Exists(fileFullPath))
                {
                    return new CreateCertificateDocumentVM { SavedPath = fileFullPath, FileName = fileName };
                }
            }

            var savedFileDir = System.Web.HttpContext.Current.Server.MapPath(vSavingPath);
            Directory.CreateDirectory(savedFileDir);
            if (extractDocument.DPOSignatureBlob != null && extractDocument.DPOSignatureContentType != null)
            { extractDocument.DPOSignatureImageSrc = $"data:{extractDocument.DPOSignatureContentType};base64,{extractDocument.DPOSignatureBlob}"; }
            //Check if the file already exist in the folder
            byte[] certificateByte = null;
            if (returnByte)
            {
                certificateByte = _exportToPDF.Value.DownloadAsPdfNRecoLibNoBorders(null, template.File, extractDocument, template.BasePath);
            }
            else
            {
                _exportToPDF.Value.SaveAsPdfNRecoLibNoBorders(null, template.File, extractDocument, fileName, vSavingPath, template.BasePath);
            }

            return new CreateCertificateDocumentVM { SavedPath = fileFullPath, FileName = fileName, DocByte = certificateByte };
        }


        /// <summary>
        /// Checks if there is an extract request with specified file ref number that has been approved
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public bool CheckIfApprovedExtractRequestExists(string fileRefNumber)
        {
            return _repo.Count(x => x.Request.FileRefNumber == fileRefNumber && x.Request.Status == (int)PSSRequestStatus.Approved) > 0;
        }


        /// <summary>
        /// Checks if there is an extract request with specified file ref number that has been approved for currenly logged in user
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public bool CheckIfApprovedExtractRequestExists(string fileRefNumber, long taxEntityId)
        {
            return _repo.Count(x => x.Request.FileRefNumber == fileRefNumber && x.Request.TaxEntity == new TaxEntity { Id = taxEntityId } && x.Request.Status == (int)PSSRequestStatus.Approved) > 0;
        }


        /// <summary>
        /// Generates and saves extract document for request with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        public void CreateAndSaveExtractDocument(string fileRefNumber)
        {
            ExtractDocumentVM extractDocument = _repo.GetExtractDocumentDetails(fileRefNumber);
            if (extractDocument == null) { throw new NoRecordFoundException("404 for PSS Extract with File Ref Number " + fileRefNumber); }
            extractDocument.ExtractCategories = _pssReqExtractDetailsCategoryRepo.GetExtractCategoriesForExtractDocument(fileRefNumber).Distinct();
            var template = TemplateUtil.RazorTemplateFor("ExtractDocument", _orchardServices.WorkContext.CurrentSite.SiteName);

            PSSRequestDetailsVM adminDetail = null;
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                adminDetail = ObjectCacheProvider.GetCachedObject<PSSRequestDetailsVM>(_orchardServices.WorkContext.CurrentSite.SiteName, $"USSD-Admin-{fileRefNumber.ToUpper()}");
                if(adminDetail == null)
                {
                    throw new NoRecordFoundException($"Unable to get cached admin user for file number {fileRefNumber} USSD request approval.");
                }
            }

            int adminUserId = _orchardServices.WorkContext.CurrentUser != null ? _orchardServices.WorkContext.CurrentUser.Id : adminDetail.ApproverId;

            PSSAdminSignatureUploadVM adminSignature = _pssAdminSignatureUploadManager.GetActiveAdminSignature(adminUserId);
            PSSExtractDocument extractDocumentModel = new PSSExtractDocument
            {
                ExtractDetails = new ExtractDetails { Id = extractDocument.ExtractDetailsId },
                CommandName = extractDocument.CommandName,
                CommandStateName = extractDocument.CommandStateName,
                ApprovalDate = extractDocument.ApprovalDate,
                ApprovalNumber = extractDocument.ApprovalNumber,
                DiarySerialNumber = extractDocument.DiarySerialNumber,
                IncidenDateAndTime = extractDocument.IncidenDateAndTimeParsed.Value,
                CrossRef = extractDocument.CrossRef,
                ExtractCategories = string.Join(", ", extractDocument.ExtractCategories),
                DPOName = extractDocument.DPOName,
                Content = extractDocument.Content,
                ExtractDocumentTemplate = JsonConvert.SerializeObject(new CertificateTemplateVM { Template = template, TemplateName = "ExtractDocument" }),
                DPORankCode = extractDocument.DPORankCode,
                DPOSignatureBlob = adminSignature?.SignatureBlob,
                DPOSignatureContentType = adminSignature?.SignatureContentType
            };

            if (!_pssExtractDocumentRepo.Save(extractDocumentModel))
            {
                _pssExtractDocumentRepo.RollBackAllTransactions();
                throw new CouldNotSaveRecord($"Could not save extact document for request with file ref number --- {fileRefNumber}");
            }
        }


        /// <summary>
        /// Retrieves Default PSS Extract Document before approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateDefaultExtractDocument(string fileRefNumber)
        {
            ExtractDocumentVM extractDocument = _repo.GetExtractDocumentDetails(fileRefNumber);
            if (extractDocument == null) { throw new NoRecordFoundException("404 for PSS Extract with File Ref Number " + fileRefNumber); }
            Template template = TemplateUtil.RazorTemplateFor("DraftExtractDocument", _orchardServices.WorkContext.CurrentSite.SiteName);

            extractDocument.ExtractCategoriesConcat = string.Join(", ", _pssReqExtractDetailsCategoryRepo.GetExtractCategoriesForExtractDocument(fileRefNumber).Distinct());

            extractDocument.LogoURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.PoliceLogo.ToDescription());
            extractDocument.PossapLogoUrl = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.PossapLogo.ToDescription());
            extractDocument.PSSExtractDocumentStripURL = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.CertificateStrip.ToDescription()).Replace("C:\\", "\\");
            extractDocument.PSSExtractDocumentBGPath = System.Web.HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.ExtractDocumentBG.ToDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");

            byte[] documentByte = _exportToPDF.Value.DownloadAsPdfNRecoLibNoBorders(null, template.File, extractDocument, template.BasePath);

            return new CreateCertificateDocumentVM { DocByte = documentByte };
        }


        /// <summary>
        /// Indicates if the extract document with the specified approval number has a signature attached
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <returns></returns>
        public bool CheckIfExtractDocumentIsSigned(string approvalNumber)
        {
            return _pssExtractDocumentRepo.Count(x => (x.ApprovalNumber == approvalNumber) && (x.DPOSignatureBlob != null)) > 0;
        }
    }
}