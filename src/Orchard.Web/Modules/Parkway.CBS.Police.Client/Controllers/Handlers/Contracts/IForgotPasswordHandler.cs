using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IForgotPasswordHandler : IDependency
    {

        /// <summary>
        /// Fetch RegisterUserResponse Obj for registered user with the specified email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>RegisterUserResponse</returns>
        RegisterUserResponse GetRegisteredUserResponseObjByEmail(string email, ref List<ErrorModel> errors, string fieldName);


        /// <summary>
        /// Validate Email address and check if it exists
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        /// <param name="fieldname"></param>
        void ValidateEmail(string email, ref List<ErrorModel> errors, string fieldname);

    }
}
