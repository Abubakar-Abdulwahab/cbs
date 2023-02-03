using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ICBSUserManager<CBSUser> : IDependency, IBaseManager<CBSUser>
    {
        /// <summary>
        /// Get the user details for the userPartRecordId
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns>UserDetailsModel | null</returns>
        UserDetailsModel GetUserDetails(int userPartRecordId);
        

        /// <summary>
        /// Get CBS user and tax entity details
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns>UserDetailsModel</returns>
        UserDetailsModel GetCBSUserAndTaxEntity(long profileId);


        /// <summary>
        /// Get user details for account verification
        /// </summary>
        /// <param name="userPartId"></param>
        /// <returns>UserDetailsModel</returns>
        UserDetailsModel GetUserDetailsForAccountVerification(int userPartId);


        /// <summary>
        /// Get the user details for the cbs User Id
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns>UserDetailsModel | null</returns>
        UserDetailsModel GetUserDetailsForCBSUserId(long cbsUserId);


        /// <summary>
        /// Get the register user model for this email
        /// <para></para>
        /// </summary>
        /// <param name="email"></param>
        /// <returns>RegisterUserResponse</returns>
        RegisterUserResponse GetRegisterUserResponse(string email);


        /// <summary>
        /// Get the register user model for this cbs user email
        /// <para>This method return only part properties</para>
        /// </summary>
        /// <param name="email"></param>
        /// <returns>RegisterUserResponse</returns>
        RegisterUserResponse GetRegisterCBSUserResponse(string email);


        /// <summary>
        /// Get the register user model for this cbs user phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        RegisterUserResponse GetRegisterCBSUserResponseWithPhoneNumber(string phoneNumber);


        /// <summary>
        /// Updates verified state for cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <param name="isVerified"></param>
        void UpdateCBSUserVerifiedState(long cbsUserId, bool isVerified);


        /// <summary>
        /// Get cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        CBSUserVM GetCBSUserWithId(long cbsUserId);


        /// <summary>
        /// Gets tax entity id for cbs user with specified user part record id
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns></returns>
        long GetTaxEntityIdForAdminCBSUserWithUserPartRecord(int userPartRecordId);


        /// <summary>
        /// Toggles is active value for cbs user with specified user part record id
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="isActive"></param>
        void ToggleIsActiveForCBSUserWithUserId(int userPartRecordId, bool isActive);

        /// <summary>
        /// Gets admin cbs user with specified payer id
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns></returns>
        CBSUserVM GetAdminCBSUserWithPayerId(string payerId);


        /// <summary>
        /// Gets admin cbs user with specified tax entity id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        CBSUserVM GetAdminCBSUserWithTaxEntityId(long taxEntityId);

        /// <summary>
        /// Gets user part record id for admin cbs user with specified tax entity id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        int GetUserPartRecordIdForAdminCBSUserWithTaxEntityId(long taxEntityId);
    }
}