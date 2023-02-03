using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreUserService : IDependency
    {

        /// <summary>
        /// Create a front end user on central billing
        /// <para>
        /// This would create a CBSUser, tax profile and activate the tax profile account. 
        /// Safe from null checks. Would return a valid object, or throw an exception
        /// </para>
        /// </summary>
        /// <param name="registerCBSUserModel"></param>
        /// <param name="category"></param>
        /// <param name="errors"></param>
        /// <param name="expertSystem"></param>
        /// <param name="requestRef"></param>
        /// <returns>RegisterUserResponse</returns>
        /// <exception cref="CBSUserNotFoundException">for api call when </exception>
        /// <exception cref="CannotSaveTaxEntityException">for when the tax profile could not be saved</exception>
        RegisterUserResponse TryCreateCBSUser(RegisterCBSUserModel registerCBSUserModel, TaxEntityCategory category, ref List<ErrorModel> errors, ExpertSystemSettings expertSystem = null, string requestRef = null, string fieldPrefix = null, bool validateEmail = false, bool validatePhoneNumber = false);


        /// <summary>
        /// Creates a front end user on central billing
        /// </summary>
        /// <para>
        /// This would create a CBSUser linked to the tax profile account with the specified tax entity id.
        /// Safe from null checks. Would return a valid object, or throw an exception
        /// </para>
        /// <param name="validatedRegisterCBSUserModel"></param>
        /// <param name="taxEntity"></param>
        /// <param name="category"></param>
        /// <param name="errors"></param>
        /// <param name="expertSystem"></param>
        /// <param name="requestRef"></param>
        /// <param name="fieldPrefix"></param>
        /// <param name="validateEmail"></param>
        /// <param name="validatePhoneNumber"></param>
        /// <returns></returns>
        RegisterUserResponse TryCreateCBSSubUser(RegisterCBSUserModel validatedRegisterCBSUserModel, TaxEntity taxEntity, TaxEntityCategory category, ref List<ErrorModel> errors, ExpertSystemSettings expertSystem = null, string requestRef = null, string fieldPrefix = null, bool validateEmail = false, bool validatePhoneNumber = false);


        /// <summary>
        /// Get tax entity by id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns>TaxEntity</returns>
        /// <exception cref="NoRecordFoundException">If entity record 404 </exception>
        TaxEntity GetTaxEntityById(long taxEntityId);

        /// <summary>
        /// Validate Email
        /// </summary>
        /// <param name="email">email address to validate</param>
        /// <param name="errors">errors detected</param>
        /// <param name="fieldName">name of field containing email address</param>
        /// <param name="compulsory">Email will be validated if set to true else it will not</param>
        void DoEmailValidation(string email, ref List<ErrorModel> errors, string fieldName, bool compulsory);

        /// <summary>
        /// Validate Phone Number
        /// </summary>
        /// <param name="sPhoneNumber">phone number to validate</param>
        /// <param name="errors">errors detected</param>
        /// <param name="fieldName">name of field containing phone number</param>
        /// <param name="compulsory">Phone number will be validated if set to true else it will not</param>
        void DoPhoneNumberValidation(string sPhoneNumber, ref List<ErrorModel> errors, string fieldName, bool compulsory);


        /// <summary>
        /// Check the count for email
        /// <para>throw error iff email count != 1</para>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        /// <param name="fieldName"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        void CheckIfEmailExists(string email, ref List<ErrorModel> errors, string fieldName);

    }
}
