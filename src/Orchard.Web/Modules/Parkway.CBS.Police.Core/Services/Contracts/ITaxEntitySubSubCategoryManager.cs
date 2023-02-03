using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> : IDependency, IBaseManager<TaxEntitySubSubCategory>
    {
        /// <summary>
        /// Gets active sub categories for tax entity sub category with the specified Id
        /// </summary>
        /// <param name="subCategoryId">Id of the tax entity sub category</param>
        /// <returns>List of TaxEntitySubSubCategoryVM</returns>
        IEnumerable<TaxEntitySubSubCategoryVM> GetActiveTaxEntitySubSubCategoryBySubCategoryId(int subCategoryId);


        /// <summary>
        /// Get active default sub categories for tax entity sub category with specified Id
        /// </summary>
        /// <param name="subCategoryId">Id of the tax entity sub category</param>
        /// <returns>TaxEntitySubSubCategoryVM</returns>
        TaxEntitySubCategoryVM GetActiveDefaultTaxEntitySubCategoryById(int subCategoryId);


        TaxEntitySubSubCategoryVM GetActiveDefaultTaxEntitySubSubCategoryById(int subCategoryId);


        /// <summary>
        /// Check that this sub sub category has a relationship to subcategory
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subCategoryId"></param>
        /// <returns>bool</returns>
        bool CheckIfExists(int id, int subCategoryId);

    }
}
