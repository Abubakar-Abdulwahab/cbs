using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSEscortServiceCategoryManager<PSSEscortServiceCategory> : IDependency, IBaseManager<PSSEscortServiceCategory>
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
        /// Gets category types for service category with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<PSSEscortServiceCategoryVM> GetCategoryTypesForServiceCategoryWithId(int id);
    }
}
