using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IChangePCCBioDataHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);


        /// <summary>
        /// Processes character certificate international passport bio data page update
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="bioDataPostedFile"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        bool ProcessBioDataUpdate(string fileNumber, HttpPostedFileBase bioDataPostedFile, ref List<ErrorModel> errors);
    }
}
