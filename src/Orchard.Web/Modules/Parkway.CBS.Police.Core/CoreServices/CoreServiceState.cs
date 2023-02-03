using Orchard;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreServiceState : ICoreServiceState
    {
        private readonly IPSServiceStateManager<PSServiceState> _serviceStateManager;
        private readonly IOrchardServices _orchardServices;

        public CoreServiceState(IPSServiceStateManager<PSServiceState> serviceStateManager, IOrchardServices orchardServices)
        {
            _serviceStateManager = serviceStateManager;
            _orchardServices = orchardServices;
        }


    }
}