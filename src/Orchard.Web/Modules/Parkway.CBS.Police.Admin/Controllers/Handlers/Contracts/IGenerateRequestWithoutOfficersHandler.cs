using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IGenerateRequestWithoutOfficersHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);


        /// <summary>
        /// Gets GenerateRequestForDefaultBranchWithoutOfficersUploadVM
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        GenerateRequestForDefaultBranchWithoutOfficersUploadVM GetGenerateRequestForDefaultBranchWithoutOfficersUploadVM(string payerId, GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams);


        /// <summary>
        /// Gets GenerateRequestForBranchWithoutOfficersUploadVM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        GenerateRequestForBranchWithoutOfficersUploadVM GetGenerateRequestForBranchWithoutOfficersUploadVM(GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams);


        /// <summary>
        /// Validates file size and type
        /// </summary>
        /// <param name="generateRequestWithoutOfficersUploadFile"></param>
        /// <param name="errors"></param>
        /// <returns>returns true if there are errors</returns>
        bool ValidateGenerateRequestWithoutOfficersUploadFile(HttpPostedFileBase generateRequestWithoutOfficersUploadFile, ref List<ErrorModel> errors);


        /// <summary>
        /// Process generate request without officers upload file, saving and returning the batch token
        /// </summary>
        /// <param name="generateRequestWithoutOfficersUploadFile"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        string ProcessGenerateRequestWithoutOfficersUploadFileForEntity(HttpPostedFileBase generateRequestWithoutOfficersUploadFile, string payerId);


        /// <summary>
        /// Process generate request without officers upload file, saving and returning the batch token
        /// </summary>
        /// <param name="generateRequestWithoutOfficersUploadFile"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        string ProcessGenerateRequestWithoutOfficersUploadFileForBranch(HttpPostedFileBase generateRequestWithoutOfficersUploadFile, int branchId);


        /// <summary>
        /// Gets GenerateRequestWithoutOfficersFileUploadValidationResultVM
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        GenerateRequestWithoutOfficersFileUploadValidationResultVM GetUploadValidationResult(string batchToken, GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams);

        /// <summary>
        /// Checks if generate request without officers upload file batch with id embedded in specified batch token has been successfully uploaded and validated
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        APIResponse CheckIfBatchUploadValidationCompleted(string batchToken);

        /// <summary>
        /// Get GenerateRequestDetailVM for the view Generate Request Details
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        GenerateRequestWithoutOfficersDetail GetGenerateRequestDetailVM(GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams, bool isDefault = false);
    }
}
