using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICorePAYEService : IDependency
    {

        /// <summary>
        /// Process and save staging data for paye items
        /// </summary>
        /// <param name="savedBatchRecord"></param>
        /// <param name="directAssessmentPayeeLines"></param>
        /// <param name="adapter"></param>
        /// <param name="entity"></param>
        void ProcessPayeItemsForStagingInput(PAYEBatchRecordStaging savedBatchRecord, ICollection<PAYEAssessmentLine> directAssessmentPayeeLines, AssessmentInterface adapter);

        /// <summary>
        /// Get direct assessment adapter
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <returns>AssessmentInterface | null</returns>
        AssessmentInterface GetDirectAssessmentAdapter(string adapterValue);

        /// <summary>
        /// Process PAYE assessment 
        /// <para>Sort the records by tax entity Id and set the values to user not found if not payer Id </para>
        /// </summary>
        /// <param name="objValue"></param>
        void ProcessPAYEAssessment(FileProcessModel objValue);


        /// <summary>
        /// Check to see if the staging batch record has completed processing
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns>APIResponse</returns>
        APIResponse CheckForCompletionPercentage(FileProcessModel objValue);


        /// <summary>
        /// Get report details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        DirectAssessmentReportVM GetReportDetails(long id, long taxEntityId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<PayeeReturnModelVM> GetPagedDataForBatchItemsStaging(long batchId, long taxEntityId, int skip, int take);


        /// <summary>
        /// Get batch amount deets
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>BatchStats</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        decimal GetTotalAmountForValidEntriesInBatchStaging(long id, long taxEntityId);


        /// <summary>
        /// Process PAYE items from file
        /// </summary>
        /// <param name="savedBatchRecord"></param>
        /// <param name="adapter"></param>
        void ProcessPayeItemsForFileUploadStagingInput(PAYEBatchRecordStaging savedBatchRecord, AssessmentInterface adapter);

    }

}
