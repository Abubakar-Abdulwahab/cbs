using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using static Parkway.CBS.Core.HTTP.Handlers.CoreNumberToWordsService;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEBatchItemReceiptValidationHandler : IPAYEBatchItemReceiptValidationHandler
    {
        private readonly IPAYEBatchItemReceiptManager<PAYEBatchItemReceipt> _payeBatchItemReceiptManager;
        private readonly ICorePAYEBatchItemReceiptService _payeBatchItemService;

        public PAYEBatchItemReceiptValidationHandler(IPAYEBatchItemReceiptManager<PAYEBatchItemReceipt> payeBatchItemReceiptManager, ICorePAYEBatchItemReceiptService payeBatchItemService)
        {
            _payeBatchItemReceiptManager = payeBatchItemReceiptManager;
            _payeBatchItemService = payeBatchItemService;
        }

        public PAYEBatchItemReceiptViewModel GetPAYEBatchItemReceiptVM(string receiptNumber)
        {
            try
            {
                var result = _payeBatchItemReceiptManager.GetReceiptDetails(receiptNumber);
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


        public CreateReceiptDocumentVM CreateReceiptFile(string receiptNumber)
        {
            try
            {
                PAYEBatchItemReceiptViewModel receiptVM = _payeBatchItemReceiptManager.GetReceiptDetails(receiptNumber);
                if (receiptVM == null) { throw new NoRecordFoundException("No record found for receipt number " + receiptNumber); }
                string[] taxAmt = AmountInWords(receiptVM.TaxAmountPaid).Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);
                if(taxAmt.Length > 13)
                {
                    receiptVM.TaxAmountPaidInWordsFirstLine = string.Join(" ", taxAmt, 0, 12);
                    receiptVM.TaxAmountPaidInWordsSecondLine = string.Join(" ", taxAmt, 12, (taxAmt.Length - 12));
                }
                else
                {
                    receiptVM.TaxAmountPaidInWordsFirstLine = string.Join(" ", taxAmt);
                }
                return _payeBatchItemService.CreateReceiptDocument(receiptVM, false);
            }
            catch (Exception) { throw; }
        }

    }
}