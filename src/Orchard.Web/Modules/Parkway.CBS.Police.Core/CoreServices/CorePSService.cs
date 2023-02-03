using Orchard.Logging;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;


namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSService : ICorePSService
    {
        private readonly IPSServiceManager<PSService> _serviceRepo;
        public ILogger Logger { get; set; }

        public CorePSService(IPSServiceManager<PSService> serviceRepo)
        {
            _serviceRepo = serviceRepo;
            Logger = NullLogger.Instance;
        }



        /// <summary>
        /// Get the initialization request flow level Id for this service
        /// </summary>
        /// <param name="serviceId"></param>
        public int GetInitFlow(int serviceId)
        {
            return _serviceRepo.GetFirstLevelDefinitionId(serviceId);
        }


    }
}