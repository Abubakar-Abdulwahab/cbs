using Parkway.CBS.Core.Payee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Services.PAYEAPI.Contracts
{
    public interface IPAYEBatchItemValidation
    {
        /// <summary>
        /// Start PAYE batch validation 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchIdentifier"></param>
        /// <param name="expertSystemId"></param>
        /// <param name="adapter"></param>
        void ProcessPAYEBatchItemsValidation(string tenantName, string batchIdentifier, int expertSystemId, AssessmentInterface adapter);

    }
}
