using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSSettlementFeePartyManager : BaseManager<PSSSettlementFeeParty>, IPSSSettlementFeePartyManager<PSSSettlementFeeParty>
    {
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public PSSSettlementFeePartyManager(IRepository<PSSSettlementFeeParty> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;

        }

        /// <summary>
        /// Sets all max percentage to false
        /// </summary>
        /// <param name="settlementId"></param>
        public void SetMaxPercentageToFalse(int settlementId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSSettlementFeeParty).Name;
                string settlementIdName = nameof(PSSSettlementFeeParty.Settlement) + "_Id";
                string maxPercentName = nameof(PSSSettlementFeeParty.MaxPercentage);
                string updatedAtName = nameof(PSSSettlementFeeParty.UpdatedAtUtc);

                var queryText = $"UPDATE {tableName} SET {maxPercentName} = :maxPercent, {updatedAtName} = :updateDate WHERE {settlementIdName} = :settlementId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("settlementId", settlementId);
                query.SetParameter("maxPercent", false);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating details for settlement fee party with id {0}, Exception message {1}", settlementId, exception.Message));
                throw;
            }
        }

        private int GetIdSettlementWithMaxPercentage(int settlementId)
        {
            return _transactionManager.GetSession().Query<PSSSettlementFeeParty>().Where(x => x.Settlement == new PSSSettlement { Id = settlementId} && !x.IsDeleted).OrderByDescending(x => x.DeductionValue).First().Id;
        }

        /// <summary>
        /// Sets the max percentage for settlement fee party
        /// </summary>
        /// <param name="settlementId"></param>
        public void SetMaxPercentage(int settlementId)
        {
            try
            {
                int pssSettlementFeePartyId = GetIdSettlementWithMaxPercentage(settlementId);

                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSSettlementFeeParty).Name;
                string settlementIdName = nameof(PSSSettlementFeeParty.Settlement) + "_Id";
                string psssettlementFeePartyIdName = nameof(PSSSettlementFeeParty.Id);
                string maxPercentName = nameof(PSSSettlementFeeParty.MaxPercentage);
                string updatedAtName = nameof(PSSSettlementFeeParty.UpdatedAtUtc);

                var queryText = $"UPDATE {tableName} SET {maxPercentName} = :maxPercent, {updatedAtName} = :updateDate WHERE {settlementIdName} = :settlementId AND {psssettlementFeePartyIdName} = :pssSettlementFeePartyId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("settlementId", settlementId);
                query.SetParameter("pssSettlementFeePartyId", pssSettlementFeePartyId);
                query.SetParameter("maxPercent", true);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating details for settlement fee party with id {0}, Exception message {1}", settlementId, exception.Message));
                throw;
            }
        }


    }
}