using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts
{
    public interface IBridgeAPIStateTINHandler : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        APIResponse CreateStateTIN(StateTINBridgeController callback, CreateStateTINModel model, dynamic headerParams);

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">StateTINBridgeController</param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> DoModelCheck(StateTINBridgeController callback);

    }
}
