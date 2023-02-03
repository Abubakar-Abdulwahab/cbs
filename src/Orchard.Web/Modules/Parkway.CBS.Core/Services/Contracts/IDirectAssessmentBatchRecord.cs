using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee;
using System;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> : IDependency, IBaseManager<DirectAssessmentBatchRecord>
    {

        /// <summary>
        /// Get DirectAssessmentBatchRecord
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns>DirectAssessmentBatchRecord</returns>
        DirectAssessmentBatchRecord Get(long batchRecordId);
    }


    public interface IDirectAssessmentPayeeManager<DirectAssessmentBatchRecord> : IDependency, IBaseManager<DirectAssessmentBatchRecord>
    {
        void SaveRecords(List<PayeeAssessmentLineRecordModel> payees, Int64 batchRecordId, TaxEntity entity);

        void SaveRecords(List<PayeeAssessmentLineRecordModel> payees, Models.DirectAssessmentBatchRecord record, TaxEntity entity);

        /// <summary>
        /// Get paged data
        /// </summary>
        /// <param name="payeeRecord"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>List{DirectAssessmentPayeeRecord}</returns>
        IList<DirectAssessmentPayeeRecord> GetRecords(Int64 batchId, int take, int skip);


        void Delete(Models.DirectAssessmentBatchRecord record);


        ReceiptObj GetPayeReceipts(string phoneNumber, string receiptNumber, ReceiptStatus status, DateTime startDate, DateTime endDate, int skip, int take, bool queryForCount = false);
    }
}
