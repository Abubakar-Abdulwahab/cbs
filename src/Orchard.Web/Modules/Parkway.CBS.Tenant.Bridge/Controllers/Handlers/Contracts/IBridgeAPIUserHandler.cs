using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts
{
    public interface IBridgeAPIUserHandler : IDependency
    {

        /// <summary>
        /// Search for tax payer profile
        /// </summary>
        /// <param name="searchFilter">TaxProfilesSearchParams</param>
        /// <param name="headerParams">dynamic</param>
        /// <returns>APIResponse</returns>
        APIResponse SearchForTaxProfileByFilter(TaxProfilesSearchParams searchFilter, dynamic headerParams);

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">UserBridgeController</param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> DoModelCheck(UserBridgeController callback);

    }
}
