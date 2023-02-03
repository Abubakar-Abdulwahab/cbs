using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreExtractCategory : IDependency
    {

        /// <summary>
        /// Get active extract categories
        /// </summary>
        /// <returns>IEnumerable{ExtractCategoryVM}</returns>
        IEnumerable<ExtractCategoryVM> GetActiveCategories();



        /// <summary>
        /// Get active extract sub categories for the given category Id
        /// </summary>
        /// <returns>ExtractCategoryVM</returns>
        ExtractCategoryVM GetActiveSubCategories(int categoryId);


        /// <summary>
        /// Gets active extract sub categories for the given category Id using future query
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        IEnumerable<ExtractCategoryVM> GetActiveSubCategoriesList(int categoryId);

        /// <summary>
        /// Checks if sub category exists for extract category with specified id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="subCategoryId"></param>
        /// <returns></returns>
        bool CheckIfSubCategoryExistsForCategory(int categoryId, int subCategoryId);

        /// <summary>
        /// Checks if category with specified id has free form
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        bool CheckIfCategoryHasFreeForm(int categoryId);

    }
}
