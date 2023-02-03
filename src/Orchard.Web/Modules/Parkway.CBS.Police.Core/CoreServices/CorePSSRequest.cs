using Orchard.Logging;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;


namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePSSRequest : ICorePSSRequest
    {
        private readonly IPSSRequestManager<PSSRequest> _reqRepo;
        public ILogger Logger { get; set; }

        public CorePSSRequest(IPSSRequestManager<PSSRequest> reqRepo)
        {
            _reqRepo = reqRepo;
            Logger = NullLogger.Instance;
        }

    }
}