using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSSettlementBatchManager<PSSSettlementBatch> : IDependency, IBaseManager<PSSSettlementBatch>
    {
        /// <summary>
        /// Gets id for settlement batch with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        long GetSettlementBatchId(string batchRef);

        /// <summary>
        /// Gets settlement batch with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        PSSSettlementBatchVM GetSettlementBatchWithRef(string batchRef);
    }
}
