using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using System.Collections.Generic;
using System.Data;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSSBranchOfficersBatchItemsDAOManager : IRepository<PSSBranchOfficersUploadBatchItemsStaging>
    {
        /// <summary>
        /// Save PSSBranchOfficers line items as a bundle
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="batchId"></param>
        void SavePSSBranchOfficersLineItemsRecords(List<PSSBranchOfficersItemVM> lineItems, long batchId);

        /// <summary>
        /// Get records from PSSBranchOfficers in batch with specified id and having no validation errors
        /// </summary>
        /// <param name="batchId"></param>
        IEnumerable<PSSBranchOfficersItemVM> GetValidBranchOfficersByBatchId(long batchId);

        /// <summary>
        /// Perform update on PSSBranchSubUsersItem in batch with APNumber specified PersonnelReportRecord
        /// </summary>
        /// <param name="officerDetails"></param>
        /// <param name="itemId"></param>
        void UpdatePSSBranchOfficersDetails(PersonnelReportRecord officerDetails, long itemId);

        /// <summary>
        /// Update error status for PSSBranchOfficers with specified batch id using the specified error message.
        /// </summary>
        /// <param name="batchOfficerId"></param>
        /// <param name="errorMessage"></param>
        void UpdatePSSBranchOfficersErrorStatus(string errorMessage, long batchOfficerId);

        /// <summary>
        /// Perform validation on PSSBranchOfficerItems APNumber column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateAPNumberIsNotDuplicateAndUpdatePSSBranchOfficersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform bulk validation on PSSBranchOfficerItems APNumber column that an officer with same APNumber is not on Active deployment, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateAPNumberIsNotOnActiveDeploymentndUpdatePSSBranchOfficersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform bulk validation on PSSBranchOfficerItems BRANCH CODE column to make sure it is same as current batch branch, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateBranchCodeIsSameAsCurrentBranchAndUpdatePSSBranchOfficersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Build PSSBranchOfficersUploadBatchItemsStaging Temp Table for Bulk Update
        /// </summary>
        /// <param name="PSSBranchOfficersUploadBatchItems"></param>
        /// <param name="batchId"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempTableQuery"></param>
        /// <param name="updateTableQuery"></param>
        /// <param name="dataTable"></param>
        void BuildPSSBranchOfficersUploadBatchItemsStagingBulkUpdate(IEnumerable<APNumberValidationVM> PSSBranchOfficersUploadBatchItems, long batchId, out string tempTableName, out string createTempTableQuery, out string updateTableQuery, out DataTable dataTable);


        /// <summary>
        /// Performs a bulk update using ADO.NET on <see cref="PSSBranchOfficersUploadBatchItemsStaging"/> which requires creating a temporary table <paramref name="tempTableName"/>
        /// </summary>
        /// <param name="stagingBatchItemsDataTable"></param>
        /// <param name="tempTableName"></param>
        /// <param name="createTempQuery"></param>
        /// <exception cref="Exception">When an error occures</exception>
        void UpdateRecordsAfterValidation(DataTable stagingBatchItemsDataTable, string tempTableName, string createTempQuery, string updateTableQuery);
    }
}
