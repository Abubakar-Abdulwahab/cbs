using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IAccountWalletConfigurationAJAXHandler : IDependency
    {
        /// <summary>
        /// Gets admin user detail using the <paramref name="adminUsername"/>
        /// </summary>
        /// <param name="adminUsername"></param>
        /// <returns></returns>
        APIResponse GetAdminUserDetail(string adminUsername);
    }
}
