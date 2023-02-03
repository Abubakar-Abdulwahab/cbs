using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.PAYEAPI.Contracts
{
    public interface IPAYEAPIBatchItemsRefDAOManager : IRepository<PAYEAPIBatchItemsRef>
    {
        /// <summary>
        /// Get paginated PAYE API batch items using the batchRecordStagingId
        /// </summary>
        /// <param name="payeBatchRecordStagingId"></param>
        /// <param name="chunkSize"></param>
        /// <param name="skip"></param>
        /// <returns>ICollection<PAYEAssessmentLine></returns>
        List<PAYEAPIBatchItemsRefVM> GetBatchItems(long payeBatchRecordStagingId, int chunkSize, int skip);

        /// <summary>
        /// Save the validation response in a temporary table
        /// </summary>
        /// <param name="payeBatchRecordStagingId"></param>
        /// <param name="payees"></param>
        /// <param name="batchLimit"></param>
        void SaveRecords(long payeBatchRecordStagingId, List<PAYEAPIBatchItemsRefVM> payees, int batchLimit);

        /// <summary>
        /// populate the batch items staging table items that belong to the batch Id 
        /// with the tax entity that correspond to the payer Id on both tables 
        /// </summary>
        /// <param name="id"></param>
        void PopulateTaxEntityId(long id);


        /// <summary>
        /// When the batch items have been updated with the tax entity Id, 
        /// we need to update the items that don't have a tax entity Id
        /// that means a corresponding value was not found for the payer id
        /// Here we need to set the has errors to true of false
        /// </summary>
        /// <param name="batchId"></param>
        void SetHasErrorsForNullTaxEntity(long batchId);

        /// <summary>
        /// Update the error message for items that has HasError flag as true i.e has an existing error message.
        /// This will just append the validation error to the existing error message
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateErrorMessageAfterItemValidation(long batchId);


        /// <summary>
        /// Update the error message for items that has HasError flag as false i.e has no existing error message.
        /// This will just set the validation error message
        /// </summary>
        /// <param name="batchId"></param>
        void SetHasErrorsAfterItemValidation(long batchId);


    }
}
