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
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSSettlementManager : BaseManager<PSSSettlement>, IPSSSettlementManager<PSSSettlement>
    {
        private readonly IRepository<PSSSettlement> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public PSSSettlementManager(IRepository<PSSSettlement> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets all active settlements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PSSSettlementVM> GetActiveSettlements()
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSSettlement>().Where(x => x.IsActive).Select(x => new PSSSettlementVM { Id = x.Id, Name = x.Name }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets settlement using Id
        /// </summary>
        /// <param name="settlementId"></param>
        /// <returns></returns>
        public PSSSettlementVM GetSettlementById(int settlementId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSSettlement>().Where(x => x.Id == settlementId).Select(x => new PSSSettlementVM { Id = x.Id, Name = x.Name }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Updates updated at time for settlement with specified id
        /// </summary>
        /// <param name="settlementId"></param>
        public void UpdateSettlementBatchUpdatedAtDate(int settlementId)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_PSSSettlement SET {nameof(PSSSettlement.UpdatedAtUtc)} = :updatedAtUtc WHERE {nameof(PSSSettlement.Id)} = :settlementId";

                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("settlementId", settlementId);
                query.SetParameter("updatedAtUtc", DateTime.UtcNow.ToLocalTime());
                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}