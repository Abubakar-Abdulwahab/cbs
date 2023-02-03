using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IExtractSubCategoryManager<ExtractSubCategory> : IDependency, IBaseManager<ExtractSubCategory>
    {

        /// <summary>
        /// Get the sub category with the given Id
        /// </summary>
        /// <param name="catId"></param>
        /// <param name="subCatId"></param>
        /// <returns>ExtractSubCategoryVM</returns>
        ExtractSubCategoryVM GetSubCategory(int catId, int subCatId1);


    }
}
