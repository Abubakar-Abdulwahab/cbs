using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface ILoginHandler : IDependency
    {
        APIResponse ProcessLoginRequest(AuthenticationModel authenticationModel, System.Net.Http.HttpRequestMessage httpRequest);
    }
}
