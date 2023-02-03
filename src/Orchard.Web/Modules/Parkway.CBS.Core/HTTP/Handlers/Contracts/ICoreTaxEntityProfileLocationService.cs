using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreTaxEntityProfileLocationService : IDependency
    {
        /// <summary>
        /// Creates new branch information for tax entity with specified Id
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="isDefault"></param>
        /// <returns>location id</returns>
        int CreateBranch(TaxEntityProfileLocationVM userInput, bool isDefault = false);

        /// <summary>
        /// Checks if branch location for tax entity with specified id already exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="taxEntityId"></param>
        bool CheckIfBranchWithAddressExists(string address, Int64 taxEntityId);

        /// <summary>
        /// Checks if branch name for tax entity with specified id already exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        bool CheckIfBranchWithNameExists(string name, Int64 taxEntityId);

        /// <summary>
        /// Gets locations of tax entity with the specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        IEnumerable<TaxEntityProfileLocationVM> GetTaxEntityLocations(long taxEntityId);

        /// <summary>
        /// Gets tax entity profile location with specified id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        TaxEntityProfileLocationVM GetTaxEntityLocationWithId(long taxEntityId, int locationId);
    }
}
