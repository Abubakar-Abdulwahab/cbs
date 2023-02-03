using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEBatchRecordManager<PAYEBatchRecord> : IDependency, IBaseManager<PAYEBatchRecord>
    {

        /// <summary>
        /// set the batch record payment completed value
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <param name="completed"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        void SetPaymentCompletedValue(long batchRecordId, bool completed);


        /// <summary>
        /// get batch record with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        PAYEBatchRecordVM GetBatchRecordWithRef(string batchRef);

    }
}