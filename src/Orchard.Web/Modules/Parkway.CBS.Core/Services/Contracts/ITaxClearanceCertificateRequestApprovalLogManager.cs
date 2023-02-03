using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxClearanceCertificateRequestApprovalLogManager<TaxClearanceCertificateRequestApprovalLog> : IDependency, IBaseManager<TaxClearanceCertificateRequestApprovalLog>
    {
        /// <summary>
        /// Get approval logs for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>List<TCCApprovalLogVM></returns>
        List<TCCApprovalLogVM> GetApprovalLogForRequestById(long requestId);
    }
}
