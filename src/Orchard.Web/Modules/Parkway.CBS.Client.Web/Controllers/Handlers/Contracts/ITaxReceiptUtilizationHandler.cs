using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface ITaxReceiptUtilizationHandler : IDependency
    {
        /// <summary>
        /// Get Receipt Utilization VM for schedule with specified Batch Ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        ReceiptUtilizationVM GetVM(string batchRef);

        /// <summary>
        /// Get PAYE Receipt with specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="userId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPAYEReceipt(string receiptNumber, long userId);

        /// <summary>
        /// Apply receipt with specified receipt number to batch with specified batch ref
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="batchRef"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        APIResponse ApplyReceiptToBatch(string receiptNumber, string batchRef, long userId);

        /// <summary>
        /// When a user is signed in, we would like to redirect the user to a page where they can progress with their invoice generation
        /// </summary>
        /// <param name="user"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>ProceedWithInvoiceGenerationVM</returns>
        ProceedWithInvoiceGenerationVM GetModelWhenUserIsSignedIn(UserDetailsModel user, int revenueHeadId, int categoryId);

        /// <summary>
        /// Encrypt specified batch token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        string EncryptBatchToken(string token);

        /// <summary>
        /// Get Revenue Head Id for PAYE Assessment
        /// </summary>
        /// <returns></returns>
        int GetRevenueHeadIdForPAYE();
    }
}
