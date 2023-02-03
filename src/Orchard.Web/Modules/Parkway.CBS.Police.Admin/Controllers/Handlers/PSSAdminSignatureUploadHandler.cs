using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSAdminSignatureUploadHandler : IPSSAdminSignatureUploadHandler
    {
        ILogger Logger { get; set; }
        private readonly IHandlerComposition _handlerComposition;
        private readonly ICoreHelperService _corehelper;
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSAdminSignatureUploadManager<PSSAdminSignatureUpload> _pssAdminSignatureUploadManager;

        public PSSAdminSignatureUploadHandler(IHandlerComposition handlerComposition, ICoreHelperService corehelper, IOrchardServices orchardServices, IPSSAdminSignatureUploadManager<PSSAdminSignatureUpload> pssAdminSignatureUploadManager)
        {
            _handlerComposition = handlerComposition;
            _corehelper = corehelper;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _pssAdminSignatureUploadManager = pssAdminSignatureUploadManager;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canUploadSignature"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission canUploadSignature)
        {
            _handlerComposition.IsAuthorized(canUploadSignature);
        }


        /// <summary>
        /// Saves uploaded signature file
        /// </summary>
        /// <param name="signatureFile"></param>
        /// <param name="errors"></param>
        public void SaveSignature(HttpPostedFileBase signatureFile, ref List<ErrorModel> errors)
        {
            try
            {
                int userId = _orchardServices.WorkContext.CurrentUser.Id;
                string fileName = string.Empty;
                string path = string.Empty;
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                fileName = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString() + userId + Path.GetExtension(signatureFile.FileName);
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
                Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PSSAdminSignaturesFilePath.ToString()).FirstOrDefault();
                if (node == null || string.IsNullOrEmpty(node.Value))
                {
                    Logger.Error(string.Format("Unable to get pss admin signatures file upload path in config file"));
                    errors.Add(new ErrorModel { ErrorMessage = "Error uploading signature file.", FieldName = "signatureFile" });
                    throw new DirtyFormDataException();
                }

                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value + siteName);
                path = Path.Combine(basePath.FullName, fileName);
                //save file
                signatureFile.SaveAs(path);

                PSSAdminSignatureUpload signatureUpload = new PSSAdminSignatureUpload
                {
                    SignatureBlob = Convert.ToBase64String(File.ReadAllBytes(path)),
                    SignatureContentType = signatureFile.ContentType,
                    SignatureFileName = signatureFile.FileName,
                    SignatureFilePath = path,
                    AddedBy = new Orchard.Users.Models.UserPartRecord { Id = userId },
                    IsActive = true
                };

                _pssAdminSignatureUploadManager.DeactivateExisitingSignaturesForAdmin(userId);

                if (!_pssAdminSignatureUploadManager.Save(signatureUpload))
                {
                    errors.Add(new ErrorModel { FieldName = "signatureFile", ErrorMessage = "Unable to save signature file. Please try again later" });
                    throw new DirtyFormDataException();
                }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _pssAdminSignatureUploadManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Validates signature file size and type
        /// </summary>
        /// <param name="signatureFile"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool ValidateSignatureFile(HttpPostedFileBase signatureFile, ref List<ErrorModel> errors)
        {
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PSSAdminSignaturesFileSize.ToString()).FirstOrDefault();
            int fileSizeCap = 0;
            if (node == null || string.IsNullOrEmpty(node.Value) || !int.TryParse(node.Value, out fileSizeCap))
            {
                Logger.Error(string.Format("Unable to get pss admin signatures file size in config file"));
                errors.Add(new ErrorModel { ErrorMessage = "Error uploading signature file.", FieldName = "signatureFile" });
                throw new DirtyFormDataException();
            }
            { filesAndFileNames.Add(new UploadedFileAndName { Upload = signatureFile, UploadName = "signatureFile" }); }
            _corehelper.CheckFileSize(filesAndFileNames, errors, fileSizeCap);
            _corehelper.CheckFileType(filesAndFileNames, errors, new List<string> { "jpg", "png", "jpeg" }, new List<string> { ".jpg", ".png", ".jpeg" });

            return errors.Any();
        }


        /// <summary>
        /// Gets PSSAdminSignaturesListVM initialized with all the signatures uploaded by the current admin
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public PSSAdminSignaturesListVM GetSignaturesListVM(int userId, int take, int skip, DateTime start, DateTime end)
        {
            try
            {
                return new PSSAdminSignaturesListVM
                {
                    Signatures = _pssAdminSignatureUploadManager.GetAdminSignatures(userId, take, skip, start, end),
                    TotalSignaturesUploaded = _pssAdminSignatureUploadManager.GetNumberOfUploadedSignatures(userId, start, end),
                    Start = start.ToString("dd/MM/yyyy"),
                    End = end.ToString("dd/MM/yyyy")
                };

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Sets the IsActive state of admin signature with specified id to true
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="adminSignatureId"></param>
        /// <param name="errorMessage"></param>
        public void EnableAdminSignature(int userId, int adminSignatureId, ref string errorMessage)
        {
            try
            {
                if (_pssAdminSignatureUploadManager.Count(x => (x.Id == adminSignatureId) && (x.AddedBy.Id == userId)) == 0) 
                {
                    errorMessage = "signature does not exist";
                    throw new DirtyFormDataException();
                }

                _pssAdminSignatureUploadManager.DeactivateExisitingSignaturesForAdmin(userId);
                _pssAdminSignatureUploadManager.UpdateSignatureStatus(userId, adminSignatureId, true);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _pssAdminSignatureUploadManager.RollBackAllTransactions();
                throw;
            }
        }
    }
}