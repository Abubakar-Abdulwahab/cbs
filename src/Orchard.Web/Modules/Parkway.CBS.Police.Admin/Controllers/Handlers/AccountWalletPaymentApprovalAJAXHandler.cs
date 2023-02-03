using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class AccountWalletPaymentApprovalAJAXHandler : IAccountWalletPaymentApprovalAJAXHandler
    {
        private readonly ICorePSSAdminVerificationService _corePSSAdminVerificationService;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _adminUsersManager;
        ILogger Logger { get; set; }

        public AccountWalletPaymentApprovalAJAXHandler(ICorePSSAdminVerificationService corePSSAdminVerificationService, IPSSAdminUsersManager<PSSAdminUsers> adminUsersManager)
        {
            _corePSSAdminVerificationService = corePSSAdminVerificationService;
            _adminUsersManager = adminUsersManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get the verification token
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <param name="redirectObj">if a redirect is needed after verification provide this object</param>
        /// <returns>string</returns>
        public string ProvideVerificationToken(int userPartRecordId, VerificationType verificationType, RedirectReturnObject redirectObj = null)
        {
            Core.VM.PSSAdminUsersVM adminUser = _adminUsersManager.GetAdminUserWithUserPartRecordId(userPartRecordId);
            if (adminUser == null) { throw new Exception($"PSS Admin user with UserPartRecordId {userPartRecordId} not found."); }
            VerTokenResult queResult = _corePSSAdminVerificationService.QueueVerificationToken(adminUser, new AccountVerificationEmailNotificationModel { Sender = PSSPulseTemplateFileNames.Sender.GetDescription(), Subject = "Police Service Account Wallet Payment Verification", TemplateFileName = PSSPulseTemplateFileNames.AccountVerification.GetDescription() }, verificationType);
            VerTokenEncryptionObject enobj = new VerTokenEncryptionObject { VerId = queResult.VerObjId, PaddingText = PaddingForToken(), CreatedAt = queResult.CreatedAt, RedirectObj = redirectObj, VerificationType = (int)verificationType };
            return Util.LetsEncrypt(JsonConvert.SerializeObject(enobj));
        }


        /// <summary>
        /// Resend verification token
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NoRecordFoundException">when ver code not found</exception>
        /// <exception cref="InvalidOperationException">when resend count is maxed</exception>
        public void ResendVerificationCode(string token)
        {
            try
            {
                int resendLimit = 5;
                string sresendLimit = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.AccountWalletPaymentVerificationResendCodeLimit);
                if (!string.IsNullOrEmpty(sresendLimit)) { int.TryParse(sresendLimit, out resendLimit); }
                _corePSSAdminVerificationService.ResendCodeNotification(token, new AccountVerificationEmailNotificationModel { Sender = PSSPulseTemplateFileNames.Sender.GetDescription(), Subject = "Police Service Account Wallet Payment Verification", TemplateFileName = PSSPulseTemplateFileNames.AccountVerification.GetDescription() }, VerificationType.AccountWalletPayment, DateTime.Now.Date, DateTime.Now.Date.AddDays(1).AddMilliseconds(-1), resendLimit);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Do token validation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        public PSSAdminValidateVerReturnValue DoTokenValidation(string token, string userCodeInput)
        {
            return _corePSSAdminVerificationService.ValidateVerificationCode(token, userCodeInput, VerificationType.AccountWalletPayment);
        }


        /// <summary>
        /// Check if token for account wallet payment verification has expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        public bool TokenHasExpired(string token)
        {
            return _corePSSAdminVerificationService.CheckForTokenExpiry(token);
        }


        /// <summary>
        /// Apply padding to token
        /// </summary>
        /// <returns></returns>
        private string PaddingForToken()
        {
            byte[] byteBit1 = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] byteBit2 = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(byteBit1.Concat(byteBit2).ToArray());
        }
    }
}