using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.Contracts
{
    public interface IHandlerComposition : IDependency
    {

        UserDetailsModel GetLoggedInUserDetails(bool checkVerifiedGate = false);

    }
}
