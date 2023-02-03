using Orchard.Logging;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;
using System;
using Orchard;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreExtractCategory : ICoreExtractCategory
    {
        private readonly IExtractCategoryManager<ExtractCategory> _categoryRepo;
        private readonly Lazy<IExtractSubCategoryManager<ExtractSubCategory>> _subCategoryRepo;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }


        public CoreExtractCategory(IExtractCategoryManager<ExtractCategory> categoryRepo, Lazy<IExtractSubCategoryManager<ExtractSubCategory>> subCategoryRepo, IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _categoryRepo = categoryRepo;
            _subCategoryRepo = subCategoryRepo;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Get active extract categories
        /// </summary>
        /// <returns>IEnumerable{ExtractCategoryVM}</returns>
        public IEnumerable<ExtractCategoryVM> GetActiveCategories()
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            IEnumerable<ExtractCategoryVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<ExtractCategoryVM>>(tenant, $"{nameof(POSSAPCachePrefix.ExtractAllsubcategory)}");

            if (result == null)
            {
                result = _categoryRepo.GetActiveCategories();

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.ExtractAllsubcategory)}", result);
                }
            }

            return result;
        }


        /// <summary>
        /// Get active extract sub categories for the given category Id
        /// </summary>
        /// <returns>ExtractCategoryVM</returns>
        public ExtractCategoryVM GetActiveSubCategories(int categoryId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            ExtractCategoryVM result = ObjectCacheProvider.GetCachedObject<ExtractCategoryVM>(tenant, $"{nameof(POSSAPCachePrefix.Extractsubcategory)}-{categoryId}");

            if (result == null)
            {
                result = _categoryRepo.GetActiveSubCategories(categoryId);

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.Extractsubcategory)}-{categoryId}", result);
                }
            }

            return result;
        }


        /// <summary>
        /// Gets active extract sub categories for the given category Id using future query
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<ExtractCategoryVM> GetActiveSubCategoriesList(int categoryId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            IEnumerable<ExtractCategoryVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<ExtractCategoryVM>>(tenant, $"{nameof(POSSAPCachePrefix.ExtractAllsubcategory)}-{categoryId}");

            if (result == null)
            {
                result = _categoryRepo.GetActiveSubCategoriesList(categoryId);

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.ExtractAllsubcategory)}-{categoryId}", result);
                }
            }

            return result;
        }


        /// <summary>
        /// Checks if sub category exists for extract category with specified id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="subCategoryId"></param>
        /// <returns></returns>
        public bool CheckIfSubCategoryExistsForCategory(int categoryId, int subCategoryId)
        {
            return _subCategoryRepo.Value.Count(x => (x.ExtractCategory == new ExtractCategory { Id = categoryId }) && (x.Id == subCategoryId)) > 0;
        }


        /// <summary>
        /// Checks if category with specified id has free form
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public bool CheckIfCategoryHasFreeForm(int categoryId)
        {
            return _categoryRepo.Count(x => x.Id == categoryId && x.FreeForm) > 0;
        }

    }
}