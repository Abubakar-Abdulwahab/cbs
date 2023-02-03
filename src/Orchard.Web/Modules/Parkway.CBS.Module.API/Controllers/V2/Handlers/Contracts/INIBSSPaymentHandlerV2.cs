using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.API.Controllers.V2.Handlers.Contracts
{
    public interface INIBSSPaymentHandlerV2 : IDependency
    {
        /// <summary>
        /// Payment notification for NIBSS EBills pay (Encrypted JSON)
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="authorization"></param>
        /// <param name="hash"></param>
        /// <returns>APIResponse</returns>
        APIResponse NIBSSPaymentNotif(string signature, string authorization, string hash);

    }
}
