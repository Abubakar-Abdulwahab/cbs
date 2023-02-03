using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSExtractHandler : IDependency
    {

        /// <summary>
        /// Get VM for police extract
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>ExtractRequestVM</returns>
        ExtractRequestVM GetVMForPoliceExtract(int serviceId);

        /// <summary>
        /// Get the list of commands for this lgaId
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        List<CommandVM> GetListOfCommands(int lgaId);


        /// <summary>
        /// Get the list of commands for this stateId
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        List<CommandVM> GetListOfCommandsByStateId(int stateId);

        /// <summary>
        /// Get next action direction for extract
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        dynamic GetNextDirectionForConfirmation();


        /// <summary>
        /// Validate  and get the command details
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="selectedStateLGA"></param>
        /// <param name="selectedCommand"></param>
        /// <returns>CommandVM</returns>
        CommandVM ValidateSelectedCommand(PSSExtractController callback, int selectedState, int selectedStateLGA, int selectedCommand);

        /// <summary>
        /// Validates selected extract categories and sub categories
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="userInput"></param>
        /// <param name="reason"></param>
        void ValidateExtractCategoriesAndSubCategories(PSSExtractController callback, ExtractRequestVM userInput, string reason);


        /// <summary>
        /// Do validation for extract category
        /// </summary>
        /// <param name="selectedCategory"></param>
        /// <param name="selectedSubCategory"></param>
        /// <param name="reason"></param>
        /// <returns>string</returns>
        string ValidateExtractCategory(PSSExtractController callback, int selectedCategory, int selectedSubCategory, string reason);


        /// <summary>
        /// Get the category with this category Id
        /// </summary>
        /// <param name="selectedCategory"></param>
        /// <returns>ExtractCategoryVM</returns>
        ExtractCategoryVM GetCategory(int selectedCategory);


        /// <summary>
        /// Gets active sub categories for specified extract categories
        /// </summary>
        /// <param name="selectedCategories"></param>
        /// <returns></returns>
        IEnumerable<ExtractCategoryVM> GetExtractCategoriesList(IEnumerable<int> selectedCategories);

        /// <summary>
        /// Check if <paramref name="affivdavitNumber"/> does not exist with the user 
        /// with the <paramref name="taxEntityId"/>
        /// </summary>
        /// <param name="affivdavitNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        bool CheckIfExistingAffidavitNumber(string affivdavitNumber, long taxEntityId);
    }
}
