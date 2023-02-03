using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICorePAYEBatchItemReceiptService : IDependency
    {
        /// <summary>
        /// Create PAYEBatchItemReceipt for download
        /// </summary>
        /// <param name="receiptVM"></param>
        /// <param name="returnByte"></param>
        /// <returns></returns>
        CreateReceiptDocumentVM CreateReceiptDocument(PAYEBatchItemReceiptViewModel receiptVM, bool returnByte = false);
    }
}
