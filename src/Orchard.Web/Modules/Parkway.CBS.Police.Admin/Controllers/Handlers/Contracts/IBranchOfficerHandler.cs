using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.VM;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IBranchOfficerHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canAddBranchOfficer"></param>
        void CheckForPermission(Permission canAddBranchOfficer);


        /// <summary>
        /// Handles the processing of the <paramref name="branchOfficerFile"/> which includes saving and batch creation
        /// </summary>
        /// <param name="branchOfficerFile"></param>
        /// <param name="payerId"></param>
        /// <returns>A serialized and encrpted <see cref="FileProcessModel"/></returns>
        string ProcessBranchOfficerUpload(HttpPostedFileBase branchOfficerFile, int payerId);


        /// <summary>
        /// Validates branch sub users file size and type
        /// </summary>
        /// <param name="branchOfficerFile"></param>
        /// <param name="errors"></param>
        /// <returns>returns true if there are errors</returns>
        bool ValidateBranchOfficerFile(HttpPostedFileBase branchOfficerFile, ref List<ErrorModel> errors);


        /// <summary>
        /// Gets category types for service category with specified id
        /// </summary>
        /// <param name="serviceCategoryId"></param>
        /// <returns></returns>
        APIResponse GetCategoryTypesForServiceCategoryWithId(string serviceCategoryId);


        /// <summary>
        /// Validates and Generate escort
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="userInputModel"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        InvoiceGenerationResponse GenerateEscortRequest(ref List<ErrorModel> errors, PSSBranchGenerateEscortRequestVM userInputModel);

        /// <summary>
        /// Get view model for branch officer report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalRecordCount }</returns>
        PSSBranchProfileDetailVM GetBranchProfileDetailVM(PSSBranchOfficersUploadBatchStagingReportSearchParams searchParams);

        EscortRequestDetailVM GetEscortRequestDetailVM(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams);

        FileProcessModel GetFileProcessModel(string batchToken);

        /// <summary>
        /// Gets the view model for the validated officers
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        OfficerValidationResultVM GetOfficerValidationResultVM(PSSBranchOfficersUploadBatchItemsStagingReportSearchParams searchParams);

        /// <summary>
        /// Checks if branch officer file batch with id embedded in specified batch token has been successfully uploaded and validated
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        APIResponse CheckIfBatchUploadValidationCompleted(string batchToken);

        /// <summary>
        /// Gets add officer vm
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        PSSBranchGenerateEscortRequestVM GetGenerateEscortRequestVM(long batchId);

        /// <summary>
        /// populates <paramref name="vm"/> for postback
        /// </summary>
        /// <param name="vm"></param>
        void PopulateGenerateEscortRequestVMForPostback(PSSBranchGenerateEscortRequestVM vm);

        /// <summary>
        /// Gets commands for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        APIResponse GetCommandsForCommandTypeWithId(string commandTypeId);

        /// <summary>
        /// Gets next level commands using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        APIResponse GetNextLevelCommandsWithCode(string code);


        /// <summary>
        /// Gets area and divisional commands using stateId and optionally specified lgaId
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        APIResponse GetAreaAndDivisionalCommandsByStateAndLGA(string stateId, string lgaId);
    }
}
