using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class SelectServiceHandler : ISelectServiceHandler
    {
        private readonly Lazy<IServiceTaxCategoryManager<ServiceTaxCategory>> _serviceTaxCategoryManager;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ITaxEntitySubCategoryManager<TaxEntitySubCategory> _taxEntitySubCategoryRepository;
        private readonly ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> _taxEntitySubSubCategoryRepository;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }

        public SelectServiceHandler(ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ITaxEntitySubCategoryManager<TaxEntitySubCategory> taxEntiySubCategoryRepository, ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> taxEntitySubSubCategoryRepository, Lazy<IServiceTaxCategoryManager<ServiceTaxCategory>> serviceTaxCategoryManager, IOrchardServices orchardServices)
        {
            _taxCategoriesRepository = taxCategoriesRepository;
            _taxEntitySubCategoryRepository = taxEntiySubCategoryRepository;
            _taxEntitySubSubCategoryRepository = taxEntitySubSubCategoryRepository;
            Logger = NullLogger.Instance;
            _serviceTaxCategoryManager = serviceTaxCategoryManager;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Get vm for select services
        /// </summary>
        /// <returns>SelectServiceVM</returns>
        public SelectServiceVM GetSelectServiceVM(UserDetailsModel userDetails)
        {
            IEnumerable<PSSRequestTypeVM> services = null;
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            if (userDetails != null)
            {

                services = ObjectCacheProvider.GetCachedObject<IEnumerable<PSSRequestTypeVM>>(tenant, $"{nameof(POSSAPCachePrefix.ServiceCategory)}-{userDetails.CategoryVM.Id}");

                if (services == null)
                {
                    services = _serviceTaxCategoryManager.Value.GetAllActiveServices(userDetails.CategoryVM.Id);

                    if (services != null)
                    {
                        ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.ServiceCategory)}-{userDetails.CategoryVM.Id}", services);
                    }
                }
            }

            IEnumerable<TaxEntityCategoryVM> categories = ObjectCacheProvider.GetCachedObject<IEnumerable<TaxEntityCategoryVM>>(tenant, nameof(POSSAPCachePrefix.Category));

            if (categories == null)
            {
                categories = _taxCategoriesRepository.GetCategories();

                if (categories != null)
                {
                    ObjectCacheProvider.TryCache(tenant, nameof(POSSAPCachePrefix.Category), categories);
                }
            }

            return new SelectServiceVM
            {
                Services = services?.ToList(),
                HeaderObj = new HeaderObj { },
                TaxCategories = categories.ToList()
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userIsLoggedIn"></param>
        /// <returns></returns>
        public dynamic GetNextActionModelForSelectService(bool userIsLoggedIn)
        {
            if (userIsLoggedIn) { return new { RouteName = "P.ConfirmUserProfile", Stage = PSSUserRequestGenerationStage.ConfirmUserProfile }; }
            return GetDirectionForProfile();
        }


        /// <summary>
        /// Get the service type and service prefix for this service Id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>PSServiceVM</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public PSServiceVM GetServiceType(int serviceId, int categoryId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            PSServiceVM result = ObjectCacheProvider.GetCachedObject<PSServiceVM>(tenant, $"{nameof(POSSAPCachePrefix.ServiceType)}-{serviceId}-{categoryId}");

            if (result == null)
            {
                result = _serviceTaxCategoryManager.Value.GetServiceType(serviceId, categoryId);

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.ServiceType)}-{serviceId}-{categoryId}", result);
                }
            }

            if (!result.IsActive) { throw new NoRecordFoundException("Service is inactive"); }
            return result;
        }


        /// <summary>
        /// Do extract
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userIsLoggedIn"></param>
        /// <returns></returns>
        private dynamic GetDirectionForProfile()
        {
            return new { RouteName = "P.RequestUserProfile", Stage = PSSUserRequestGenerationStage.RequestUserFormStage };
        }


        /// <summary>
        /// Get sub categories for this category
        /// </summary>
        /// <param name="parsedTaxEntityCategoryId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPSSSubCategories(int parsedTaxEntityCategoryId)
        {
            try
            {
                string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

                IEnumerable<TaxEntitySubCategoryVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<TaxEntitySubCategoryVM>>(tenant, $"{nameof(POSSAPCachePrefix.Subcategory)}-{parsedTaxEntityCategoryId}");

                if (result == null)
                {
                    result = _taxEntitySubCategoryRepository.GetActiveTaxEntitySubCategoryByCategoryId(parsedTaxEntityCategoryId);

                    if (result != null)
                    {
                        ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.Subcategory)}-{parsedTaxEntityCategoryId}", result);
                    }
                }

                if (result == null) { return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().ToString() }; }

                return new APIResponse { ResponseObject = result };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception GetPSSSubCategories for Id {0}, Msg {1}", parsedTaxEntityCategoryId, exception.Message));
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
        }


        /// <summary>
        /// Get sub categories for this tax entity sub category
        /// </summary>
        /// <param name="parsedTaxEntitySubCategoryId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPSSSubSubCategories(int parsedTaxEntitySubCategoryId)
        {
            try
            {
                string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

                IEnumerable<TaxEntitySubSubCategoryVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<TaxEntitySubSubCategoryVM>>(tenant, $"{nameof(POSSAPCachePrefix.Subsubcategory)}-{parsedTaxEntitySubCategoryId}");

                if (result == null)
                {
                    result = _taxEntitySubSubCategoryRepository.GetActiveTaxEntitySubSubCategoryBySubCategoryId(parsedTaxEntitySubCategoryId);

                    if (result != null)
                    {
                        ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.Subsubcategory)}-{parsedTaxEntitySubCategoryId}", result);
                    }
                }

                if (result == null) { return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().ToString() }; }

                return new APIResponse { ResponseObject = result };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception GetPSSSubCategories for Id {0}, Msg {1}", parsedTaxEntitySubCategoryId, exception.Message));
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
        }


        /// <summary>
        /// Get active default tax entity sub sub category vm for tax entity sub category with specified Id
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns>TaxEntitySubSubCategoryVM</returns>
        public TaxEntitySubCategoryVM GetDefaultSubCategoryVM(int subCategoryId)
        {
            return _taxEntitySubSubCategoryRepository.GetActiveDefaultTaxEntitySubCategoryById(subCategoryId);
        }

        /// <summary>
        /// Get active default tax entity sub sub category vm for tax entity sub category with specified Id
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns>TaxEntitySubSubCategoryVM</returns>
        public TaxEntitySubSubCategoryVM GetDefaultSubSubCategoryVM(int subCategoryId)
        {
            return _taxEntitySubSubCategoryRepository.GetActiveDefaultTaxEntitySubSubCategoryById(subCategoryId);
        }


        /// <summary>
        /// Get the services for this category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetServicePerCategory(int categoryId)
        {
            return new APIResponse { ResponseObject = _serviceTaxCategoryManager.Value.GetAllActiveServices(categoryId) };
        }

        /// <summary>
        /// Check if sub tax category exists
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns>bool</returns>
        public bool CheckIfSubCategoryExists(int subCategoryId, int categoryId)
        {
            return _taxEntitySubCategoryRepository.SubCategoryExists(subCategoryId, categoryId);
        }


        /// <summary>
        /// Check if sub sub category exists
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <param name="subSubCategoryId"></param>
        /// <returns>bool</returns>
        public bool CheckIfSubSubCategoryExists(int subCategoryId, int id)
        {
            return _taxEntitySubSubCategoryRepository.CheckIfExists(id, subCategoryId);
        }

    }
}