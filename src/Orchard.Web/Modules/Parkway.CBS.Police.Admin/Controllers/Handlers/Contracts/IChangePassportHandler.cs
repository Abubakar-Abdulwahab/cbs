using System.Collections.Generic;
using System.Web;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IChangePassportHandler : IDependency
    {
        APIResponse GetFileNumberDetails(string fileNumber);


        void ValidatePassportPhoto(HttpPostedFileBase httpPostedFileBase, ref List<ErrorModel> errors);


        string SavePassportPhoto(HttpPostedFileBase httpPostedFileBase, ref List<ErrorModel> errors);

        /// <summary>
        /// Change photo
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="certDeets"></param>
        bool ChangePassportPhoto(string filePathName, CharacterCertificateDocumentVM certDeets);
    }
}
