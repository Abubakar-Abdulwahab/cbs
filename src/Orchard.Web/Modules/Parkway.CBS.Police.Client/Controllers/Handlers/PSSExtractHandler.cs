using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSExtractHandler : IPSSExtractHandler
    {
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly Lazy<ICoreCommand> _coreCommandService;
        private readonly Lazy<ICoreExtractCategory> _coreExtractCategory;
        private readonly Lazy<ICoreExtractDetails> _coreExtractDetails;
        private readonly IPSServiceCaveatManager<PSServiceCaveat> _caveatRepo;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }


        public PSSExtractHandler(ICoreStateAndLGA coreStateLGAService, Lazy<ICoreCommand> coreCommandService, Lazy<ICoreExtractCategory> coreExtractCategory, Lazy<ICoreExtractDetails> coreExtractDetails, IPSServiceCaveatManager<PSServiceCaveat> caveatRepo, IOrchardServices orchardServices)
        {
            _coreStateLGAService = coreStateLGAService;
            _coreCommandService = coreCommandService;
            _coreExtractCategory = coreExtractCategory;
            Logger = NullLogger.Instance;
            _coreExtractDetails = coreExtractDetails;
            _caveatRepo = caveatRepo;
            _orchardServices = orchardServices;
        }



        /// <summary>
        /// Get VM for police extract
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>ExtractRequestVM</returns>
        public ExtractRequestVM GetVMForPoliceExtract(int serviceId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            PSServiceCaveatVM caveat = ObjectCacheProvider.GetCachedObject<PSServiceCaveatVM>(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}");

            if (caveat == null)
            {
                caveat = _caveatRepo.GetServiceCaveat(serviceId);

                if (caveat != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}", caveat);
                }
            }

            return new ExtractRequestVM
            {
                HeaderObj = new HeaderObj { },
                StateLGAs = _coreStateLGAService.GetStates(),
                ExtractCategories = _coreExtractCategory.Value.GetActiveCategories().ToList(),
                Caveat = caveat
            };
        }


        /// <summary>
        /// Get the list of commands for this lgaId
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        public List<CommandVM> GetListOfCommands(int lgaId)
        {
            return _coreCommandService.Value.GetAreaAndDivisionalCommandsByLGA(lgaId).ToList();
        }

        /// <summary>
        /// Get the list of commands for this stateId
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public List<CommandVM> GetListOfCommandsByStateId(int stateId)
        {
            return _coreCommandService.Value.GetAreaAndDivisionalCommandsByStateId(stateId).ToList();
        }


        /// <summary>
        /// Get next action direction for extract
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public dynamic GetNextDirectionForConfirmation()
        {
            return new { RouteName = "P.Request.Confirm", Stage = PSSUserRequestGenerationStage.PSSRequestConfirmation };
        }


        /// <summary>
        /// Validate  and get the command details
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="selectedStateLGA"></param>
        /// <param name="selectedCommand"></param>
        /// <returns>CommandVM</returns>
        public CommandVM ValidateSelectedCommand(PSSExtractController callback, int selectedState, int selectedStateLGA, int selectedCommand)
        {
            CommandVM command = _coreCommandService.Value.GetCommandDetails(selectedCommand);
            if (command.StateId != selectedState) //|| command.LGAId != selectedStateLGA)
            {
                callback.ModelState.AddModelError("Command", "Select a valid Command.");
                throw new DirtyFormDataException();
            }
            return command;
        }


        /// <summary>
        /// Validates selected extract categories and sub categories
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="userInput"></param>
        /// <param name="reason"></param>
        public void ValidateExtractCategoriesAndSubCategories(PSSExtractController callback, ExtractRequestVM userInput, string reason)
        {
            if (userInput.SelectedCategories.Count() <= 0 || string.IsNullOrEmpty(userInput.SelectedCategoriesAndSubCategories))
            {
                callback.ModelState.AddModelError("SelectedCategory", "Select a valid category.");
                throw new DirtyFormDataException();
            }

            if ((userInput.SelectedSubCategories == null || userInput.SelectedSubCategories.Count() <= 0) && !_coreExtractCategory.Value.CheckIfCategoryHasFreeForm(userInput.SelectedCategories.FirstOrDefault()))
            {
                callback.ModelState.AddModelError("SelectedSubCategory", "Select a valid sub category.");
                throw new DirtyFormDataException();
            }

            IDictionary<int, IEnumerable<int>> selectedCategoriesAndSubCategories = JsonConvert.DeserializeObject<IDictionary<int, IEnumerable<int>>>(userInput.SelectedCategoriesAndSubCategories);
            if (selectedCategoriesAndSubCategories.Count() == 0)
            {
                callback.ModelState.AddModelError("SelectedCategory", "Select a valid category.");
                throw new DirtyFormDataException();
            }

            foreach (var selectedCategoryAndSubCategory in selectedCategoriesAndSubCategories)
            {
                if (selectedCategoryAndSubCategory.Key < 1)
                {
                    callback.ModelState.AddModelError("SelectedCategory", "Select a valid category.");
                    throw new DirtyFormDataException();
                }

                if (_coreExtractCategory.Value.CheckIfCategoryHasFreeForm(selectedCategoryAndSubCategory.Key))
                {
                    if (string.IsNullOrEmpty(reason))
                    {
                        callback.ModelState.AddModelError("Reason", "Reason field must be between 20 to 1000 characters long.");
                        throw new DirtyFormDataException();
                    }
                    if (reason.Length > 1000 || reason.Length < 20) { callback.ModelState.AddModelError("Reason", "Reason field must be between 20 to 1000 characters long."); throw new DirtyFormDataException(); }
                    continue;
                }

                foreach (var selectedSubCategory in selectedCategoryAndSubCategory.Value)
                {
                    if (selectedSubCategory < 0)
                    {
                        callback.ModelState.AddModelError("SelectedSubCategory", "Select a valid sub category.");
                        throw new DirtyFormDataException();
                    }
                    if (selectedSubCategory == 0) { continue; }
                    if (!_coreExtractCategory.Value.CheckIfSubCategoryExistsForCategory(selectedCategoryAndSubCategory.Key, selectedSubCategory))
                    {
                        callback.ModelState.AddModelError("SelectedCategory", "Select a valid category.");
                        callback.ModelState.AddModelError("SelectedSubCategory", "Select a valid sub category.");
                        throw new DirtyFormDataException();
                    }
                }
            }
            userInput.SelectedCategoriesAndSubCategoriesDeserialized = selectedCategoriesAndSubCategories;
        }


        /// <summary>
        /// Do validation for extract category
        /// </summary>
        /// <param name="selectedCategory"></param>
        /// <param name="selectedSubCategory"></param>
        /// <param name="reason"></param>
        public string ValidateExtractCategory(PSSExtractController callback, int selectedCategory, int selectedSubCategory, string reason)
        {
            //check int value
            if (selectedCategory <= 0)
            {
                callback.ModelState.AddModelError("SelectedCategory", "Select a valid category.");
                throw new DirtyFormDataException();
            }
            //get this sub category
            ExtractCategoryVM category = _coreExtractCategory.Value.GetActiveSubCategories(selectedCategory);
            if (category == null)
            {
                callback.ModelState.AddModelError("SelectedCategory", "Select a valid sub-category.");
                throw new DirtyFormDataException();
            }
            //check if free form
            if (category.FreeForm)
            {
                if (string.IsNullOrEmpty(reason))
                {
                    callback.ModelState.AddModelError("Reason", "Reason field must be between 20 to 1000 characters long.");
                    throw new DirtyFormDataException();
                }
                if (reason.Length > 1000 || reason.Length < 20) { callback.ModelState.AddModelError("Reason", "Reason field must be between 20 to 100 characters long."); throw new DirtyFormDataException(); }
                return reason;
            }

            if (category.SubCategories == null || !category.SubCategories.Any())
            {
                callback.ModelState.AddModelError("SelectedCategory", "No subcategory found. Select a valid category.");
                throw new DirtyFormDataException();
            }
            //get sub category
            try
            {
                ExtractSubCategoryVM subCategory = category.SubCategories.Where(sub => sub.Id == selectedSubCategory).Single();
                if (subCategory.FreeForm)
                {
                    if (string.IsNullOrEmpty(reason))
                    {
                        callback.ModelState.AddModelError("Reason", "Reason field must be between 20 to 1000 characters long.");
                        throw new DirtyFormDataException();
                    }
                    reason = reason.Trim();
                    if (reason.Length > 1000 || reason.Length < 20)
                    {
                        callback.ModelState.AddModelError("Reason", "Reason field must be between 20 to 100 characters long.");
                        throw new DirtyFormDataException();
                    }
                    return string.Format("{0}: {1}", category.Name, reason);
                }
                return string.Format("{0}: {1}", category.Name, subCategory.Name);
            }
            catch (DirtyFormDataException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                callback.ModelState.AddModelError("SelectedCategory", "No subcategory found. Select a valid category.");
                throw new DirtyFormDataException();
            }
        }


        /// <summary>
        /// Get the category with this category Id
        /// </summary>
        /// <param name="selectedCategory"></param>
        /// <returns>ExtractCategoryVM</returns>
        public ExtractCategoryVM GetCategory(int selectedCategory)
        {
            return _coreExtractCategory.Value.GetActiveSubCategories(selectedCategory);
        }

        /// <summary>
        /// Check if <paramref name="affivdavitNumber"/> does not exist with the user 
        /// with the <paramref name="taxEntityId"/>
        /// </summary>
        /// <param name="affivdavitNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public bool CheckIfExistingAffidavitNumber(string affivdavitNumber, long taxEntityId)
        {
            return _coreExtractDetails.Value.CheckIfExistingAffidavitNumber(affivdavitNumber, taxEntityId);
        }


        /// <summary>
        /// Gets active sub categories for specified extract categories
        /// </summary>
        /// <param name="selectedCategories"></param>
        /// <returns></returns>
        public IEnumerable<ExtractCategoryVM> GetExtractCategoriesList(IEnumerable<int> selectedCategories)
        {
            List<ExtractCategoryVM> extractCategories = new List<ExtractCategoryVM>();
            foreach (var selectedCategory in selectedCategories)
            {
                extractCategories.AddRange(_coreExtractCategory.Value.GetActiveSubCategoriesList(selectedCategory));
            }
            return extractCategories;
        }
    }
}