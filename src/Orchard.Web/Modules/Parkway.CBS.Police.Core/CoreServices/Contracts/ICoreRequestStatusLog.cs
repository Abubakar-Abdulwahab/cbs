using Orchard;


namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreRequestStatusLog : IDependency
    {

        /// <summary>
        /// once payment has been confirmed on an invoice
        /// We need to set the fulfilled flag on the status to true
        /// so as to mark that item as being fulfilled for display/reporting purposes
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="definitionLevelId"></param>
        /// <param name="invoiceId"></param>
        void UpdateStatusLogAfterPaymentConfirmation(long requestId, int definitionLevelId, long invoiceId);


        /// <summary>
        /// once payment has been confirmed on an invoice
        /// We need to set the fulfilled flag on the status to true
        /// so as to mark that item as being fulfilled for display/reporting purposes
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        void UpdateStatusLogAfterPaymentConfirmation(long requestId, long invoiceId);
    }
}
