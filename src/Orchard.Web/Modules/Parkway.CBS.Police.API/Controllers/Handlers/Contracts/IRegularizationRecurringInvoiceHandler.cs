using Orchard;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IRegularizationRecurringInvoiceHandler : IDependency
    {
        /// <summary>
        /// this method checks to confirm that the invoie generated for this request has been fully paid for
        /// </summary>
        /// <param name="requestToken"></param>
        /// <param name="invoiceNumber"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        IEnumerable<PSServiceRequestInvoiceValidationDTO> ConfirmRequestInvoiceFee(string requestToken, string invoiceNumber);

        /// <summary>
        /// Synchronize recurring invoice payment
        /// </summary>
        /// <param name="invoiceNumberGrp"></param>
        void DoProcessing(IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp);
    }
}
