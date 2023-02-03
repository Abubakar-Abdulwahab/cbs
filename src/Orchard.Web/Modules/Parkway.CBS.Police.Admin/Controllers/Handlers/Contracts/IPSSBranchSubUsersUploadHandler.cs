using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSBranchSubUsersUploadHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);

        /// <summary>
        /// Get PSSBranchDetailsVM
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSBranchDetailsVM GetPSSBranchDetailsVM(string payerId, TaxEntityProfileLocationReportSearchParams searchParams);

        /// <summary>
        /// Validates branch sub users file size and type
        /// </summary>
        /// <param name="branchSubUserFile"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        bool ValidateBranchSubUserFile(HttpPostedFileBase branchSubUserFile, ref List<ErrorModel> errors);

        /// <summary>
        /// Process branch sub users file, saving and returning the batch token
        /// </summary>
        /// <param name="branchSubUserFile"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        string ProcessBranchSubUsersFileUpload(HttpPostedFileBase branchSubUserFile, string payerId);

        /// <summary>
        /// Gets branch sub users file upload validation result
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSBranchSubUsersUploadValidationResultVM GetUploadValidationResult(string batchToken, PSSBranchSubUsersUploadBatchItemsSearchParams searchParams);

        /// <summary>
        /// Checks if branch sub users file batch with id embedded in specified batch token has been successfully uploaded and validated
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        APIResponse CheckIfBatchUploadValidationCompleted(string batchToken);

        /// <summary>
        /// Creates branch sub users
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>payer id of tax entity attached to the batch</returns>
        string SaveBranchSubUsers(string batchToken);
    }
}
