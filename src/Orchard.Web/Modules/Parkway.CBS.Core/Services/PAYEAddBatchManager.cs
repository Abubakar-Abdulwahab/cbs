using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class PAYEAddBatchManager : IPAYEAddBatchManager
    {
        private readonly IPAYEBatchItemsPagesTrackerManager<PAYEAPIBatchItemsPagesTracker> _PAYEBatchItemsPagesTrackerManager;
        private readonly IPAYEBatchItemsRefManager<PAYEAPIBatchItemsRef> _PAYEBatchItemsRefManager;
        private readonly IPAYEBatchItemsStagingManager<PAYEBatchItemsStaging> _PAYEBatchItemsStagingManager;
        private readonly ITransactionManager _transactionManager;

        public PAYEAddBatchManager(IPAYEBatchItemsStagingManager<PAYEBatchItemsStaging> payeBatchItemsStagingManager,
                             IPAYEBatchItemsPagesTrackerManager<PAYEAPIBatchItemsPagesTracker> payeBatchItemsPagesTrackerManager,
                              IPAYEBatchItemsRefManager<PAYEAPIBatchItemsRef> payeAPIBatchItemsRefManager, IOrchardServices orchardServices)
        {
            _PAYEBatchItemsRefManager = payeAPIBatchItemsRefManager;
            _PAYEBatchItemsPagesTrackerManager = payeBatchItemsPagesTrackerManager;
            _PAYEBatchItemsStagingManager = payeBatchItemsStagingManager;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Checks if the batchIdentifier and the page number already exist
        /// </summary>
        /// <param name="batchIdentifier"></param>
        /// <param name="pageNumber"></param>
        /// <returns>Boolean (True or False)</returns>
        public bool BatchItemPageExist(string batchIdentifier, int pageNumber)
        {
            return _PAYEBatchItemsPagesTrackerManager.BatchItemPageExist(batchIdentifier, pageNumber);
        }

        /// <summary>
        /// Saves PAYEAPIBatchItemsPagesTracker to database
        /// </summary>
        /// <param name="payeAPIBatchItemsPagesTracker"></param>
        /// <returns>Boolean</returns>
        public bool SaveBatchItemsPagesTracker(PAYEAPIBatchItemsPagesTracker payeAPIBatchItemsPagesTracker)
        {
            return _PAYEBatchItemsPagesTrackerManager.Save(payeAPIBatchItemsPagesTracker);
        }

        /// <summary>
        /// Generates a PAYEBatchItemsStaging object
        /// </summary>
        /// <param name="batchDetailsVM"></param>
        /// <param name="payeItem"></param>
        /// <returns>PAYEBatchItemsStaging</returns>
        public PAYEBatchItemsStaging PopulatePAYEBatchItems(PAYEAPIRequestBatchDetailVM batchDetailsVM, PAYEBatchItemsModel payeItem)
        {
            return new PAYEBatchItemsStaging
            {
                Exemptions = payeItem.Exemptions.ToString(),
                GrossAnnual = payeItem.GrossAnnual.ToString(),
                IncomeTaxPerMonth = payeItem.IncomeTaxPerMonth.ToString(),
                Year = payeItem.Year.ToString(),
                Month = payeItem.Month,
                PayerId = payeItem.PayerId,
                TaxEntity = new TaxEntity { Id = batchDetailsVM.TaxEntityId },
                YearValue = payeItem.Year,
                IncomeTaxPerMonthValue = payeItem.IncomeTaxPerMonth,
                PAYEBatchRecordStaging = new PAYEBatchRecordStaging { Id = batchDetailsVM.PAYEBatchRecordStagingId },
            };
        }

        /// <summary>
        /// Rolls back all transaction done on PAYEAPIBatchItemsPagesTracker, PAYEBatchItemsStaging
        /// and PAYEAPIBatchItemsRef
        /// </summary>
        public void RollBackAllTransaction()
        {
            _PAYEBatchItemsRefManager.RollBackAllTransactions();
        }

        /// <summary>
        /// Saves to  PAYEBatchItemsStaging and PAYEAPIBatchItemsRef Tables
        /// </summary>
        /// <param name="model"></param>
        /// <param name="batchDetailsVM"></param>
        /// <param name="payeItem"></param>
        /// <param name="saveBatchItem"></param>
        /// <param name="saveBatchItemResult"></param>
        /// <param name="saveBatchItemRefResult"></param>
        public void TrySavePAYEBatchItems(PAYEAPIBatchItemsPagesTracker payeAPIBatchItemsPagesTracker, PAYEAPIRequestBatchDetailVM batchDetailsVM, PAYEBatchItemsModel payeItem, PAYEBatchItemsStaging saveBatchItem, out bool saveBatchItemResult, out bool saveBatchItemRefResult)
        {
            saveBatchItemResult = _PAYEBatchItemsStagingManager.Save(saveBatchItem);

            saveBatchItemRefResult = _PAYEBatchItemsRefManager.Save(new PAYEAPIBatchItemsRef
            {
                ItemNumber = payeItem.ItemNumber,
                PAYEAPIRequest = new PAYEAPIRequest { Id = batchDetailsVM.PAYEAPIRequestId },
                Mac = payeItem.Mac,
                PAYEAPIBatchItemsPagesTracker = new PAYEAPIBatchItemsPagesTracker { Id = payeAPIBatchItemsPagesTracker.Id },
                PAYEBatchItemsStaging = new PAYEBatchItemsStaging { Id = saveBatchItem.Id }
            });
        }
    }
}