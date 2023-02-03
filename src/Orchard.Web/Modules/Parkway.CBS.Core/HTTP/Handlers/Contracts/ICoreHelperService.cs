using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreHelperService : IDependency
    {
        void CheckFileSize(List<UploadedFileAndName> files, List<ErrorModel> validationErrors, int sizeInKb = 0);

        void CheckFileType(List<UploadedFileAndName> uploadedFiles, List<ErrorModel> validationErrors, List<string> defineMimes = null, List<string> defineExtensions = null);
    }
  
}
