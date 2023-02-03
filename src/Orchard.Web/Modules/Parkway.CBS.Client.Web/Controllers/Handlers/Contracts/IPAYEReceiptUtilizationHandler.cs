using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEReceiptUtilizationHandler : IDependency
    {
        /// <summary>
        /// Get paginated PAYE receipts
        /// </summary>
        PAYEReceiptObj GetPAYEReceipts(PAYEReceiptSearchParams receiptSearchParams);


        /// <summary>
        /// Gets receipt utilizations report for a specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        PAYEReceiptUtilizationReportObj GetUtilizationsReport(string receiptNumber);

        /// <summary>
        /// Get paginated PAYE receipts list
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPagedPAYEReceiptData(string token, int page);
    }
}
