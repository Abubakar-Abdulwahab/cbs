using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientFileServices.Implementations.Contracts
{
    public interface ITaxPayerEnumerationFileUpload
    {
        /// <summary>
        /// Process tax payer enumeration line items for file upload as a background job.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="filePath"></param>
        /// <param name="tenantName"></param>
        void ProcessTaxPayerEnumerationFileUpload(long batchId, string filePath, string tenantName);
    }
}
