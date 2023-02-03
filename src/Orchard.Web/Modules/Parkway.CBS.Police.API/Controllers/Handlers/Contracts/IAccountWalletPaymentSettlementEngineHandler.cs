using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IAccountWalletPaymentSettlementEngineHandler : IDependency
    {
        /// <summary>
        /// Proccesses the request
        /// </summary>
        /// <param name="model"></param>
        APIResponse ProcessPaymentRequestCallBack(SettlementEnginePaymentStatusVM model);
    }
}
