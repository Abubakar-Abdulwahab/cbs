using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreExtractSubCategory : IDependency
    {

        /// <summary>
        /// Get active extract sub categories for the give category Id
        /// </summary>
        /// <returns>IEnumerable{ExtractSubCategoryVM}</returns>
        IEnumerable<ExtractSubCategoryVM> GetActiveSubCategories(int categoryId);
    }
}
