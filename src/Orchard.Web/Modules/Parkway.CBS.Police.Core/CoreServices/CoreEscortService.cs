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
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreEscortService : ICoreEscortService
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSEscortDetailsManager<PSSEscortDetails> _pssEscortDetailsManager;
        private readonly IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> _policeOfficerDeploymentLogManager;
        private readonly IPSSDispatchNoteManager<PSSDispatchNote> _pssDispatchNoteManager;
        private readonly Lazy<IPDFExportEngine> _exportToPDF = new Lazy<IPDFExportEngine>(() =>
        {
            return new PdfEngine();
        });


        public CoreEscortService(IOrchardServices orchardServices, IPSSEscortDetailsManager<PSSEscortDetails> pssEscortDetailsManager, IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> policeOfficerDeploymentLogManager, IPSSDispatchNoteManager<PSSDispatchNote> pssDispatchNoteManager)
        {
            _orchardServices = orchardServices;
            _pssEscortDetailsManager = pssEscortDetailsManager;
            _policeOfficerDeploymentLogManager = policeOfficerDeploymentLogManager;
            _pssDispatchNoteManager = pssDispatchNoteManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Creates dispatch note
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateDispatchNote(string fileNumber, bool returnByte = false)
        {
            try
            {
                DispatchNoteVM dispatchNote = _pssDispatchNoteManager.GetDispatchNote(fileNumber);
                if (dispatchNote == null) { throw new NoRecordFoundException($"404 for PSS Dispatch Note with File Ref Number {fileNumber}"); }

                Template template = JsonConvert.DeserializeObject<CertificateTemplateVM>(dispatchNote.Template).Template;

                string fileName = fileNumber + ".pdf";

                dispatchNote.LogoURL = HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.PoliceLogo.ToDescription());
                dispatchNote.PossapLogoUrl = HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.PossapLogo.ToDescription());
                dispatchNote.StripURL = HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.CertificateStrip.ToDescription()).Replace("C:\\", "\\");
                dispatchNote.BGPath = HttpContext.Current.Server.MapPath(_orchardServices.WorkContext.CurrentTheme.VirtualPath + TenantConfigKeys.ThemeImages.ToDescription() + PSSTenantConfigKeys.DispatchNoteBG.ToDescription()).Replace("C:\\", "\\").Replace("\\", "\\\\");
                string vSavingPath = template.SavingPath + "/" + _orchardServices.WorkContext.CurrentSite.SiteName + "/";
                string fileFullPath = HttpContext.Current.Server.MapPath(vSavingPath + fileName);

                CBS.Core.StateConfig.StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                CBS.Core.StateConfig.Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.ValidateDocumentURL.ToString())?.FirstOrDefault();
                if (node == null) { throw new Exception("Unable to fetch Validate Document URL from StateConfig in Core Escort Service"); }
                dispatchNote.ValidateDocumentUrl = $"{node.Value}";

                if (!returnByte)
                {
                    //Check if the file already exist in the folder
                    if (File.Exists(fileFullPath))
                    {
                        return new CreateCertificateDocumentVM { SavedPath = fileFullPath, FileName = fileName };
                    }
                }

                var savedFileDir = HttpContext.Current.Server.MapPath(vSavingPath);
                Directory.CreateDirectory(savedFileDir);
                byte[] certificateByte = null;
                if (returnByte)
                {
                    certificateByte = _exportToPDF.Value.DownloadAsPdfNRecoLibNoBorders(null, template.File, dispatchNote, template.BasePath);
                }
                else
                {
                    _exportToPDF.Value.SaveAsPdfNRecoLibNoBorders(null, template.File, dispatchNote, fileName, vSavingPath, template.BasePath);
                }

                return new CreateCertificateDocumentVM { SavedPath = fileFullPath, FileName = fileName, DocByte = certificateByte };
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Checks if there is an escort request with specified file ref number that has been approved
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public bool CheckIfApprovedEscortRequestExists(string fileRefNumber)
        {
            return _pssEscortDetailsManager.Count(x => x.Request.FileRefNumber == fileRefNumber && x.Request.Status == (int)PSSRequestStatus.Approved) > 0;
        }


        /// <summary>
        /// Checks if there is a escort request with specified file ref number that has been approved for the currently logged in user
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public bool CheckIfApprovedEscortRequestExists(string fileRefNumber, long taxEntityId)
        {
            return _pssEscortDetailsManager.Count(x => x.Request.FileRefNumber == fileRefNumber && x.Request.TaxEntity == new CBS.Core.Models.TaxEntity { Id = taxEntityId } && x.Request.Status == (int)PSSRequestStatus.Approved) > 0;
        }
    }
}