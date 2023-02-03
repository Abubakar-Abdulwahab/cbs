using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSSRequestInvoiceService : ICorePSSRequestInvoiceService
    {
        private readonly IPSSRequestInvoiceManager<PSSRequestInvoice> _iPSSRequestInvoiceManager;
        ILogger Logger { get; set; }

        public CorePSSRequestInvoiceService(IPSSRequestInvoiceManager<PSSRequestInvoice> iPSSRequestInvoiceManager)
        {
            _iPSSRequestInvoiceManager = iPSSRequestInvoiceManager;
        }


        /// <summary>
        /// Validate that the request has been fully paid
        /// <para>Here we get the request invoice details and do a validation
        /// to ascertain that there is no amount due and the invoice has not been cancelled</para>
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceNumber"></param>
        public IEnumerable<PSServiceRequestInvoiceValidationDTO> ValidateProcessingFeeFullyPaid(long requestId, string invoiceNumber)
        {
            //here we get the request invoice details for the request
            IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp = _iPSSRequestInvoiceManager.GetRequestInvoiceDetailsWithRequestIdAndInvoiceNumber(requestId, invoiceNumber);

            if (invoiceNumberGrp == null || !invoiceNumberGrp.Any())
            { throw new NoInvoicesMatchingTheParametersFoundException("404 for PSS application fee. Request Id " + requestId); }

            if (invoiceNumberGrp.First().AmountDue > 0.00m)
            { throw new InvoiceHasPaymentException("Invoice has pending payments Request Id " + requestId); }


            if (invoiceNumberGrp.First().InvoiceStatus == InvoiceStatus.WriteOff)
            {
                throw new InvoiceCancelledException(string.Format("Invoice has been written off invoice number {0}, request Id {1}, Payment date {2}, Cancellation date {3} ", invoiceNumberGrp.First().InvoiceNumber, requestId, invoiceNumberGrp.First().PaymentDate.HasValue ? invoiceNumberGrp.First().PaymentDate.Value.ToString() : "", invoiceNumberGrp.First().CancellationDate.HasValue ? invoiceNumberGrp.First().CancellationDate.Value.ToString() : ""));
            }

            return invoiceNumberGrp;
        }
    }
}