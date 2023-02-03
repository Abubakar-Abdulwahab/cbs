using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class VerificationItemVM
    {
        public Int64 Id { get; set; }

        public VerificationCodeVM VerificationCodeVM { get; set; }

        public string CodeHash { get; set; }

        public int State { get; set; }
    }

}