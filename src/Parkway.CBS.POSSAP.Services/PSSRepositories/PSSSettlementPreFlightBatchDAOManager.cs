using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementPreFlightBatchDAOManager : Repository<PSSSettlementPreFlightBatch>, IPSSSettlementPreFlightBatchDAOManager
    {
        private static readonly ILogger log = new Log4netLogger();

        public PSSSettlementPreFlightBatchDAOManager(IUoW uow) : base(uow) { }

    }
}