using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IRegisterUserHandler : IDependency
    {

        /// <summary>
        /// Return view for User sign up page
        /// </summary>
        /// <returns>RegisterPSSUserObj</returns>
        RegisterPSSUserObj GetViewModelForUserSignup();


        /// <summary>
        /// Do validation for model
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <param name="identificationFile"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <returns>RegisterUserResponse</returns>
        RegisterUserResponse RegisterCBSUserModel(ref List<ErrorModel> errors, RegisterPSSUserObj userInput, HttpPostedFileBase identificationFile);

        /// <summary>
        /// Gets categories
        /// </summary>
        /// <returns></returns>
        IEnumerable<TaxEntityCategoryVM> GetCategories();

        /// <summary>
        /// Gets active tax category permissions
        /// </summary>
        /// <returns></returns>
        IEnumerable<TaxCategoryTaxCategoryPermissionsVM> GetTaxCategoryPermissions();


        /// <summary>
        /// Gets identification types available for tax category with specified id.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        IEnumerable<Core.VM.IdentificationTypeVM> GetIdentificationTypesForCategory(int categoryId);


        /// <summary>
        /// Validates contact entity validation model
        /// </summary>
        /// <param name="contactInfo"></param>
        /// <param name="errors"></param>
        void ValidateContactEntityInfo(ContactEntityValidationModel contactInfo, List<ErrorModel> errors);
    }
}
