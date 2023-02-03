using Orchard;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Module.API.Controllers.V2.Handlers.Contracts
{
    public interface IAPINIBSSInvoiceHandlerV2 : IDependency
    {
        /// <summary>
        /// Do invoice validate for NIBSS Ebills Pay (Encrypted JSON)
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="authorization"></param>
        /// <param name="hash"></param>
        /// <returns>APIResponse</returns>
        APIResponse ValidateInvoiceNIBSS(string signature, string authorization, string hash);

    }
}
