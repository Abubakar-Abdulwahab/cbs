using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSAdminVerificationCode : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSAdminUsers AdminUser { get; set; }

        public virtual int ResendCount { get; set; }

        /// <summary>
        /// <see cref="Enums.VerificationType"/>
        /// </summary>
        public virtual int VerificationType { get; set; }
    }


    public static class PSSAdminVerificationCodeExtension
    {
        /// <summary>
        /// Converts to PSSAdminVerificationCodeVM
        /// </summary>
        /// <param name="verificationCode"></param>
        /// <returns></returns>
        public static HelperModels.PSSAdminVerificationCodeVM ToVM(this PSSAdminVerificationCode verificationCode)
        {
            return new HelperModels.PSSAdminVerificationCodeVM
            {
                Id = verificationCode.Id,
                AdminUser = new VM.PSSAdminUsersVM { Id = verificationCode.AdminUser.Id, Email = verificationCode.AdminUser.Email, Fullname = verificationCode.AdminUser.Fullname, PhoneNumber = verificationCode.AdminUser.PhoneNumber },
                CreatedAtUtc = verificationCode.CreatedAtUtc,
                ResendCount = verificationCode.ResendCount,
                VerificationType = verificationCode.VerificationType
            };
        }
    }
}