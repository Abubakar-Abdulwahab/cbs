using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementScheduleUpdateDAOManager : Repository<PSSSettlementScheduleUpdate>, IPSSSettlementScheduleUpdateDAOManager
    {
        private static readonly ILogger log = new Log4netLogger();

        public PSSSettlementScheduleUpdateDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Save settlement schedule update items
        /// </summary>
        /// <param name="settlementScheduleUpdateItems"></param>
        /// <returns>bool | return true if saved successfully</returns>
        public bool SaveItems(List<PSSSettlementScheduleUpdate> settlementScheduleUpdateItems)
        {
            try
            {
                _uow.BeginStatelessTransaction();
                foreach (var item in settlementScheduleUpdateItems)
                {
                    _uow.Session.Save(item);
                }
                _uow.Commit();
                log.Info($"Saved records for settlement schedule update items");
                return true;
            }
            catch (Exception exception)
            {
                _uow.Rollback();
                log.Error($"Error inserting records settlement schedule update", exception);
                return false;
            }
        }


        /// <summary>
        /// Updates settlement schedule date
        /// </summary>
        /// <param name="preflightBatchId"></param>
        public void UpdateSettlementScheduleDate(long preflightBatchId)
        {
            var queryText = $"UPDATE T2 SET T2.{nameof(Core.Models.SettlementRule.SettlementPeriodStartDate)} = T1.{nameof(PSSSettlementScheduleUpdate.NextStartDate)}," +
                $" T2.{nameof(Core.Models.SettlementRule.SettlementPeriodEndDate)} = T1.{nameof(PSSSettlementScheduleUpdate.NextEndDate)}," +
                $" T2.{nameof(Core.Models.SettlementRule.NextScheduleDate)} = T1.{nameof(PSSSettlementScheduleUpdate.NextSchedule)} FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementScheduleUpdate)} T1" +
                $" INNER JOIN Parkway_CBS_Core_{nameof(Core.Models.SettlementRule)} T2 ON T1.{nameof(PSSSettlementScheduleUpdate.SettlementRule)}_Id = T2.{nameof(Core.Models.SettlementRule.Id)}" +
                $" INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlement)} T3 ON T1.{nameof(PSSSettlementScheduleUpdate.PSSSettlement)}_Id = T3.{nameof(PSSSettlement.Id)}" +
                $" WHERE T1.{nameof(PSSSettlementScheduleUpdate.PSSSettlementPreFlightBatch)}_Id = :preflightBatchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("preflightBatchId", preflightBatchId);
            query.ExecuteUpdate();
        }
    }
}
