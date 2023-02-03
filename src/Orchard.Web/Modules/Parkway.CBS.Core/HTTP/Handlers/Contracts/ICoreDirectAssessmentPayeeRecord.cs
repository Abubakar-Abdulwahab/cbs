using Orchard;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreDirectAssessmentPayeeRecord : IDependency
    {

        /// <summary>
        /// Save payee records
        /// </summary>
        /// <param name="payees"></param>
        /// <param name="recordId"></param>
        /// <param name="entity"></param>
        void SaveRecords(List<PayeeAssessmentLineRecordModel> payees, long recordId, TaxEntity entity);
    }
}
