using System;

namespace Parkway.CBS.Core.Models
{
    public class VerificationCode : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual CBSUser CBSUser { get; set; }

        public virtual int ResendCount { get; set; }

        /// <summary>
        /// <see cref="Enums.VerificationType"/>
        /// </summary>
        public virtual int VerificationType { get; set; }
    }
}