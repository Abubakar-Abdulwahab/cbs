using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSSettlementBatchManager : BaseManager<PSSSettlementBatch>, IPSSSettlementBatchManager<PSSSettlementBatch>
    {
        private readonly IRepository<PSSSettlementBatch> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public PSSSettlementBatchManager(IRepository<PSSSettlementBatch> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets id for settlement batch with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public long GetSettlementBatchId(string batchRef)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSSettlementBatch>().Where(x => x.BatchRef == batchRef).Select(x => x.Id).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets settlement batch with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        public PSSSettlementBatchVM GetSettlementBatchWithRef(string batchRef)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSSettlementBatch>().Where(x => x.BatchRef == batchRef).Select(x => new PSSSettlementBatchVM
                {
                    SettlementName = x.PSSSettlement.Name,
                    SettlementRangeStartDate = x.SettlementRangeStartDate,
                    SettlementRangeEndDate = x.SettlementRangeEndDate,
                    StatusMessage = x.StatusMessage,
                    TransactionDate = x.SettlementDate.Value,
                    SettlementBatchRef = x.BatchRef
                }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}