using System;
using System.Collections.Generic;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using NHibernate.Linq;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    class PSSSettlementPreFlightItemsDAOManager : Repository<PSSSettlementPreFlightItems>, IPSSSettlementPreFlightItemsDAOManager
    {
        private static readonly ILogger log = new Log4netLogger();

        public PSSSettlementPreFlightItemsDAOManager(IUoW uow) : base(uow) { }


        /// <summary>
        /// Save pre flight items
        /// </summary>
        /// <param name="preFligtSettlementItems"></param>
        /// <returns>bool | return true if saved successfully</returns>
        public bool SaveItems(List<PSSSettlementPreFlightItems> preFligtSettlementItems)
        {
            try
            {
                _uow.BeginStatelessTransaction();
                foreach (var item in preFligtSettlementItems)
                {
                    _uow.Session.Save(item);
                }
                _uow.Commit();
                log.Info($"Saved records for pre settlement items");
                return true;
            }
            catch (Exception exception)
            {
                _uow.Rollback();
                log.Error($"Error inserting records settlement batch", exception);
                return false;
            }
        }


        /// <summary>
        /// We need to set the column to initiate queueing of items that are have not be added to the settlement batch
        /// to true, so the next job can pick them up and add to the settlement batch table
        /// </summary>
        /// <param name="preflightBatchId"></param>
        public void SetItemsForSettlementBatchInsertion(long preflightBatchId)
        {
            var queryText = $"UPDATE prfi SET {nameof(PSSSettlementPreFlightItems.AddToSettlementBatch)} = :boolVal FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementPreFlightItems)} prfi INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} stb ON stb.SettlementRangeStartDate = prfi.StartRange AND stb.SettlementRangeEndDate = prfi.EndRange AND stb.PSSSettlement_Id = prfi.PSSSettlement_Id WHERE prfi.Batch_Id = :preflightBatchId";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("boolVal", false);
            query.SetParameter("preflightBatchId", preflightBatchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Move the items that have AddToSettlementBatch set to true to the batch settlement table
        /// </summary>
        /// <param name="preflightBatchId"></param>
        public void InsertItemsForSettlementIntoSettlementBatchTable(long preflightBatchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} (PSSSettlement_Id, ScheduleDate, SettlementRangeStartDate, SettlementRangeEndDate, HasCommandSplits, CreatedAtUtc, UpdatedAtUtc) " + $"SELECT prfi.PSSSettlement_Id, prfi.SettlementScheduleDate, prfi.StartRange, prfi.EndRange, PSSet.HasCommandSplits, GETDATE(), GETDATE() FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementPreFlightItems)} prfi INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlement)} PSSet ON prfi.PSSSettlement_Id = PSSet.Id " + $"WHERE prfi.Batch_Id = :preflightBatchId AND prfi.AddToSettlementBatch = :boolVal;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("preflightBatchId", preflightBatchId);
            query.SetParameter("boolVal", true);
            query.ExecuteUpdate();
        }
    }
}