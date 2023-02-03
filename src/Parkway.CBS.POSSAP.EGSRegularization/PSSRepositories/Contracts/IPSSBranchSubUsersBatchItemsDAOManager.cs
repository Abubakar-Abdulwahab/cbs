using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSSBranchSubUsersBatchItemsDAOManager : IRepository<PSSBranchSubUsersUploadBatchItemsStaging>
    {
        /// <summary>
        /// Save PSSBranchSubUsers line items as a bundle
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="batchId"></param>
        void SavePSSBranchSubUsersLineItemsRecords(List<PSSBranchSubUsersItemVM> lineItems, long batchId);

        /// <summary>
        /// Resolves Branch LGA Ids and Branch LGA values for PSSBranchSubUsers items that have been created in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        void ResolveBranchLGAAndStateIdsAndValuesForCreatedBranches(long batchId);

        /// <summary>
        /// Perform bulk update on PSSBranchSubUsersItems, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void UpdateBranchLGAAndStateResolutionErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform bulk validation and update on PSSBranchSubUsersItems SubUserEmail column, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateSubUserEmailIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems SubUserEmail column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateSubUserEmailIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems phoneNumber column that a user with same number doesn't exist on CBSUser, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidatePhoneNumberIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems phoneNumber column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidatePhoneNumberIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems branch address column that a tex entity with same address doesn't exist on TaxEntityProfileLocation, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateAddressIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems branch Address column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateAddressIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems branch name column that a tex entity with same name doesn't exist on TaxEntityProfileLocation, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateBranchNameIsNotExisitingAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform validation on PSSBranchSubUsersItems branch name column that duplicates are not uploaded, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateBranchNameIsNotDuplicateAndUpdatePSSBranchSubUsersItemErrorMessage(long batchId, string errorMessage);
    }
}
