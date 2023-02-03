using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePSSEscortServiceCategory : IDependency
    {
        /// <summary>
        /// Gets all active escort service categories
        /// </summary>
        /// <returns></returns>
        IEnumerable<PSSEscortServiceCategoryVM> GetEscortServiceCategories();

        /// <summary>
        /// Gets an active escort service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PSSEscortServiceCategoryVM GetEscortServiceCategoryWithId(int id);

        /// <summary>
        /// Checks if category type for specified service category exists
        /// </summary>
        /// <param name="serviceCategoryId"></param>
        /// <param name="categoryTypeId"></param>
        /// <returns></returns>
        bool CheckIfCategoryTypeInServiceCategory(int serviceCategoryId, int categoryTypeId);

        /// <summary>
        /// Gets category types for service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<PSSEscortServiceCategoryVM> GetCategoryTypesForServiceCategoryWithId(int id);
    }
}
