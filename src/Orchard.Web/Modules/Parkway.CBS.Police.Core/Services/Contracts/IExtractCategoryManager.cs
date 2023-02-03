using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IExtractCategoryManager<ExtractCategory> : IDependency, IBaseManager<ExtractCategory>
    {


        /// <summary>
        /// Get active extract categories
        /// </summary>
        /// <returns>IEnumerable{ExtractCategoryVM}</returns>
        IEnumerable<ExtractCategoryVM> GetActiveCategories();


        /// <summary>
        /// Get active sub categories for this category Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>ExtractCategoryVM</returns>
        ExtractCategoryVM GetActiveSubCategories(int categoryId);

        /// <summary>
        /// Get active sub categories for this category Id. Uses future queries for batching multiple calls
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>ExtractCategoryVM</returns>
        IEnumerable<ExtractCategoryVM> GetActiveSubCategoriesList(int categoryId);
    }
}
