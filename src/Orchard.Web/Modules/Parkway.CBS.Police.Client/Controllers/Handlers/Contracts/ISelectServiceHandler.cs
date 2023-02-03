using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface ISelectServiceHandler : IDependency
    {

        SelectServiceVM GetSelectServiceVM(UserDetailsModel userDetails);


        dynamic GetNextActionModelForSelectService(bool userIsLoggedIn);

        /// <summary>
        /// Get the service type and service prefix for this service Id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>PSServiceVM</returns>
        PSServiceVM GetServiceType(int serviceId, int categoryId);

        /// <summary>
        /// Get sub categories for this category
        /// </summary>
        /// <param name="parsedTaxEntityCategoryId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPSSSubCategories(int parsedTaxEntityCategoryId);

        /// <summary>
        /// Get sub categories for this tax entity sub category
        /// </summary>
        /// <param name="parsedTaxEntitySubCategoryId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPSSSubSubCategories(int parsedTaxEntitySubCategoryId);


        /// <summary>
        /// Get active default tax entity sub sub category vm for tax entity sub category with specified Id
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns>TaxEntitySubSubCategoryVM</returns>
        TaxEntitySubCategoryVM GetDefaultSubCategoryVM(int subCategoryId);


        /// <summary>
        /// Get active default tax entity sub sub category vm for tax entity sub category with specified Id
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns>TaxEntitySubSubCategoryVM</returns>
        TaxEntitySubSubCategoryVM GetDefaultSubSubCategoryVM(int subCategoryId);


        /// <summary>
        /// Get the services for this category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetServicePerCategory(int categoryId);


        /// <summary>
        /// Check if sub tax category exists
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <param name="categoryId"></param>
        /// <returns>bool</returns>
        bool CheckIfSubCategoryExists(int subCategoryId, int categoryId);

        /// <summary>
        /// Check if sub sub category exists
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <param name="subSubCategoryId"></param>
        /// <returns>bool</returns>
        bool CheckIfSubSubCategoryExists(int subCategoryId, int subSubCategoryId);

    }
}
