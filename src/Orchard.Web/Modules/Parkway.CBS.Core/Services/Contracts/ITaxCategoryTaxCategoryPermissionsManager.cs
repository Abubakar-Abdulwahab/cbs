using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxCategoryTaxCategoryPermissionsManager<TaxCategoryTaxCategoryPermissions> : IDependency, IBaseManager<TaxCategoryTaxCategoryPermissions>
    {
        /// <summary>
        /// Gets all active tax category permissions
        /// </summary>
        /// <returns></returns>
        IEnumerable<TaxCategoryTaxCategoryPermissionsVM> GetTaxCategoryPermissions();
    }
}
