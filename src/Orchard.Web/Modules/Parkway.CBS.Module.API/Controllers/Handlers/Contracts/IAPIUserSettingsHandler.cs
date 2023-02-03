using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIUserSettingsHandler : IDependency
    {

        /// <summary>
        /// Create a CBS user
        /// </summary>
        /// <param name="callback">UserController</param>
        /// <param name="model">RegisterCBSUserModel</param>
        /// <param name="headerParams">dynamic</param>
        /// <returns>APIResponse</returns>
        APIResponse CreateCBSUser(UserController callback, RegisterUserModel model, dynamic headerParams);


        /// <summary>
        /// Search for tax payer profile
        /// </summary>
        /// <param name="searchFilter">TaxProfilesSearchParams</param>
        /// <param name="headerParams">dynamic</param>
        /// <returns>APIResponse</returns>
        APIResponse SearchTaxProfilesByFilter(TaxProfilesSearchParams searchFilter, dynamic headerParams);


        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="userController"></param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> DoModelCheck(UserController userController);

    }
}
