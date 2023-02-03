using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IOfficersDataFromExternalSourceStagingManager<OfficersDataFromExternalSourceStaging> : IDependency, IBaseManager<OfficersDataFromExternalSourceStaging>
    {
    }
}
