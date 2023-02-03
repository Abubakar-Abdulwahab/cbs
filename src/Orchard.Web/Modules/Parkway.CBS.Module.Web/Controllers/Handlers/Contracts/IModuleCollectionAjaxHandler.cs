using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.Contracts
{
    public interface IModuleCollectionAjaxHandler : IDependency, ICommonBaseHandler
    {
        /// <summary>
        /// Get the tax payer profiles by category id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetTaxProfilesByCategory(string categoryId);

        /// <summary>
        /// Get Payment Reference using the Invoice Number
        /// </summary>
        /// <param name="InvoiceId"></param>
        /// <param name="provider"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPaymentReferenceNumber(int InvoiceId, string InvoiceNumber, PaymentProvider provider);

        /// <summary>
        /// Get LGAs that belong to a particular State Id
        /// </summary>
        /// <param name="StateId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetLgasByStates(string StateId);


        /// <summary>
        /// Send notification for the specified payment reference and provider
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <param name="provider"></param>
        /// <returns>APIResponse</returns>
        APIResponse SendNotification(string paymentReference, PaymentProvider provider);



    }
}
