using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations.Contracts
{
    public interface IBatchInvoiceResponseProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        void GetCashFlowBatchInvoiceResponse(string tenantName, long batchId, string batchFileName);
    }
}
