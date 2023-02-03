using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.Models.Enums;
using System.IO;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class ChangePCCBioDataHandler : IChangePCCBioDataHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IHandlerComposition _handlerComposition;
        private readonly ICoreHelperService _corehelper;
        private readonly IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> _repo;
        private readonly IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob> _characterCertificateDetailsBlobManager;
        ILogger Logger { get; set; }


        public ChangePCCBioDataHandler(IOrchardServices orchardServices, IHandlerComposition handlerComposition, ICoreHelperService corehelper, IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> repo, IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob> characterCertificateDetailsBlobManager)
        {
            _orchardServices = orchardServices;
            _handlerComposition = handlerComposition;
            _corehelper = corehelper;
            _repo = repo;
            _characterCertificateDetailsBlobManager = characterCertificateDetailsBlobManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }


        /// <summary>
        /// Processes character certificate international passport bio data page update
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="bioDataPostedFile"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool ProcessBioDataUpdate(string fileNumber, HttpPostedFileBase bioDataPostedFile, ref List<ErrorModel> errors)
        {
            try
            {
                Logger.Information($"Checking if there is a pending pcc request with file number {fileNumber}");
                if(_repo.Count(x => (x.Request.FileRefNumber == fileNumber) && (x.Request.Status == (int)PSSRequestStatus.PendingApproval)) == 0)
                {
                    Logger.Error($"No pcc request pending approval with file number {fileNumber}");
                    errors.Add(new ErrorModel { FieldName = nameof(VM.ChangeBioDataVM.FileNumber), ErrorMessage = $"No pcc request pending approval with file number {fileNumber}" });
                }

                Logger.Information($"Validating uploaded bio data page file size and type");
                List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>(1);
                filesAndFileNames.Add(new UploadedFileAndName { Upload = bioDataPostedFile, UploadName = "PassportBioDataPageFile" });
                _corehelper.CheckFileSize(filesAndFileNames, errors, 2048);
                _corehelper.CheckFileType(filesAndFileNames, errors, new List<string> { "jpg", "png", "jpeg", "pdf" }, new List<string> { ".jpg", ".png", ".jpeg", ".pdf" });

                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                Logger.Information($"Saving uploaded bio data page file");
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.CharacterCertificateFilePath)).SingleOrDefault();

                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value + siteName);
                string path = Path.Combine(basePath.FullName, Guid.NewGuid().ToString("N") + Util.StrongRandomNoSpecailCharacters() + DateTime.Now.Ticks.ToString() + Path.GetExtension(bioDataPostedFile.FileName));
                //save file
                bioDataPostedFile.SaveAs(path);

                return _characterCertificateDetailsBlobManager.UpdatePassportBioDataPage(path, _repo.GetCharacterCertificateDetailsIdWithFileNumber(fileNumber));
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}