using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSExtractSubCategoryHandler : IDependency
    {

        /// <summary>
        /// Get sub categories for this category
        /// </summary>
        /// <param name="parsedCategoryVal"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetSubCategories(int parsedCategoryVal);

    }
}
