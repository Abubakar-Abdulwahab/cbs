using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIStateTINHandler : IDependency
    {
        /// <summary>
        /// Create a State TIN and CBS user
        /// </summary>
        /// <param name="callback">UserController</param>
        /// <param name="model">RegisterCBSUserModel</param>
        /// <param name="headerParams">dynamic</param>
        /// <returns>APIResponse</returns>
        APIResponse CreateStateTIN(CreateStateTINModel model, dynamic headerParams);

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">StateTINController</param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> DoModelCheck(StateTINController callback);
    }
}
