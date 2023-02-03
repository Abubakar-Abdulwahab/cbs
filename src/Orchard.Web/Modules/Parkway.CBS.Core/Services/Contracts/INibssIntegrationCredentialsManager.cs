using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface INibssIntegrationCredentialsManager<NibssIntegrationCredentials> : IDependency, IBaseManager<NibssIntegrationCredentials>
    {
        /// <summary>
        /// Gets the active IV and SecretKey
        /// </summary>
        /// <returns>NIBSSIntegrationCredentialVM</returns>
        NIBSSIntegrationCredentialVM GetActiveNibssIntegrationCredential();
    }
}
