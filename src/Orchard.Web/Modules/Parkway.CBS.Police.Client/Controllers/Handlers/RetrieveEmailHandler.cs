using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class RetrieveEmailHandler : IRetrieveEmailHandler
    {
        private readonly ICBSUserManager<CBSUser> _iCBSUserManager;
        private readonly IVerificationCodeManager<VerificationCode> _verRepo;
        ILogger Logger { get; set; }
        public RetrieveEmailHandler(ICBSUserManager<CBSUser> iCBSUserManager, IVerificationCodeManager<VerificationCode> verRepo)
        {
            _verRepo = verRepo;
            _iCBSUserManager = iCBSUserManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Validates specified phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="errors"></param>
        /// <returns>RegisterUserResponse</returns>
        public RegisterUserResponse ValidatePhoneNumber(string phoneNumber, ref List<ErrorModel> errors)
        {
            try
            {
                if (!Util.DoPhoneNumberValidation(phoneNumber))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Enter a valid phone number.", FieldName = nameof(RetrieveEmailVM.PhoneNumber) });
                    return null;
                }

                return _iCBSUserManager.GetRegisterCBSUserResponseWithPhoneNumber(Util.NormalizePhoneNumber(phoneNumber));
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Checks if user with specified cbs user id has exceeded the resend limit for today
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        public bool CheckIfCBSUserExceededResendCount(long cbsUserId)
        {
            try
            {
                //check for resend count
                int resendLimit = 5;
                string sresendLimit = AppSettingsConfigurations.GetSettingsValue(CBS.Core.Models.Enums.AppSettingEnum.RetrieveEmailVerificationResendCodeLimit);
                if (!string.IsNullOrEmpty(sresendLimit)) { int.TryParse(sresendLimit, out resendLimit); }
                return _verRepo.GetResendCountForTimePeriod(cbsUserId, CBS.Core.Models.Enums.VerificationType.RetrieveEmail, DateTime.Now.Date, DateTime.Now.Date.AddDays(1).AddMilliseconds(-1)) >= resendLimit;
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}