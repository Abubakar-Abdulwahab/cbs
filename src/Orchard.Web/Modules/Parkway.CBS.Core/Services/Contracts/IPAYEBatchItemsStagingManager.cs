using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEBatchItemsStagingManager<PAYEBatchItemsStaging> : IDependency, IBaseManager<PAYEBatchItemsStaging>
    {
        /// <summary>
        /// Save PAYE batch items in staging table
        /// <para>This uses ADO.Net for faster insertion for files with large amounts of data</para>
        /// </summary>
        /// <param name="payeItems"></param>
        /// <param name="batchStagingId"></param>
        void SavePAYELineItemsRecords(List<PayeeAssessmentLineRecordModel> payeItems, long batchStagingId);

        /// <summary>
        /// Save PAYE batch items in staging table
        /// <para>This uses ADO.Net for faster insertion for files with large amounts of data</para>
        /// </summary>
        /// <param name="payeItems"></param>
        /// <param name="batchStagingId"></param>
        void SavePAYEAssessmentLineItems(List<PAYEAssessmentLine> payeItems, long batchStagingId);

        /// <summary>
        /// Save PAYE batch items in staging table
        /// </summary>
        /// <param name="payees"></param>
        /// <param name="batchStagingId"></param>
        void SaveRecords(List<PayeeAssessmentLineRecordModel> payees, long batchStagingId);

        /// <summary>
        /// populate the batch items staging table items that belong to the batch Id
        /// with the tax entity that correspond to the payer Id on both tables
        /// </summary>
        /// <param name="id"></param>
        void PopulateTaxEntityId(long id);

        /// <summary>
        /// Returns Only the PAYEBatchItemsStaging Id
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>The Id of the PAYEBatchItemsStaging</returns>
        /// <exception cref="NoRecordFoundException"> Thrown when no record is found matching the query</exception>
        long GetPAYEBatchItemsStagingId(Expression<Func<PAYEBatchItemsStaging, bool>> lambda);

        /// <summary>
        /// Checks if the payerId and payeBatchRecordStagingId already exist
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="payeBatchRecordStagingId"></param>
        /// <returns>Boolean (True or False)</returns>
        bool BatchItemStagingExist(string payerId, long payeBatchRecordStagingId);

        /// <summary>
        /// When the batch items have been updated with the tax entity Id,
        /// we need to update the items that don't have a tax entity Id
        /// that means a corresponding value was not found for the payer id
        /// Here we need to set the has errors to true of false
        /// </summary>
        /// <param name="batchId"></param>
        void SetHasErrorsForNullTaxEntity(long batchId);

        /// <summary>
        /// Get the list of batch items
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>IEnumerable{PayeeReturnModelVM}</returns>
        IEnumerable<PayeeReturnModelVM> GetListOfPayes(long batchId, long taxEntityId, int skip, int take);

        /// <summary>
        /// Get report aggregate
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        IEnumerable<FileUploadReport> GetReportAggregate(long batchId, long taxEntityId);

        /// <summary>
        /// Get count of items
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{int}</returns>
        IEnumerable<int> GetCount(long batchId);
    }
}