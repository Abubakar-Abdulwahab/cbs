using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSRequestInvoiceManager<PSSRequestInvoice> : IDependency, IBaseManager<PSSRequestInvoice>
    {
        /// <summary>
        /// Gets CBSUserVM for invoice with specified invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        CBSUserVM GetCBSUserWithInvoiceNumber(string invoiceNumber);

        /// <summary>
        /// Get PSSRequestInvoice with specified invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        PSSRequestInvoiceDTO GetPSSRequestInvoiceWithInvoiceNumber(string invoiceNumber);

        /// <summary>
        /// Gets request invoice details with specified request id and invoice number
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceNumber"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        IEnumerable<PSServiceRequestInvoiceValidationDTO> GetRequestInvoiceDetailsWithRequestIdAndInvoiceNumber(long requestId, string invoiceNumber);
    }   
}
