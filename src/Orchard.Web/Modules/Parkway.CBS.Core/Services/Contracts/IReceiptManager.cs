using System;
using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IReceiptManager<Receipt> : IDependency, IBaseManager<Receipt>
    {

        /// <summary>
        /// Get the VM for a receipt
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>ReceiptViewModel</returns>
        ReceiptViewModel GetReceiptDetails(string receiptNumber);


        /// <summary>
        /// Get receipt Id for this receipt number
        /// </summary>
        /// <returns>Int64</returns>
        Int64 GetReceiptId(string receiptNumber);

    }
}
