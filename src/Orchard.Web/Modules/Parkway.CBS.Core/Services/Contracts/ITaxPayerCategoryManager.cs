using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxEntityCategoryManager<TaxPayerCategory> : IDependency, IBaseManager<TaxPayerCategory>
    {

        /// <summary>
        /// Get the list of categories
        /// </summary>
        /// <returns>IEnumerable{TaxEntityCategoryVM}</returns>
        IEnumerable<TaxEntityCategoryVM> GetCategories();


        /// <summary>
        /// Get the tax entity categories
        /// </summary>
        /// <returns>IEnumerable{TaxEntityCategoryVM}</returns>
        List<TaxEntityCategoryVM> GetTaxEntityCategoryVM();


        /// <summary>
        /// Get tax category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>TaxEntityCategoryVM</returns>
        TaxEntityCategoryVM GetTaxEntityCategoryVM(int categoryId);


        /// <summary>
        /// Get first category Id
        /// </summary>
        /// <returns>int</returns>
        int GetFirstCategoryId();

    }
}
