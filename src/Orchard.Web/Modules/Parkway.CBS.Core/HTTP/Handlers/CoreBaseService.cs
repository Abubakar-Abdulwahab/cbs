using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaLibrary.Models;
using Orchard.MediaLibrary.Services;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public abstract class CoreBaseService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IMimeTypeProvider _mimeTypeProvider;

        public ILogger Logger { get; set; }

        protected CoreBaseService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _mediaLibraryService = mediaManagerService;
            _mimeTypeProvider = mimeTypeProvider;
            Logger = NullLogger.Instance;
        }       


        public void CheckFileSize(List<UploadedFileAndName> files, List<ErrorModel> validationErrors, int sizeInKb = 0)
        {
            if (validationErrors == null) { validationErrors = new List<ErrorModel>(); }
            //get allowed file size
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            HttpRuntimeSection section = config.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
            var maxFileSizeInBytes = (sizeInKb == 0) ? section.MaxRequestLength * 1000 : sizeInKb * 1000;
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Upload.ContentLength > maxFileSizeInBytes || files[i].Upload.ContentLength <= 0)
                {
                    validationErrors.Add(new ErrorModel { FieldName = files[i].UploadName, ErrorMessage = Lang.ErrorLang.filesizeexceeded().ToString() });
                }
            }
        }

        /// <summary>
        /// Check file type for images
        /// </summary>
        /// <param name="uploadedFiles"></param>
        /// <param name="validationErrors"></param>
        public void CheckFileType(List<UploadedFileAndName> uploadedFiles, List<ErrorModel> validationErrors, List<string> defineMimes = null, List<string> defineExtensions = null)
        {
            if(validationErrors == null) { validationErrors = new List<ErrorModel>(); }
            List<string> mimes = new List<string>() { "jpg", "png", "gif", "jpeg" };
            List<string> extensions = new List<string>() { ".jpg", ".png", ".gif", ".jpeg" };

            if (defineMimes != null) { mimes = defineMimes; }
            if( validationErrors != null) { extensions = defineExtensions; }           

            for (int i = 0; i < uploadedFiles.Count; i++)
            {
                if (uploadedFiles[i].Upload.ContentType == null) { continue; }
                if (!mimes.Any(type => uploadedFiles[i].Upload.ContentType.Contains(type)) && !extensions.Any(extension => uploadedFiles[i].UploadName.EndsWith(extension)))
                {
                    validationErrors.Add( new ErrorModel { FieldName = uploadedFiles[i].UploadName, ErrorMessage = Lang.ErrorLang.filetypenotallowed(string.Join(",", extensions.ToArray())).ToString() });
                }
            }
        }

        public void CreateFolders(string folderPath, string[] folderNames)
        {
            if (!_mediaLibraryService.CanManageMediaFolder(folderPath)) { throw new UserNotAuthorizedForThisActionException(); }
            try
            {
                for (int i = 0; i < folderNames.Count(); i++)
                {
                    _mediaLibraryService.CreateFolder(folderPath, folderNames[i]);
                }
            }
            catch (ArgumentException exception) { Logger.Error("Error creating folder: Folder has already been created", exception); }
            catch (Exception exception) { Logger.Error("Error creating folder", exception); throw exception; }
        }

        public MediaPart SaveMedia(string folderPath, HttpPostedFileBase file, bool createThumbnail = false)
        {
            string fileName = Guid.NewGuid().ToString("N");
            var mediaPart = _mediaLibraryService.ImportMedia(file.InputStream, folderPath, fileName + ".png", null);
            if (createThumbnail)
            {
                CreateFolders(folderPath, new string[] { "Thumbnail" });
                WebImage img = new WebImage(file.InputStream);
                Simple.ImageResizer.ImageResizer resizer = new Simple.ImageResizer.ImageResizer(img.GetBytes());
                var resized = resizer.Resize(200, 200, Simple.ImageResizer.ImageEncoding.Png);
                Stream stream = new MemoryStream(resized);
                _mediaLibraryService.ImportMedia(stream, folderPath + "\\Thumbnail", fileName + "_thumbnail.png");
            }
            return mediaPart;
        }

        /// <summary>
        /// Get base64 hash string value 
        /// </summary>
        /// <param name="soup">Value</param>
        /// <param name="salt">Salt</param>
        /// <returns>string</returns>
        protected string OnewayHashThis(string soup, string salt)
        {
            byte[] preHash = System.Text.Encoding.UTF32.GetBytes(soup + salt);
            byte[] hash = null;
            using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
            {
                hash = sha.ComputeHash(preHash);
            }
            return Convert.ToBase64String(hash);
        }
    }
}