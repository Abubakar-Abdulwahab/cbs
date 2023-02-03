using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSSubUserHandler : IDependency
    {
        /// <summary>
        /// Get create sub user VM
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        PSSSubUserVM GetCreateSubUserVM(long taxEntityId);

        /// <summary>
        /// Creates a sub user
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="taxEntityCategoryId"></param>
        /// <param name="errors"></param>
        void CreateSubUser(PSSSubUserVM userInput, long taxEntityId, int taxEntityCategoryId, ref List<ErrorModel> errors);

        /// <summary>
        /// Gets sub users for currently logged in tax entity
        /// </summary>
        /// <param name="searchParams">search params</param>
        /// <returns>PSSBranchVM</returns>
        PSSSubUserVM GetSubUsers(CBSUserTaxEntityProfileLocationReportSearchParams searchParams);

        /// <summary>
        /// Gets Paged PSS Sub Users
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="page">page</param>
        /// <param name="taxEntityId">tax entity id</param>
        /// <returns>APIResponse</returns>
        APIResponse GetPagedSubUsersData(string token, int? page, long taxEntityId);


        /// <summary>
        /// Toggles registration status of sub user with specified user part record id
        /// </summary>
        /// <param name="subUserUserId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        APIResponse ToggleSubUserStatus(int subUserUserId, bool isActive);
    }
}
