using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSReceiptHandler : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        ReceiptDisplayVM SearchForInvoiceForPaymentView(string invoiceNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        ReceiptDetailsVM GetReceiptVM(string invoiceNumber, string receiptNumber);



        /// <summary>
        /// Get receipt file
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="invoiceNumber"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        CreateReceiptDocumentVM CreateReceiptFile(string invoiceNumber, string receiptNumber);
    }
}
