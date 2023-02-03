using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEBatchItemReceiptHandler : IDependency
    {
        /// <summary>
        /// Get receipts
        /// </summary>
        ReceiptsVM GetPAYEBatchItemReceipts(PAYEReceiptSearchParams receiptSearchParams);

        /// <summary>
        /// Get paginated record of PAYE batch items receipts
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        APIResponse GetPagedBatchItemReceiptData(string token, int page);
    }
}
