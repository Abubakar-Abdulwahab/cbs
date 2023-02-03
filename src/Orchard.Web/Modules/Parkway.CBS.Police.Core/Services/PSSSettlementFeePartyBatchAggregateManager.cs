using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSSettlementFeePartyBatchAggregateManager : BaseManager<PSSSettlementFeePartyBatchAggregate>, IPSSSettlementFeePartyBatchAggregateManager<PSSSettlementFeePartyBatchAggregate>
    {
        private readonly IRepository<PSSSettlementFeePartyBatchAggregate> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public PSSSettlementFeePartyBatchAggregateManager(IRepository<PSSSettlementFeePartyBatchAggregate> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get fee party batch aggregate model for fee party with specified id belonging to batch with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <param name="feePartyBatchAggregateId"></param>
        /// <returns></returns>
        public PSSSettlementFeePartyBatchAggregateVM GetFeePartyBatchInfo(string batchRef, long feePartyBatchAggregateId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSSettlementFeePartyBatchAggregate>().Where(x => (x.Id == feePartyBatchAggregateId) && (x.Batch.BatchRef == batchRef))
                    .Select(x => new PSSSettlementFeePartyBatchAggregateVM { Batch = new DTO.PSSSettlementBatchVM { SettlementName = x.Batch.PSSSettlement.Name, SettlementRangeStartDate = x.Batch.SettlementRangeStartDate, SettlementRangeEndDate = x.Batch.SettlementRangeEndDate }, FeePartyName = x.FeePartyName, TransactionDate = x.Batch.UpdatedAtUtc.Value, SettlementDate = x.Batch.SettlementDate }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}