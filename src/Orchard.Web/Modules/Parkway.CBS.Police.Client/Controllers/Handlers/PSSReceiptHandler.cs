using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSReceiptHandler : IPSSReceiptHandler
    {
        public ILogger Logger { get; set; }
        private readonly Lazy<ICorePSServiceRequest> _coreInvoiceService;
        private readonly Lazy<ICorePSSReceiptService> _coreReceiptService;

        public PSSReceiptHandler(Lazy<ICorePSServiceRequest> coreInvoiceService, Lazy<ICorePSSReceiptService> coreReceiptService)
        {
            Logger = NullLogger.Instance;
            _coreInvoiceService = coreInvoiceService;
            _coreReceiptService = coreReceiptService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public ReceiptDisplayVM SearchForInvoiceForPaymentView(string invoiceNumber)
        {
            return _coreInvoiceService.Value.GetInvoiceReceiptsVM(invoiceNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public ReceiptDetailsVM GetReceiptVM(string invoiceNumber, string receiptNumber)
        {
            try
            {
                var result = _coreInvoiceService.Value.GetInvoiceReceiptVM(invoiceNumber, receiptNumber);
                if (result == null) { throw new NoRecordFoundException("No record found for receipt number " + receiptNumber); }
                return result;
            }
            catch (NoRecordFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Get receipt file
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="invoiceNumber"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public CreateReceiptDocumentVM CreateReceiptFile(string invoiceNumber, string receiptNumber)
        {
            ReceiptDetailsVM vm = _coreInvoiceService.Value.GetInvoiceReceiptVM(invoiceNumber, receiptNumber);
            if(vm == null) { throw new NoRecordFoundException(string.Format("No receipt record found invoice number {0}, receipt number {1}", invoiceNumber, receiptNumber)); }
            return _coreReceiptService.Value.CreateReceiptDocument(vm, false);
        }
    }
}