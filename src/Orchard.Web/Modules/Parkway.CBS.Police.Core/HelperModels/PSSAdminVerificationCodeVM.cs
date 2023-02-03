using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class PSSAdminVerificationCodeVM : BaseVerificationCodeVM
    {
        public PSSAdminUsersVM AdminUser { get; set; }
    }
}