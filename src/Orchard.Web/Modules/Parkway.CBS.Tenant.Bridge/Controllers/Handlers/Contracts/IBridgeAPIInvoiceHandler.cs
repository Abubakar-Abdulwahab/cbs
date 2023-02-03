using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts
{
    public interface IBridgeAPIInvoiceHandler : IDependency
    {

        /// <summary>
        /// Do invoice validation for merchant
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        APIResponse ValidateInvoice(ReadycashInvoiceValidationModel model);


        /// <summary>
        /// Do invoice validation for billers
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        APIResponse BillerValidateInvoice(ReadycashInvoiceValidationModel model, dynamic headerParams);

        /// <summary>
        /// This handles invoice validation for all the tenants 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse InvoiceValidation(ValidationRequest model, dynamic headerParams);

        /// <summary>
        /// This handles invoice creation 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        APIResponse CreateInvoice(CreateInvoiceModel model, dynamic headerParams);

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">InvoiceBridgeController</param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> DoModelCheck(InvoiceBridgeController callback);

    }
}
