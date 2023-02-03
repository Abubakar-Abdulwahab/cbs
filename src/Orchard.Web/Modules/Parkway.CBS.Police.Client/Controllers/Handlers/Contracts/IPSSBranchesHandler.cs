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
    public interface IPSSBranchesHandler : IDependency
    {
        /// <summary>
        /// Gets PSSBranchVM
        /// </summary>
        /// <returns></returns>
        PSSBranchVM GetCreateBranchVM();

        /// <summary>
        /// Creates a new branch for currently logged in tax entity
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        void CreateNewBranch(TaxEntityProfileLocationVM userInput, ref List<ErrorModel> errors);

        /// <summary>
        /// Gets branch locations for currently logged in tax entity
        /// </summary>
        /// <param name="searchParams">search params</param>
        /// <returns>PSSBranchVM</returns>
        PSSBranchVM GetBranches(TaxEntityProfileLocationReportSearchParams searchParams);

        /// <summary>
        /// Gets Paged PSS Branches
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="page">page</param>
        /// <param name="taxEntityId">tax entity id</param>
        /// <returns>APIResponse</returns>
        APIResponse GetPagedBranchesData(string token, int? page, long taxEntityId);
    }
}
