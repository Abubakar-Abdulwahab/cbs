using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class ForgotPasswordHandler : IForgotPasswordHandler
    {
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        public ILogger Logger { get; set; }
        private readonly Lazy<ICoreUserService> _coreUserService;

        public ForgotPasswordHandler(ICBSUserManager<CBSUser> cbsUserService, Lazy<ICoreUserService> coreUserService)
        {
            _cbsUserService = cbsUserService;
            Logger = NullLogger.Instance;
            _coreUserService = coreUserService;
        }


        /// <summary>
        /// Get user response for tax entity with email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        /// <param name="fieldName"></param>
        /// <returns>RegisterUserResponse</returns>
        public RegisterUserResponse GetRegisteredUserResponseObjByEmail(string email, ref List<ErrorModel> errors, string fieldName)
        {
            try
            {
                RegisterUserResponse response = _cbsUserService.GetRegisterCBSUserResponse(email);
                if (response == null)
                {
                    errors.Add(new ErrorModel { FieldName = (!string.IsNullOrEmpty(fieldName)) ? fieldName : "Email", ErrorMessage = "Unable to find user data" });
                    throw new DirtyFormDataException("Unable to find user data");
                }
                return response;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }



        /// <summary>
        /// Validate Email address and check if it exists
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        /// <param name="fieldname"></param>
        public void ValidateEmail(string email, ref List<ErrorModel> errors, string fieldname)
        {
            _coreUserService.Value.DoEmailValidation(email, ref errors, (!String.IsNullOrEmpty(fieldname)) ? fieldname : "Email", true);
            if (errors.Count > 0) { throw new DirtyFormDataException(); }
        }

    }
}