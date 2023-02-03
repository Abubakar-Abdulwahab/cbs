using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IHandler : IDependency
    {
        /// <summary>
        /// Get user details for this user part record Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserDetailsModel GetUserDetails(int id);


        /// <summary>
        /// Checks if user with specified user part record id is an administrator
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns></returns>
        bool CheckIfUserIsAdministrator(int userPartRecordId);


        /// <summary>
        /// Get user details for this cbs user Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserDetailsModel</returns>
        UserDetailsModel GetUserDetailsForCBSUserId(long cbsUserId);


        /// <summary>
        /// Get the verification token
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <param name="redirectObj">if a redirect is needed after verification provide this object</param>
        /// <returns>string</returns>
        string ProviderVerificationToken(CBSUserVM cbsUser, CBS.Core.Models.Enums.VerificationType verificationType, RedirectReturnObject redirectObj = null);


        /// <summary>
        /// Get the verification token (SMS only)
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <param name="redirectObj">if a redirect is needed after verification provide this object</param>
        /// <returns>string</returns>
        string ProviderVerificationTokenSMS(CBSUserVM cbsUser, CBS.Core.Models.Enums.VerificationType verificationType, RedirectReturnObject redirectObj = null);


        /// <summary>
        /// Resend verification token
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NoRecordFoundException">when ver code not found</exception>
        /// <exception cref="InvalidOperationException">when resend count is maxed</exception>
        void ResendVerificationCode(string token);


        /// <summary>
        /// Check if this user has been verified
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserDetailsModel</returns>
        UserDetailsModel CheckIfUserIsVerified(int userPartId);


        /// <summary>
        /// Get escort bill estimate using number of officers requested, duration and the rank amount rate for the lowest rank officer
        /// </summary>
        /// <param name="numberOfOfficers"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <param name="subSubTaxCategoryId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetEscortBillEstimate(int numberOfOfficers, string startDate, string endDate, int stateId, int lgaId, int subSubTaxCategoryId);


        /// <summary>
        /// get invoice URL
        /// </summary>
        /// <param name="bIN"></param>
        /// <returns></returns>
        string GetInvoiceURL(string bin);

    }
}
