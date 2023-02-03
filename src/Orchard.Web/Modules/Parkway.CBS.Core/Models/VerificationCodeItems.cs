using System;

namespace Parkway.CBS.Core.Models
{
    public class VerificationCodeItems : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual VerificationCode VerificationCode { get; set; }

        public virtual string CodeHash { get; set; }

        public virtual int State { get; set; }
    }
}