using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreReceiptService : IDependency
    {

        /// <summary>
        /// Generate receipt
        /// </summary>
        /// <param name="receipt">Receipt</param>
        /// <returns>Receipt</returns>
        /// <exception cref="CannotCreateReceiptException"></exception>
        Receipt SaveTransactionReceipt(Receipt receipt);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>TransactionLog</returns>
        ReceiptViewModel GetReceiptVMByReceiptNumber(string receiptNumber);


        /// <summary>
        /// Get receipt number by receipt Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string | Receipt number</returns>
        string GetReceiptNumberById(long id);


        /// <summary>
        /// Evict receipt object from cache
        /// </summary>
        /// <param name="receipt"></param>
        void EvictReceiptObject(Receipt receipt);


        /// <summary>
        /// Create receipt document
        /// <para>Pass return byte if receipt doc should be byte generated</para>
        /// </summary>
        /// <param name="receiptVM"></param>
        /// <param name="returnByte"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        CreateReceiptDocumentVM CreateReceiptDocument(ReceiptViewModel receiptVM, bool returnByte = false);

    }
}
