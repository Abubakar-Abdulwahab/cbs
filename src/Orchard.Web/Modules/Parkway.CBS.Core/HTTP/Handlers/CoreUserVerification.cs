using System;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using System.Security.Cryptography;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public abstract class CoreUserVerification
    {
        /// <summary>
        /// Check if verification has expired
        /// </summary>
        /// <param name="verCode"></param>
        /// <returns></returns>
        protected bool HasExpired(BaseVerificationCodeVM verCode)
        {
            if (verCode.CreatedAtUtc.AddMinutes(GetCodeValidityTimeLimitInMinutes()) < DateTime.Now.ToLocalTime())
                return true;
            return false;
        }

        /// <summary>
        /// Gets code validity time limit in minutes from config
        /// </summary>
        /// <returns></returns>
        protected static int GetCodeValidityTimeLimitInMinutes()
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
        protected string GetVerificationCode()
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


        /// <summary>
        /// Gets concatenated representaion of specified verification code
        /// </summary>
        /// <param name="verificationCodeId"></param>
        /// <param name="profileId"></param>
        /// <param name="createdAtDate"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected string GetVerificationCodeConcatenate(Int64 verificationCodeId, Int64 profileId, DateTime createdAtDate, string code)
        {
            return string.Format("{0}{1}{2}{3}", verificationCodeId, profileId, createdAtDate, code);
        }
    }
}