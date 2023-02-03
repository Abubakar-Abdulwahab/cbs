using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Services.Contracts;


namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ITaxEntitySubCategoryManager<TaxEntitySubCategory> : IDependency, IBaseManager<TaxEntitySubCategory>
    {
        /// <summary>
        /// Gets active sub categories for tax entity category with the specified Id
        /// </summary>
        /// <param name="categoryId">Id of the tax entity category</param>
        /// <returns>List of TaxEntitySubCategoryVM</returns>
        IEnumerable<TaxEntitySubCategoryVM> GetActiveTaxEntitySubCategoryByCategoryId(int categoryId);


        /// <summary>
        /// Check if subcategory exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryId"></param>
        /// <returns>bool</returns>
        bool SubCategoryExists(int id, int categoryId);

    }
}
