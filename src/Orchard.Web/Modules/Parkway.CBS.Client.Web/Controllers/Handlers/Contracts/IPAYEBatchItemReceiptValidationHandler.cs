using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEBatchItemReceiptValidationHandler : IDependency
    {
        /// <summary>
        /// Get PAYE receip details using the receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        PAYEBatchItemReceiptViewModel GetPAYEBatchItemReceiptVM(string receiptNumber);

        /// <summary>
        /// Get PAYE receipt for download
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        CreateReceiptDocumentVM CreateReceiptFile(string receiptNumber);
    }
}
