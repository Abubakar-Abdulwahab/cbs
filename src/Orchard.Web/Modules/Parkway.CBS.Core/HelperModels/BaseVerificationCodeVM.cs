using System;

namespace Parkway.CBS.Core.HelperModels
{
    public abstract class BaseVerificationCodeVM
    {
        public Int64 Id { get; set; }

        public string CodeHash { get; set; }

        public int State { get; set; }

        public int ResendCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// <see cref="Models.Enums.VerificationType"/>
        /// </summary>
        public int VerificationType { get; set; }
    }
}