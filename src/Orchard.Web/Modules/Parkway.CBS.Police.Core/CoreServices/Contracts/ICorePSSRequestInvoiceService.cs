using Orchard;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePSSRequestInvoiceService : IDependency
    {
        /// <summary>
        /// Validate that the request has been fully paid
        /// Here we get the request invoice details and do a validation
        /// to ascertain that there is no amount due and the invoice has not been cancelled</para>
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceNumber"></param>
        IEnumerable<PSServiceRequestInvoiceValidationDTO> ValidateProcessingFeeFullyPaid(long requestId, string invoiceNumber);
    }
}
