using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IUserProfileHandler : IDependency
    {

        /// <summary>
        /// Get model for confirm user profile
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns>RegisterPSSUserObj</returns>
        RegisterPSSUserObj GetVMToConfirmUserProfile(UserDetailsModel userDetails);


        /// <summary>
        /// Try save user input
        /// </summary>
        /// <param name="userInput">TaxEntityViewModel</param>
        /// <returns>TaxEntityProfileHelper</returns>
        TaxEntityProfileHelper TrySaveUserInfo(TaxEntityViewModel userInput, int categoryId);


        /// <summary>
        /// check this LGA Id is valid
        /// </summary>
        /// <param name="selectedStateLGA"></param>
        /// <returns>bool</returns>
        bool ValidateLGA(int lgaId, int stateId);


        /// <summary>
        /// Get next direction for request
        /// </summary>
        /// <returns>RouteNameAndStage</returns>
        RouteNameAndStage GetNextDirectionForRequest(int serviceTypeId);
    }
}
