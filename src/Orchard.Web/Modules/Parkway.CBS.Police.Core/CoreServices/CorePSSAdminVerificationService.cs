using Orchard.Logging;
using System;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Newtonsoft.Json;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Mail.Provider.Contracts;
using System.Collections.Generic;
using Parkway.CBS.Core.StateConfig;
using Orchard;
using System.Linq;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Core.HTTP.Handlers;
using Parkway.CBS.Police.Core.HelperModels;


namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSSAdminVerificationService : CoreUserVerification, ICorePSSAdminVerificationService
    {
        private readonly Lazy<IPSSAdminVerificationCodeManager<PSSAdminVerificationCode>> _verRepo;
        private readonly Lazy<IPSSAdminVerificationCodeItemsManager<PSSAdminVerificationCodeItems>> _verItemsRepo;
        private readonly IEnumerable<Lazy<IEmailProvider>> _emailProvider;
        private readonly IEnumerable<Lazy<ISMSProvider>> _smsProvider;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }

        public CorePSSAdminVerificationService(Lazy<IPSSAdminVerificationCodeManager<PSSAdminVerificationCode>> verRepo, Lazy<IPSSAdminVerificationCodeItemsManager<PSSAdminVerificationCodeItems>> verItemsRepo, IEnumerable<Lazy<IEmailProvider>> emailProvider, IEnumerable<Lazy<ISMSProvider>> smsProvider, IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _verRepo = verRepo;
            _verItemsRepo = verItemsRepo;
            _emailProvider = emailProvider;
            _smsProvider = smsProvider;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Queue up a verification token to be sent to the user
        /// for specified verification type
        /// </summary>
        /// <param name="user"></param>
        /// <param name="verificationModel">AccountVerificationEmailNotificationModel</param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public VerTokenResult QueueVerificationToken(PSSAdminUsersVM user, AccountVerificationEmailNotificationModel verificationModel, VerificationType verificationType)
        {
            PSSAdminVerificationCode verObj = new PSSAdminVerificationCode { AdminUser = new PSSAdminUsers { Id = user.Id, Fullname = user.Fullname, Email = user.Email, PhoneNumber = user.PhoneNumber }, VerificationType = (int)verificationType };
            if (!_verRepo.Value.Save(verObj))
            {
                throw new CouldNotSaveRecord("Could not save verification code record");
            }
            return QueueANewResendToken(verObj.ToVM(), verificationModel);
        }


        /// <summary>
        /// Resend a new verification code
        /// </summary>
        /// <param name="verCodeObj"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public VerTokenResult QueueANewResendToken(PSSAdminVerificationCodeVM verCodeObj, AccountVerificationEmailNotificationModel verificationModel)
        {
            string code = GetVerificationCode();
            string hashValue = Util.HMACHash256(GetVerificationCodeConcatenate(verCodeObj.Id, verCodeObj.AdminUser.Id, verCodeObj.CreatedAtUtc, code), AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HashKey));
            //do items
            PSSAdminVerificationCodeItems verItem = new PSSAdminVerificationCodeItems { CodeHash = hashValue, VerificationCode = new PSSAdminVerificationCode { Id = verCodeObj.Id }, State = (int)VerificationState.Unused, };
            if (!_verItemsRepo.Value.Save(verItem))
            {
                throw new CouldNotSaveRecord("Could not save PSS Admin verification item record");
            }

            //Do an email and sms notification
            DoNotifications(verCodeObj, verificationModel, code);

            return new VerTokenResult { VerObjId = verCodeObj.Id, CreatedAt = verCodeObj.CreatedAtUtc };
        }


        /// <summary>
        /// Send email and SMS notifications
        /// </summary>
        /// <param name="verCodeObj"></param>
        /// <param name="code"></param>
        private void DoNotifications(PSSAdminVerificationCodeVM verCodeObj, AccountVerificationEmailNotificationModel verificationModel, string code)
        {
            try
            {
                bool isSettingsEnabled;

                StateConfig stateConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node emailNode = stateConfig.Node.Where(x => x.Key == nameof(TenantConfigKeys.IsEmailEnabled)).SingleOrDefault();
                if (emailNode != null && !string.IsNullOrEmpty(emailNode.Value))
                {
                    bool.TryParse(emailNode.Value, out isSettingsEnabled);
                    if (isSettingsEnabled && !string.IsNullOrEmpty(verCodeObj.AdminUser.Email))
                    {
                        bool result = int.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.EmailProvider), out int providerId);
                        if (!result)
                        {
                            providerId = (int)EmailProvider.Gmail;
                        }
                        foreach (var impl in _emailProvider)
                        {
                            if ((EmailProvider)providerId == impl.Value.GetEmailNotificationProvider)
                            {
                                impl.Value.SendEmail(new EmailNotificationModel { Sender = verificationModel.Sender, CBSUser = new CBSUserVM { Email = verCodeObj.AdminUser.Email }, Subject = verificationModel.Subject, TemplateFileName = verificationModel.TemplateFileName, Params = new Dictionary<string, string> { { "Name", verCodeObj.AdminUser.Fullname }, { "Code", code } } });
                            }
                        }
                    }
                }

                Node smsNode = stateConfig.Node.Where(x => x.Key == nameof(TenantConfigKeys.IsSMSEnabled)).SingleOrDefault();
                if (smsNode != null && !string.IsNullOrEmpty(smsNode.Value))
                {
                    isSettingsEnabled = false;
                    bool.TryParse(smsNode.Value, out isSettingsEnabled);
                    if (isSettingsEnabled && !string.IsNullOrEmpty(verCodeObj.AdminUser.PhoneNumber))
                    {
                        int providerId = 0;
                        bool result = int.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                        if (!result)
                        {
                            providerId = (int)SMSProvider.Pulse;
                        }
                        foreach (var impl in _smsProvider)
                        {
                            if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                            {
                                string message = $"Dear {verCodeObj.AdminUser.Fullname}, Please use the 6-digit code {code} to verify your account.";
                                impl.Value.SendSMS(new List<string> { verCodeObj.AdminUser.PhoneNumber }, message, _orchardServices.WorkContext.CurrentSite.SiteName);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Unable to send sms and email notifications for admin account verification. Exception message - {exception.Message}");
            }
        }


        /// <summary>
        /// Resends code for specified verification type provided the max number of resend count has not yet been exceed within the period defined by the from and to parameters
        /// </summary>
        /// <param name="token"></param>
        /// <param name="verificationModel"></param>
        /// <param name="verificationType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="resendLimit"></param>
        /// <exception cref="NoRecordFoundException">throws this when no record is found</exception>
        /// <exception cref="InvalidOperationException">throws this when the number of resends has exceeded limit</exception>
        public void ResendCodeNotification(string token, AccountVerificationEmailNotificationModel verificationModel, VerificationType verificationType, DateTime from, DateTime to, int resendLimit)
        {
            string decpToken = Util.LetsDecrypt(token);
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(decpToken);
            PSSAdminVerificationCodeVM code = _verRepo.Value.GetVerificationCode(enobj.VerId);
            if (code == null) { throw new NoRecordFoundException("No record for verification code found " + enobj.VerId); }
            //check for resend count
            if (_verRepo.Value.GetResendCountForTimePeriod(code.AdminUser.Id, verificationType, from, to) >= resendLimit)
            {
                Logger.Error(string.Format(ErrorLang.exceededcoderetry().Text + " Token {0}, Verification Id {1}", token, enobj.VerId));
                throw new InvalidOperationException(ErrorLang.exceededcoderetry().Text);
            }
            //if resend count is still within limit
            code.ResendCount += 1;
            _verRepo.Value.UpdateVerificationCodeResendCount(code.Id, code.ResendCount);
            //resend a new token
            QueueANewResendToken(code, verificationModel);
        }


        /// <summary>
        /// Validate token against user input for specified verification type
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <param name="verificationType"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        public PSSAdminValidateVerReturnValue ValidateVerificationCode(string token, string userCodeInput, VerificationType verificationType)
        {
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(Util.LetsDecrypt(token));
            if (enobj.VerificationType != (int)verificationType)
            {
                Logger.Error($"Verification type mismatch. Supplied verification type - {(VerificationType)enobj.VerificationType} Expected verification type - {verificationType}");
                throw new Exception($"Verification type mismatch. Supplied verification type - {(VerificationType)enobj.VerificationType} Expected verification type - {verificationType}");
            }

            PSSAdminVerificationCodeVM verCode = _verRepo.Value.GetVerificationCode(enobj.VerId);
            //if this isn't for account verification or user has not been verified (only if this is for account verification), check if it's expired
            if (HasExpired(verCode))
            {
                Logger.Error("Token has expired " + verCode.Id);
                throw new TimeoutException("Token has expired.");
            }

            string hashValue = Util.HMACHash256(GetVerificationCodeConcatenate(verCode.Id, verCode.AdminUser.Id, verCode.CreatedAtUtc, userCodeInput), AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HashKey));

            //check if verification code item exists
            if (_verItemsRepo.Value.Count(vi => (vi.VerificationCode.Id == verCode.Id) && (vi.CodeHash == hashValue)) == 0)
            {
                Logger.Error("No record for found for ver item for ver code Id {0}" + verCode.Id);
                throw new NoRecordFoundException("Verification code not found");
            }

            //if it's not expired, change the state to used
            if (_verItemsRepo.Value.Count(vi => (vi.VerificationCode.Id == verCode.Id) && (vi.CodeHash == hashValue) && (vi.State == (int)VerificationState.Used)) > 0)
            {
                Logger.Error("Token has already been user " + verCode.Id);
                throw new Exception("Token has already been used");
            }

            _verItemsRepo.Value.UpdateVerificationItemState(verCode.Id, VerificationState.Used);

            return new PSSAdminValidateVerReturnValue { RedirectObj = enobj.RedirectObj, PSSAdminUserId = verCode.AdminUser.Id };
        }


        /// <summary>
        /// Check if token has expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        public bool CheckForTokenExpiry(string token)
        {
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(Util.LetsDecrypt(token));
            PSSAdminVerificationCodeVM verCode = _verRepo.Value.GetVerificationCode(enobj.VerId);
            return HasExpired(verCode);
        }
    }
}