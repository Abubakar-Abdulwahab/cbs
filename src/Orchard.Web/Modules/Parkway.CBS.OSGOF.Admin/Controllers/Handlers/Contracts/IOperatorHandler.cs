using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System.Collections.Generic;

namespace Parkway.CBS.OSGOF.Admin.Controllers.Handlers.Contracts
{
    public interface IOperatorHandler : IDependency
    {
        /// <summary>
        /// Get the list of active tax entity categories
        /// </summary>
        /// <returns>List of categories</returns>
        IEnumerable<TaxEntityCategoryViewModel> GetTaxCategories();

        RegisteredOperatorVM CreateCBSUser(SiteOperatorViewModel model);

        IEnumerable<TaxEntityProfileVM> GetCellSiteOperators();

        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns>TaxPayerWithDetails</returns>
        TaxPayerWithDetails GetOperator(string payerId);
        
    }
}
