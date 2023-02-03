using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IRequestStatusLogManager<RequestStatusLog> : IDependency, IBaseManager<RequestStatusLog>
    {

        /// <summary>
        /// When payment has been confirmed we need to set the fulfilled flag to true
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="definitionLevelId"></param>
        /// <param name="invoiceId"></param>
        void UpdateStatusToFulfilledAfterPayment(long requestId, int definitionLevelId, long invoiceId);


        /// <summary>
        /// When payment has been confirmed we need to set the fulfilled flag to true
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        void UpdateStatusToFulfilledAfterPayment(long requestId, long invoiceId);


        /// <summary>
        /// Get list of request status log VMs for request with specified Id
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        List<PSSRequestStatusLogVM> GetRequestStatusLogVMById(long reqId);


        /// <summary>
        /// Get list of request status log VMs for request with specified fileRefNumber
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        IEnumerable<PSSRequestStatusLogVM> GetRequestStatusLogVMByFileRefNumber(string fileRefNumber);
    }
}
