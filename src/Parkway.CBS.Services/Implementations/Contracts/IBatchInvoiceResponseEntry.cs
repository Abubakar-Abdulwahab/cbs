using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.Implementations.Contracts
{
    public interface IBatchInvoiceResponseEntry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="generalBatchReferenceId"></param>
        void CallImplementingClass(string tenantName, string generalBatchReferenceId, string batchFileName);
    }
}
