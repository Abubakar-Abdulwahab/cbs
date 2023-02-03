using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSAdminSignatureUploadManager<PSSAdminSignatureUpload> : IDependency, IBaseManager<PSSAdminSignatureUpload>
    {
        /// <summary>
        /// Gets all admin signatures
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEnumerable<PSSAdminSignatureUploadVM> GetAdminSignatures(int userId, int take, int skip, DateTime start, DateTime end);

        /// <summary>
        /// Deactivates all the uploaded signatures for the admin with the specified user id
        /// </summary>
        /// <param name="userId"></param>
        void DeactivateExisitingSignaturesForAdmin(int userId);

        /// <summary>
        /// Gets total number of signatures uploaded by admin
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        int GetNumberOfUploadedSignatures(int userId, DateTime start, DateTime end);

        /// <summary>
        /// Updates the status of admin signature with the specified id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="adminSignatureId"></param>
        /// <param name="status"></param>
        void UpdateSignatureStatus(int userId, int adminSignatureId, bool status);

        /// <summary>
        /// Gets the active signature of admin with specified id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        PSSAdminSignatureUploadVM GetActiveAdminSignature(int userId);
    }
}
