using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSAdminSignatureUploadHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canUploadSignature"></param>
        void CheckForPermission(Permission canUploadSignature);

        /// <summary>
        /// Saves uploaded signature file
        /// </summary>
        /// <param name="signatureFile"></param>
        /// <param name="errors"></param>
        void SaveSignature(HttpPostedFileBase signatureFile, ref List<ErrorModel> errors);

        /// <summary>
        /// Validates signature file size and type
        /// </summary>
        /// <param name="signatureFile"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        bool ValidateSignatureFile(HttpPostedFileBase signatureFile, ref List<ErrorModel> errors);

        /// <summary>
        /// Gets PSSAdminSignaturesListVM initialized with all the signatures uploaded by the current admin
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        PSSAdminSignaturesListVM GetSignaturesListVM(int userId, int take, int skip, DateTime start, DateTime end);

        /// <summary>
        /// Sets the IsActive state of admin signature with specified id to true
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="adminSignatureId"></param>
        /// <param name="errorMessage"></param>
        void EnableAdminSignature(int userId, int adminSignatureId, ref string errorMessage);
    }
}
