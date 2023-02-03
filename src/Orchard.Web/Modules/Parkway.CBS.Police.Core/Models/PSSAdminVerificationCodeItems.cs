using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSAdminVerificationCodeItems : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSAdminVerificationCode VerificationCode { get; set; }

        public virtual string CodeHash { get; set; }

        public virtual int State { get; set; }
    }
}