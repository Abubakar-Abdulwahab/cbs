using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IRetrieveEmailHandler : IDependency
    {
        /// <summary>
        /// Validates specified phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="errors"></param>
        /// <returns>RegisterUserResponse</returns>
        RegisterUserResponse ValidatePhoneNumber(string phoneNumber, ref List<ErrorModel> errors);

        /// <summary>
        /// Checks if user with specified cbs user id has exceeded the resend limit for today
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        bool CheckIfCBSUserExceededResendCount(long cbsUserId);
    }
}
