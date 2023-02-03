using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSSettlementFeePartyStagingManager : BaseManager<PSSSettlementFeePartyStaging>, IPSSSettlementFeePartyStagingManager<PSSSettlementFeePartyStaging>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }
        public PSSSettlementFeePartyStagingManager(IRepository<PSSSettlementFeePartyStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Sets IsDeleted to true for the removed settlement fee party in PSSSettlementFeeParty
        /// </summary>
        /// <param name="reference"></param>
        public void UpdateSettlementFeePartyFromStaging(string reference)
        {
            try
            {

                var queryText = $"MERGE Parkway_CBS_Police_Core_PSSSettlementFeeParty AS Target USING Parkway_CBS_Police_Core_PSSSettlementFeePartyStaging AS Source ON Source.Settlement_Id = Target.Settlement_Id AND Source.FeeParty_Id = Target.FeeParty_Id WHEN MATCHED AND Source.Reference = '{reference}' THEN UPDATE SET Target.IsDeleted = Source.IsDeleted, Target.DeductionValue = Source.DeductionValue, Target.Position = Source.Position, Target.IsActive = Source.IsActive, Target.AdditionalSplitValue = Source.AdditionalSplitValue, Target.MaxPercentage = Source.MaxPercentage, Target.UpdatedAtUtc = Source.UpdatedAtUtc, Target.HasAdditionalSplits = Source.HasAdditionalSplits WHEN NOT MATCHED BY Target AND Source.Reference = '{reference}' THEN INSERT(Settlement_Id, FeeParty_Id, HasAdditionalSplits, AdditionalSplitValue, DeductionValue, Position, IsActive, MaxPercentage, DeductionTypeId, CreatedAtUtc, UpdatedAtUtc, IsDeleted) VALUES(Source.Settlement_Id, Source.FeeParty_Id, Source.HasAdditionalSplits, Source.AdditionalSplitValue,  Source.DeductionValue, Source.Position, Source.IsActive, Source.MaxPercentage, Source.DeductionTypeId, GETDATE(), GETDATE(), 0);";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating details for settlement fee party with reference {0}, Exception message {1}", reference, exception.Message));
                throw;
            }
        }
    }
}