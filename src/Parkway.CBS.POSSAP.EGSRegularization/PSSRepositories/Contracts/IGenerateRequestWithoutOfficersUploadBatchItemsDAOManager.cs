using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IGenerateRequestWithoutOfficersUploadBatchItemsDAOManager : IRepository<GenerateRequestWithoutOfficersUploadBatchItemsStaging>
    {
        /// <summary>
        /// Save GenerateRequestWithoutOfficers line items as a bundle
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="batchId"></param>
        void SaveGenerateRequestWithoutOfficersLineItemsRecords(List<GenerateRequestWithoutOfficersUploadItemVM> lineItems, long batchId);

        /// <summary>
        /// Perform bulk validation on GenerateRequestWithoutOfficersItems BRANCH CODE column to make sure it is same as current batch branch, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
         void ValidateBranchCodeIsSameAsCurrentBranchAndUpdateGenerateRequestWithoutOfficersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Resolves Command Ids for GenerateRequestWithoutOfficers items that have been created in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        void ResolveCommandIdsForGenerateRequestsWithoutOfficersAlreadyCreated(long batchId);

        /// <summary>
        /// Perform bulk update on GenerateRequestWithoutOfficers, updating hasError and errorMessage for items in batch with specified id where commandId is null
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void UpdateCommandIdResolutionErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Perform validation on GenerateRequestWithoutOfficers items that duplicates of Command Code, Command Type and Day Type combined are not existing, updating hasError and errorMessage for items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="errorMessage"></param>
        void ValidateCommandCodeCommandTypeDayTypeCombinationIsNotDuplicateAndUpdateGenerateRequestWithoutOfficersItemErrorMessage(long batchId, string errorMessage);

        /// <summary>
        /// Gets items in batch with specified id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> GetItems(long batchId);
    }
}
