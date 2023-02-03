using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Orchard.Logging;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using Newtonsoft.Json;
using System.Web;
using System.Collections.Generic;
using Parkway.CBS.Core.StateConfig;
using System.Linq;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Core.Exceptions;
using Orchard;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System.IO;
using Parkway.CBS.Police.Core.CoreServices.Contracts;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class ChangePassportHandler : IChangePassportHandler
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> _repo;
        private readonly ICoreHelperService _corehelper;
        private readonly ICoreCharacterCertificateService _coreCharacterCertificateService;


        ILogger Logger { get; set; }
        //private readonly IOrchardServices _orchardServices;
        //private readonly ITypeImplComposer _typeImpl;

        public ChangePassportHandler(IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails> repo, IOrchardServices orchardServices, ICoreHelperService corehelper, ICoreCharacterCertificateService coreCharacterCertificateService)
        {
            _repo = repo;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _corehelper = corehelper;
            _coreCharacterCertificateService = coreCharacterCertificateService;
        }


        public APIResponse GetFileNumberDetails(string fileNumber)
        {
            try
            {
                CharacterCertificateDocumentVM result = _repo.GetPendingCharacterCertificateDocumentDetailsWithoutPassport(fileNumber);
                if (result == null) { return new APIResponse { Error = true, ResponseObject = "No record found" }; }
                return new APIResponse { ResponseObject = result };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Change photo exception");
                return new APIResponse { Error = true, ResponseObject = "Exception No record found" };
            }
        }


        /// <summary>
        /// Validates passport photo file file size and type
        /// </summary>
        /// <param name="photoPostedFile"></param>
        /// <param name="errors"></param>
        public void ValidatePassportPhoto(HttpPostedFileBase photoPostedFile, ref List<ErrorModel> errors)
        {
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>(1);
            filesAndFileNames.Add(new UploadedFileAndName { Upload = photoPostedFile, UploadName = "PassportPhotographFile" });
            _corehelper.CheckFileSize(filesAndFileNames, errors, 2048);
            _corehelper.CheckFileType(filesAndFileNames, errors, new List<string> { "jpg", "png", "jpeg" }, new List<string> { ".jpg", ".png", ".jpeg" });
        }


        public string SavePassportPhoto(HttpPostedFileBase photoPostedFile, ref List<ErrorModel> errors)
        {
            string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.CharacterCertificateFilePath.ToString()).FirstOrDefault();

            DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value + siteName);
            string path = Path.Combine(basePath.FullName, Guid.NewGuid().ToString("N") + Util.StrongRandomNoSpecailCharacters() + DateTime.Now.Ticks.ToString() + Path.GetExtension(photoPostedFile.FileName));
            //save file
            photoPostedFile.SaveAs(path);
            return path;
        }


        /// <summary>
        /// Change photo
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="certDeets"></param>
        public bool ChangePassportPhoto(string filePathName, CharacterCertificateDocumentVM certDeets)
        {
            return _coreCharacterCertificateService.ChangePassportPhoto(filePathName, certDeets);
        }

    }
}