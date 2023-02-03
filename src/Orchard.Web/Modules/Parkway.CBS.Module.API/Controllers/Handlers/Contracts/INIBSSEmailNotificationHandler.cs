using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface INIBSSEmailNotificationHandler : IDependency
    {
        /// <summary>
        /// Send email containing IV and secret credentials to NIBSS email address 
        /// </summary>
        /// <returns>APIResponse</returns>
        APIResponse SendNIBSSIntegrationCredentials();
    }
}
