using Orchard;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreServiceStateCommand : ICoreServiceStateCommand
    {
        private readonly IPSServiceStateCommandManager<PSServiceStateCommand> _requestServiceStateCommandManager;
        private readonly IOrchardServices _orchardServices;

        public CoreServiceStateCommand(IPSServiceStateCommandManager<PSServiceStateCommand> requestServiceStateCommandManager, IOrchardServices orchardServices)
        {
            _requestServiceStateCommandManager = requestServiceStateCommandManager;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Get active commands
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="serviceId"></param>
        /// <returns>IEnumerable{CommandVM}</returns>
        public IEnumerable<CommandVM> GetActiveCommands(int stateId, int serviceId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            IEnumerable<CommandVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<CommandVM>>(tenant, $"{nameof(POSSAPCachePrefix.ServiceStateCommandList)}-{stateId}-{serviceId}");

            if (result == null)
            {
                result = _requestServiceStateCommandManager.GetActiveCommands(stateId, serviceId);

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.ServiceStateCommandList)}-{stateId}-{serviceId}", result);
                }
            }
            return result;
        }

        /// <summary>
        /// Get the active command with the stateId and service Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="commandId"></param>
        /// <param name="serviceId"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetActiveCommand(int stateId, int commandId, int serviceId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            CommandVM result = ObjectCacheProvider.GetCachedObject<CommandVM>(tenant, $"{nameof(POSSAPCachePrefix.ServiceStateCommand)}-{stateId}-{serviceId}-{commandId}");

            if (result == null)
            {
                result = _requestServiceStateCommandManager.GetActiveCommand(stateId, serviceId, commandId);

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.ServiceStateCommand)}-{stateId}-{serviceId}-{commandId}", result);
                }
            }
            return result;
        }

    }
}