using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEAddBatchManager : IDependency
    {
        /// <summary>
        /// Checks if the batchIdentifier and the page number already exist
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <param name="pageNumber"></param>
        /// <returns>Boolean (True or False)</returns>
        bool BatchItemPageExist(string batchIdentifier, int pageNumber);

        /// <summary>
        /// Generates a PAYEBatchItemsStaging object
        /// </summary>
        /// <param name="batchDetailsVM"></param>
        /// <param name="payeItem"></param>
        /// <returns>PAYEBatchItemsStaging</returns>
        PAYEBatchItemsStaging PopulatePAYEBatchItems(PAYEAPIRequestBatchDetailVM batchDetailsVM, PAYEBatchItemsModel payeItem);

        /// <summary>
        /// Rolls back all transaction done on PAYEAPIBatchItemsPagesTracker, PAYEBatchItemsStaging
        /// and PAYEAPIBatchItemsRef
        /// </summary>
        void RollBackAllTransaction();

        /// <summary>
        /// Saves PAYEAPIBatchItemsPagesTracker to database
        /// </summary>
        /// <param name="payeAPIBatchItemsPagesTracker"></param>
        /// <returns>Boolean</returns>
        bool SaveBatchItemsPagesTracker(PAYEAPIBatchItemsPagesTracker payeAPIBatchItemsPagesTracker);

        /// <summary>
        /// Tries to save to PAYEAPIBatchItemsPagesTracker, PAYEBatchItemsStaging and PAYEAPIBatchItemsRef
        /// </summary>
        /// <param name="model"></param>
        /// <param name="batchDetailsVM"></param>
        /// <param name="payeItem"></param>
        /// <param name="saveBatchItem"></param>
        /// <param name="saveBatchItemResult"></param>
        /// <param name="saveBatchItemRefResult"></param>
        void TrySavePAYEBatchItems(PAYEAPIBatchItemsPagesTracker payeAPIBatchItemsPagesTracker, PAYEAPIRequestBatchDetailVM batchDetailsVM, PAYEBatchItemsModel payeItem, PAYEBatchItemsStaging saveBatchItem, out bool saveBatchItemResult, out bool saveBatchItemRefResult);
    }
}