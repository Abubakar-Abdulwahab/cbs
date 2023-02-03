using Orchard.Logging;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreRequestStatusLog : ICoreRequestStatusLog
    {

        private readonly IRequestStatusLogManager<RequestStatusLog> _reqRepo;
        public ILogger Logger { get; set; }

        public CoreRequestStatusLog(IRequestStatusLogManager<RequestStatusLog> reqRepo)
        {
            _reqRepo = reqRepo;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// once payment has been confirmed on an invoice
        /// We need to set the fulfilled flag on the status to true
        /// so as to mark that item as being fulfilled for display/reporting purposes
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="definitionLevelId"></param>
        /// <param name="invoiceId"></param>
        public void UpdateStatusLogAfterPaymentConfirmation(long requestId, int definitionLevelId, long invoiceId)
        {
            _reqRepo.UpdateStatusToFulfilledAfterPayment(requestId, definitionLevelId, invoiceId);
        }


        /// <summary>
        /// once payment has been confirmed on an invoice
        /// We need to set the fulfilled flag on the status to true
        /// so as to mark that item as being fulfilled for display/reporting purposes
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        public void UpdateStatusLogAfterPaymentConfirmation(long requestId, long invoiceId)
        {
            _reqRepo.UpdateStatusToFulfilledAfterPayment(requestId, invoiceId);
        }
    }
}