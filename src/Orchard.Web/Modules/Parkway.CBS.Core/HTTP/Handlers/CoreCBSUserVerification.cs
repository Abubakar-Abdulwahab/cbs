using Orchard.Logging;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Mail.Provider.Contracts;
using Parkway.CBS.Core.Mail.Provider;
using System.Dynamic;
using System.Collections.Generic;
using Parkway.CBS.Core.StateConfig;
using Orchard;
using System.Linq;
using Parkway.CBS.Core.SMS.Provider.Contracts;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreCBSUserVerification : ICoreCBSUserVerification
    {
       
        private readonly Lazy<IVerificationCodeManager<VerificationCode>> _verRepo;
        private readonly Lazy<IVerificationCodeItemsManager<VerificationCodeItems>> _verItmesRepo;
        private readonly IEnumerable<Lazy<IEmailProvider>> _emailProvider;
        private readonly IEnumerable<Lazy<ISMSProvider>> _smsProvider;
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<ICBSUserManager<CBSUser>> _cbsUserManager;

        public ILogger Logger { get; set; }


        public CoreCBSUserVerification( Lazy<IVerificationCodeManager<VerificationCode>> verRepo, Lazy<IVerificationCodeItemsManager<VerificationCodeItems>> verItmesRepo, IEnumerable<Lazy<IEmailProvider>> emailProvider, IEnumerable<Lazy<ISMSProvider>> smsProvider, IOrchardServices orchardServices, Lazy<ICBSUserManager<CBSUser>> cbsUserManager)
        {
            Logger = NullLogger.Instance;
            _verRepo = verRepo;
            _verItmesRepo = verItmesRepo;
            _emailProvider = emailProvider;
            _smsProvider = smsProvider;
            _orchardServices = orchardServices;
            _cbsUserManager = cbsUserManager;
        }



        /// <summary>
        /// Queue up a verification token to be sent to the user
        /// for specified verification type
        /// </summary>
        /// <param name="cbsUserId">long</param>
        /// <param name="verificationModel"></param>
        /// <param name="verificationType"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public VerTokenResult QueueVerificationToken(long cbsUserId, AccountVerificationEmailNotificationModel verificationModel, VerificationType verificationType)
        {
            VerificationCode verObj = new VerificationCode { CBSUser = new CBSUser { Id = cbsUserId }, VerificationType = (int)verificationType };
            if (!_verRepo.Value.Save(verObj))
            {
                throw new CouldNotSaveRecord("Could not save verification code record");
            }
            return QueueANewResendToken(verObj, verificationModel);
        }


        /// <summary>
        /// Queue up a verification token to be sent to the user
        /// for specified verification type
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationModel">AccountVerificationEmailNotificationModel</param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public VerTokenResult QueueVerificationToken(CBSUserVM cbsUser, AccountVerificationEmailNotificationModel verificationModel, VerificationType verificationType)
        {
            VerificationCode verObj = new VerificationCode { CBSUser = new CBSUser { Id = cbsUser.Id, Name = cbsUser.Name, Email = cbsUser.Email, PhoneNumber = cbsUser.PhoneNumber, TaxEntity = new TaxEntity { Id = cbsUser.TaxEntity.Id } }, VerificationType = (int)verificationType };
            if (!_verRepo.Value.Save(verObj))
            {
                throw new CouldNotSaveRecord("Could not save verification code record");
            }
            return QueueANewResendToken(verObj, verificationModel);
        }


        /// <summary>
        /// Queue up a verification token to be sent to the user
        /// for specified verification type (SMS only)
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public VerTokenResult QueueVerificationToken(CBSUserVM cbsUser, VerificationType verificationType)
        {
            VerificationCode verObj = new VerificationCode { CBSUser = new CBSUser { Id = cbsUser.Id }, VerificationType = (int)verificationType };
            if (!_verRepo.Value.Save(verObj))
            {
                throw new CouldNotSaveRecord("Could not save verification code record");
            }
            return QueueANewResendToken(new VerificationCodeVM { Id = verObj.Id, CBSUserVM = cbsUser, CreatedAtUtc = verObj.CreatedAtUtc });
        }


        /// <summary>
        /// Resend a new verification code
        /// </summary>
        /// <param name="verCodeObj"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public VerTokenResult QueueANewResendToken(VerificationCode verCodeObj, AccountVerificationEmailNotificationModel verificationModel)
        {
            string code = GetVerificationCode();
            string hashValue = Util.HMACHash256(GetVerificationCodeConcatenate(verCodeObj.Id, verCodeObj.CBSUser.Id, verCodeObj.CreatedAtUtc, code), AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HashKey));
            //do items
            VerificationCodeItems verItem = new VerificationCodeItems { CodeHash = hashValue, VerificationCode = verCodeObj, State = (int)VerificationState.Unused, };
            if (!_verItmesRepo.Value.Save(verItem))
            {
                throw new CouldNotSaveRecord("Could not save verification item record");
            }

            //Do an email and sms notification
            DoNotifications(verCodeObj, verificationModel, code);
            
            return new VerTokenResult { VerObjId = verCodeObj.Id, CreatedAt = verCodeObj.CreatedAtUtc };
        }


        /// <summary>
        /// Resend a new verification code (SMS only)
        /// </summary>
        /// <param name="verCodeObj"></param>
        /// <exception cref="CouldNotSaveRecord"></exception>
        public VerTokenResult QueueANewResendToken(VerificationCodeVM verCodeObj)
        {
            string code = GetVerificationCode();
            string hashValue = Util.HMACHash256(GetVerificationCodeConcatenate(verCodeObj.Id, verCodeObj.CBSUserVM.Id, verCodeObj.CreatedAtUtc, code), AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HashKey));
            //do items
            VerificationCodeItems verItem = new VerificationCodeItems { CodeHash = hashValue, VerificationCode = new VerificationCode { Id = verCodeObj.Id }, State = (int)VerificationState.Unused, };
            if (!_verItmesRepo.Value.Save(verItem))
            {
                throw new CouldNotSaveRecord("Could not save verification item record");
            }

            //Do sms notification
            DoSMSNotification(verCodeObj, code);

            return new VerTokenResult { VerObjId = verCodeObj.Id, CreatedAt = verCodeObj.CreatedAtUtc };
        }



        /// <summary>
        /// Send email and SMS notifications
        /// </summary>
        /// <param name="verCodeObj"></param>
        /// <param name="code"></param>
        private void DoNotifications(VerificationCode verCodeObj, AccountVerificationEmailNotificationModel verificationModel, string code)
        {
            try
            {
                bool isSettingsEnabled;

                StateConfig.StateConfig stateConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node emailNode = stateConfig.Node.Where(x => x.Key == TenantConfigKeys.IsEmailEnabled.ToString()).FirstOrDefault();
                if (emailNode != null && !string.IsNullOrEmpty(emailNode.Value))
                {
                    bool.TryParse(emailNode.Value, out isSettingsEnabled);
                    if (isSettingsEnabled && !string.IsNullOrEmpty(verCodeObj.CBSUser.Email))
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
                                impl.Value.AccountVerification(verCodeObj, verificationModel, code);
                            }
                        }
                    }
                }

                Node smsNode = stateConfig.Node.Where(x => x.Key == TenantConfigKeys.IsSMSEnabled.ToString()).FirstOrDefault();
                if (smsNode != null && !string.IsNullOrEmpty(smsNode.Value))
                {
                    isSettingsEnabled = false;
                    bool.TryParse(smsNode.Value, out isSettingsEnabled);
                    if (isSettingsEnabled && !string.IsNullOrEmpty(verCodeObj.CBSUser.PhoneNumber))
                    {
                        int providerId = 0;
                        bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                        if (!result)
                        {
                            providerId = (int)SMSProvider.Pulse;
                        }
                        foreach (var impl in _smsProvider)
                        {
                            if ((SMSProvider)providerId == impl.Value.GetSMSNotificationProvider)
                            {
                                string message = $"Dear {verCodeObj.CBSUser.Name}, Please use the 6-digit code {code} to verify your account.";
                                impl.Value.SendSMS(new List<string> { verCodeObj.CBSUser.PhoneNumber }, message, _orchardServices.WorkContext.CurrentSite.SiteName);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
        }


        /// <summary>
        /// Sends email address as sms to specified user
        /// </summary>
        /// <param name="cbsUserId"></param>
        public void SendEmailAddressSMSToCBSUser(long cbsUserId)
        {
            try
            {
                CBSUserVM cbsUser = _cbsUserManager.Value.GetCBSUserWithId(cbsUserId);
                bool isSettingsEnabled;

                StateConfig.StateConfig stateConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);

                Node smsNode = stateConfig.Node.Where(x => x.Key == nameof(TenantConfigKeys.IsSMSEnabled)).FirstOrDefault();
                if (!string.IsNullOrEmpty(smsNode?.Value))
                {
                    isSettingsEnabled = false;
                    bool.TryParse(smsNode.Value, out isSettingsEnabled);
                    if (isSettingsEnabled && !string.IsNullOrEmpty(cbsUser.PhoneNumber))
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
                                impl.Value.SendSMS(new List<string> { cbsUser.PhoneNumber }, $"Dear {cbsUser.Name}, Your email address is {cbsUser.Email}.", _orchardServices.WorkContext.CurrentSite.SiteName);
                                break;
                            }
                        }
                    }
                }
                else { Logger.Error($"Could not get SMS enabled value from config"); }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Send SMS notification
        /// </summary>
        /// <param name="verCodeObj"></param>
        /// <param name="code"></param>
        private void DoSMSNotification(VerificationCodeVM verCodeObj, string code)
        {
            try
            {
                bool isSettingsEnabled;

                StateConfig.StateConfig stateConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);

                Node smsNode = stateConfig.Node.Where(x => x.Key == nameof(TenantConfigKeys.IsSMSEnabled)).FirstOrDefault();
                if (!string.IsNullOrEmpty(smsNode?.Value))
                {
                    isSettingsEnabled = false;
                    bool.TryParse(smsNode.Value, out isSettingsEnabled);
                    if (isSettingsEnabled && !string.IsNullOrEmpty(verCodeObj.CBSUserVM.PhoneNumber))
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
                                string message = $"Dear {verCodeObj.CBSUserVM.Name}, Please use the 6-digit code {code} to verify your account.";
                                impl.Value.SendSMS(new List<string> { verCodeObj.CBSUserVM.PhoneNumber }, message, _orchardServices.WorkContext.CurrentSite.SiteName);
                                break;
                            }
                        }
                    }
                }
                else { Logger.Error($"Could not get SMS enabled value from config"); throw new Exception(); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
        }



        /// <summary>
        /// Validate token against user input
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userCodeInput"></param>
        /// <exception cref="TimeoutException">Indicates whether the token has expired or the token state has already been used</exception>
        /// <exception cref="NoRecordFoundException">Indicates when no verification item has been found</exception>
        /// <exception cref="Exception">Handle general exception</exception>
        public ValidateVerReturnValue ValidateVerificationCode(string token, string userCodeInput, bool isPasswordReset = false)
        {
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(Util.LetsDecrypt(token));
            VerificationCode verCode = _verRepo.Value.Get(v => v.Id == enobj.VerId);
            if (HasExpired(verCode))
            {
                Logger.Error("Token has expired " + verCode.Id);
                throw new TimeoutException("Token has expired.");
            }

            if (verCode.CBSUser.Verified && !isPasswordReset) { throw new UserNotAuthorizedForThisActionException("User has already been verified. Token: " + token); }

            string hashValue = Util.HMACHash256(GetVerificationCodeConcatenate(verCode.Id, verCode.CBSUser.Id, verCode.CreatedAtUtc, userCodeInput), AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HashKey));
            //get verification item
            VerificationCodeItems verItem = _verItmesRepo.Value.Get(vi => ((vi.VerificationCode == verCode) && (vi.CodeHash == hashValue)));
            if (verItem == null)
            {
                Logger.Error("No record for found for ver item for ver code Id {0}" + verCode.Id);
                throw new NoRecordFoundException("Verification code not found");
            }
            //if a match was found, change the state to used
            if (verItem.State == (int)VerificationState.Used)
            {
                Logger.Error("Token has already been user " + verCode.Id);
                throw new Exception("Token has already been used");
            }
            verItem.State = (int)VerificationState.Used;
            verCode.CBSUser.Verified = true;

            return new ValidateVerReturnValue { RedirectObj = enobj.RedirectObj, CBSUserId = verCode.CBSUser.Id, IsAdministrator = verCode.CBSUser.IsAdministrator };
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
        public ValidateVerReturnValue ValidateVerificationCode(string token, string userCodeInput, VerificationType verificationType)
        {
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(Util.LetsDecrypt(token));
            if(enobj.VerificationType != (int)verificationType)
            {
                Logger.Error($"Verification type mismatch. Supplied verification type - {(VerificationType)enobj.VerificationType} Expected verification type - {verificationType}");
                throw new Exception($"Verification type mismatch. Supplied verification type - {(VerificationType)enobj.VerificationType} Expected verification type - {verificationType}");
            }

            VerificationCodeVM verCode = _verRepo.Value.GetVerificationCode(enobj.VerId, verificationType);
            //if this isn't for account verification or user has not been verified (only if this is for account verification), check if it's expired
            if (HasExpired(verCode))
            {
                Logger.Error("Token has expired " + verCode.Id);
                throw new TimeoutException("Token has expired.");
            }

            string hashValue = Util.HMACHash256(GetVerificationCodeConcatenate(verCode.Id, verCode.CBSUserVM.Id, verCode.CreatedAtUtc, userCodeInput), AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.HashKey));

            //check if verification code item exists
            if (_verItmesRepo.Value.Count(vi => (vi.VerificationCode.Id == verCode.Id) && (vi.CodeHash == hashValue)) == 0)
            {
                Logger.Error("No record for found for ver item for ver code Id {0}" + verCode.Id);
                throw new NoRecordFoundException("Verification code not found");
            }

            //if it's not expired, check if user already verified if this is for account verification
            if (verCode.CBSUserVM.Verified && (verificationType == VerificationType.AccountVerification)) { throw new UserNotAuthorizedForThisActionException("User has already been verified. Token: " + token); }

            //if it's not expired, change the state to used
            if (_verItmesRepo.Value.Count(vi => (vi.VerificationCode.Id == verCode.Id) && (vi.CodeHash == hashValue) && (vi.State == (int)VerificationState.Used)) > 0)
            {
                Logger.Error("Token has already been user " + verCode.Id);
                throw new Exception("Token has already been used");
            }
            _verItmesRepo.Value.UpdateVerificationItemState(verCode.Id, VerificationState.Used);

            if(verificationType == VerificationType.AccountVerification)
            {
                _cbsUserManager.Value.UpdateCBSUserVerifiedState(verCode.CBSUserVM.Id, true);
            }

            return new ValidateVerReturnValue { RedirectObj = enobj.RedirectObj, CBSUserId = verCode.CBSUserVM.Id, IsAdministrator = verCode.CBSUserVM.IsAdministrator };
        }



        /// <summary>
        /// Get CBS user attached to this verification code
        /// </summary>
        /// <param name="verId"></param>
        /// <returns>long</returns>
        public long GetCBSUserId(long verId)
        {
            VerificationCode verCode = _verRepo.Value.Get(v => v.Id == verId);
            if (verCode == null) { return 0; }
            return verCode.CBSUser.Id;
        }


        /// <summary>
        /// Get the verification code db object with this Id
        /// </summary>
        /// <param name="verId"></param>
        /// <returns>VerificationCode</returns>
        public VerificationCode GetVerificationObject(long verId)
        {
            return _verRepo.Value.Get(v => v.Id == verId);
        }


        /// <summary>
        /// Resend code
        /// </summary>
        /// <param name="token"></param>
        public void ResendCodeNotification(string token, AccountVerificationEmailNotificationModel verificationModel)
        {
            string decpToken = Util.LetsDecrypt(token);
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(decpToken);
            VerificationCode code = GetVerificationObject(enobj.VerId);
            if (code == null) { throw new NoRecordFoundException("No record for verification code found " + enobj.VerId); }
            //check for resend count
            int resendLimit = 5;
            string sresendLimit = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.VerificationResendCodeLimit);
            if (!string.IsNullOrEmpty(sresendLimit)) { int.TryParse(sresendLimit, out resendLimit); }
            if (code.ResendCount == resendLimit)
            {
                Logger.Error(string.Format(ErrorLang.exceededcoderetry().ToString() + " Token {0}, Verification Id {1}", token, enobj.VerId));
                throw new InvalidOperationException(ErrorLang.exceededcoderetry().ToString());
            }
            //if resend cound is still within thres
            code.ResendCount += 1;
            //resend a new token
            QueueANewResendToken(code, verificationModel);
        }


        /// <summary>
        /// Resends code for specified verification type provided the max number of resend count has not yet been exceed within the period defined by the from and to parameters
        /// </summary>
        /// <param name="token"></param>
        /// <param name="verificationType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="resendLimit"></param>
        /// <exception cref="NoRecordFoundException">throws this when no record is found</exception>
        /// <exception cref="InvalidOperationException">throws this when the number of resends has exceeded limit</exception>
        public void ResendCodeNotification(string token, VerificationType verificationType, DateTime from, DateTime to, int resendLimit)
        {
            string decpToken = Util.LetsDecrypt(token);
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(decpToken);
            VerificationCodeVM code = _verRepo.Value.GetVerificationCode(enobj.VerId);
            if (code == null) { throw new NoRecordFoundException("No record for verification code found " + enobj.VerId); }
            //check for resend count
            if (_verRepo.Value.GetResendCountForTimePeriod(code.CBSUserVM.Id, verificationType, from, to) >= resendLimit)
            {
                Logger.Error(string.Format(ErrorLang.exceededcoderetry().Text + " Token {0}, Verification Id {1}", token, enobj.VerId));
                throw new InvalidOperationException(ErrorLang.exceededcoderetry().Text);
            }
            //if resend count is still within limit
            code.ResendCount += 1;
            _verRepo.Value.UpdateVerificationCodeResendCount(code.Id, code.ResendCount);
            //resend a new token
            QueueANewResendToken(code);
        }



        /// <summary>
        /// Check if token has expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException">If the cbs user has already been verified, this exception will be thrown</exception>
        public bool CheckForTokenExpiry(string token, bool isPasswordReset = false)
        {
            string decpToken = Util.LetsDecrypt(token);
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(decpToken);
            VerificationCode verCode = _verRepo.Value.Get(v => v.Id == enobj.VerId);
            //check if user has been verified
            //And if this is not a reset password request
            if (verCode.CBSUser.Verified && !isPasswordReset) { throw new UserNotAuthorizedForThisActionException("The given user has already been verified"); }
            return HasExpired(verCode);            
        }


        /// <summary>
        /// Check if token has expired
        /// </summary>
        /// <param name="token"></param>
        /// <param name="verificationType"></param>
        /// <returns>bool</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException">If the cbs user has already been verified and this is an account verification request, this exception will be thrown</exception>
        public bool CheckForTokenExpiry(string token, VerificationType verificationType)
        {
            VerTokenEncryptionObject enobj = JsonConvert.DeserializeObject<VerTokenEncryptionObject>(Util.LetsDecrypt(token));
            VerificationCodeVM verCode = _verRepo.Value.GetVerificationCode(enobj.VerId, verificationType);
            //check if user has been verified
            //And if this is account verification request
            if (verCode.CBSUserVM.Verified && (verificationType == VerificationType.AccountVerification)) { throw new UserNotAuthorizedForThisActionException("The given user has already been verified"); }
            return HasExpired(verCode);
        }


        /// <summary>
        /// Get model user details model for this verification Id
        /// </summary>
        /// <param name="verId"></param>
        /// <returns>ModelForTokenRegeneration</returns>
        public ModelForTokenRegeneration GetCBSUserDetailsByVerificationCodeId(long verId)
        {
            return _verRepo.Value.GetCBSUserDetailsWithVerificationId(verId);
        }


        /// <summary>
        /// Check if verification has expired
        /// </summary>
        /// <param name="verCode"></param>
        /// <returns></returns>
        private bool HasExpired(VerificationCode verCode)
        {
            if (verCode.CreatedAtUtc.AddMinutes(GetCodeValidityTimeLimitInMinutes()) < DateTime.Now.ToLocalTime())
                return true;
            return false;
        }


        /// <summary>
        /// Check if verification has expired
        /// </summary>
        /// <param name="verCode"></param>
        /// <returns></returns>
        private bool HasExpired(VerificationCodeVM verCode)
        {
            if (verCode.CreatedAtUtc.AddMinutes(GetCodeValidityTimeLimitInMinutes()) < DateTime.Now.ToLocalTime())
                return true;
            return false;
        }

        private static int GetCodeValidityTimeLimitInMinutes()
        {
            int codeExpiryMinutes = 20;
            string scodeExpiryMinutes = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.VerificationCodeExpiryInMinutes);
            if (!string.IsNullOrEmpty(scodeExpiryMinutes))
            { int.TryParse(scodeExpiryMinutes, out codeExpiryMinutes); }
            return codeExpiryMinutes;
        }


        /// <summary>
        /// Get verification code 4 digits
        /// </summary>
        /// <param name="idValue"></param>
        /// <returns>string</returns>
        private string GetVerificationCode()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[8];
                rng.GetBytes(data);

                int generatedValue = Math.Abs(BitConverter.ToInt32(data, startIndex: 0));
                string str = Convert.ToBase64String(data);
                return generatedValue.ToString().Substring(0, 6);
            }
        }


        private string GetVerificationCodeConcatenate(Int64 verificationCodeId, Int64 profileId, DateTime createdAtDate, string code)
        {
            return string.Format("{0}{1}{2}{3}", verificationCodeId, profileId, createdAtDate, code);
        }
    }
}