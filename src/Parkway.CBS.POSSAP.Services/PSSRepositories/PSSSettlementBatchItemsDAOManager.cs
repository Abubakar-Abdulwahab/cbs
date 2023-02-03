using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementBatchItemsDAOManager : Repository<PSSSettlementBatchItems>, IPSSSettlementBatchItemsDAOManager
    {
        private static readonly ILogger log = new Log4netLogger();

        public PSSSettlementBatchItemsDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// When compute has been done with fee parties that
        /// do not have additional splits
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveNonSplitRecordsFromSplitComputeToBatchItems(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchItems)} " +
                $"({nameof(PSSSettlementBatchItems.SettlementFeeParty)}_Id, {nameof(PSSSettlementBatchItems.Invoice)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Batch)}_Id, {nameof(PSSSettlementBatchItems.FeeParty)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionLog)}_Id, {nameof(PSSSettlementBatchItems.Settlement)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.MDA)}_Id, {nameof(PSSSettlementBatchItems.RevenueHead)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionAmount)}, {nameof(PSSSettlementBatchItems.FeePercentage)}, " +
                $"{nameof(PSSSettlementBatchItems.AmountSettled)}, {nameof(PSSSettlementBatchItems.TransactionDate)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentDate)}, {nameof(PSSSettlementBatchItems.SynchronizationDate)}, " +
                $"{nameof(PSSSettlementBatchItems.GeneratedByCommand)}_Id, {nameof(PSSSettlementBatchItems.StateCommand)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.ZonalCommand)}_Id, {nameof(PSSSettlementBatchItems.Service)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Request)}_Id, {nameof(PSSSettlementBatchItems.State)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.LGA)}_Id, {nameof(PSSSettlementBatchItems.FeePartyName)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentProvider)}_Id, {nameof(PSSSettlementBatchItems.PaymentChannel)}, " +
                $"{nameof(PSSSettlementBatchItems.CreatedAtUtc)}, {nameof(PSSSettlementBatchItems.UpdatedAtUtc)}, " +
                $"{nameof(PSSSettlementBatchItems.SettlementDate)}, {nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate)}_Id) " +

                $"SELECT compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Invoice)}_Id, " +
                $":batchId, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, batch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.MDA)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}, tranlg.{nameof(TransactionLog.TransactionDate)}, " +
                $"tranlg.{nameof(TransactionLog.PaymentDate)}, tranlg.{nameof(TransactionLog.CreatedAtUtc)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Service)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, fp.{nameof(PSSFeeParty.Name)}, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Channel)}, " +
                $"GETDATE(), GETDATE(), GETDATE(), fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Id)} " +

                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} compSplit " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyBatchAggregate)} fpBatch ON fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.FeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Batch)}_Id = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :falseBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} config " +
                $"ON config.{nameof(PSSServiceSettlementConfigurationTransaction.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id =  config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id AND config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id  = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :falseBolValue " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(TransactionLog)} tranlg " +
                $"ON tranlg.{nameof(TransactionLog.Id)} = config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} fp " +
                $"ON fp.{nameof(PSSFeeParty.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} batch " +
                $"ON batch.{nameof(PSSSettlementBatch.Id)} = :batchId;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("falseBolValue", false);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When compute has been done with fee parties that
        /// have additional splits
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        public void MoveAdditionalSplitRecordsFromSplitComputeToBatchItemsForCommandAdapter(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchItems)} " +
                $"({nameof(PSSSettlementBatchItems.SettlementFeeParty)}_Id, {nameof(PSSSettlementBatchItems.Invoice)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Batch)}_Id, {nameof(PSSSettlementBatchItems.FeeParty)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionLog)}_Id, {nameof(PSSSettlementBatchItems.Settlement)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.MDA)}_Id, {nameof(PSSSettlementBatchItems.RevenueHead)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionAmount)}, {nameof(PSSSettlementBatchItems.FeePercentage)}, " +
                $"{nameof(PSSSettlementBatchItems.AmountSettled)}, {nameof(PSSSettlementBatchItems.TransactionDate)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentDate)}, {nameof(PSSSettlementBatchItems.SynchronizationDate)}, " +
                $"{nameof(PSSSettlementBatchItems.GeneratedByCommand)}_Id, {nameof(PSSSettlementBatchItems.StateCommand)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.ZonalCommand)}_Id, {nameof(PSSSettlementBatchItems.Service)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Request)}_Id, {nameof(PSSSettlementBatchItems.State)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.LGA)}_Id, {nameof(PSSSettlementBatchItems.FeePartyName)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentProvider)}_Id, {nameof(PSSSettlementBatchItems.PaymentChannel)}, " +
                $"{nameof(PSSSettlementBatchItems.CreatedAtUtc)}, {nameof(PSSSettlementBatchItems.UpdatedAtUtc)}, " +
                $"{nameof(PSSSettlementBatchItems.SettlementDate)}, {nameof(PSSSettlementBatchItems.AdditionalSplitValue)}, {nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate)}_Id, {nameof(PSSSettlementBatchItems.SettlementCommand)}_Id) " +

                $"SELECT compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Invoice)}_Id, " +
                $":batchId, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, batch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.MDA)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}, tranlg.{nameof(TransactionLog.TransactionDate)}, " +
                $"tranlg.{nameof(TransactionLog.PaymentDate)}, tranlg.{nameof(TransactionLog.CreatedAtUtc)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Service)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, fp.{nameof(PSSFeeParty.Name)}, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Channel)}, " +
                $"GETDATE(), GETDATE(), GETDATE(), compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)}, fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Id)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id " +

                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} compSplit " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyBatchAggregate)} fpBatch ON fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.AdditionalSplitValue)} = :adapterVal AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.FeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id AND fpBatch.Batch_Id = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :trueBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Command)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} config " +
                $"ON config.{nameof(PSSServiceSettlementConfigurationTransaction.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id =  config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id AND config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id  = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :trueBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)} = :adapterVal " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(TransactionLog)} tranlg " +
                $"ON tranlg.{nameof(TransactionLog.Id)} = config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} fp " +
                $"ON fp.{nameof(PSSFeeParty.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} batch " +
                $"ON batch.{nameof(PSSSettlementBatch.Id)} = :batchId;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBolValue", true);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When compute has been done with fee parties that
        /// have additional splits and adapter value State
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        public void MoveAdditionalSplitRecordsForStateFromSplitComputeToBatchItems(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchItems)} " +
                $"({nameof(PSSSettlementBatchItems.SettlementFeeParty)}_Id, {nameof(PSSSettlementBatchItems.Invoice)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Batch)}_Id, {nameof(PSSSettlementBatchItems.FeeParty)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionLog)}_Id, {nameof(PSSSettlementBatchItems.Settlement)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.MDA)}_Id, {nameof(PSSSettlementBatchItems.RevenueHead)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionAmount)}, {nameof(PSSSettlementBatchItems.FeePercentage)}, " +
                $"{nameof(PSSSettlementBatchItems.AmountSettled)}, {nameof(PSSSettlementBatchItems.TransactionDate)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentDate)}, {nameof(PSSSettlementBatchItems.SynchronizationDate)}, " +
                $"{nameof(PSSSettlementBatchItems.GeneratedByCommand)}_Id, {nameof(PSSSettlementBatchItems.StateCommand)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.ZonalCommand)}_Id, {nameof(PSSSettlementBatchItems.Service)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Request)}_Id, {nameof(PSSSettlementBatchItems.State)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.LGA)}_Id, {nameof(PSSSettlementBatchItems.FeePartyName)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentProvider)}_Id, {nameof(PSSSettlementBatchItems.PaymentChannel)}, " +
                $"{nameof(PSSSettlementBatchItems.CreatedAtUtc)}, {nameof(PSSSettlementBatchItems.UpdatedAtUtc)}, " +
                $"{nameof(PSSSettlementBatchItems.SettlementDate)}, {nameof(PSSSettlementBatchItems.AdditionalSplitValue)}, {nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate)}_Id, {nameof(PSSSettlementBatchItems.SettlementCommand)}_Id) " +

                $"SELECT compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Invoice)}_Id, " +
                $":batchId, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, batch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.MDA)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}, tranlg.{nameof(TransactionLog.TransactionDate)}, " +
                $"tranlg.{nameof(TransactionLog.PaymentDate)}, tranlg.{nameof(TransactionLog.CreatedAtUtc)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Service)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, fp.{nameof(PSSFeeParty.Name)}, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Channel)}, " +
                $"GETDATE(), GETDATE(), GETDATE(), compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)}, fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Id)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id " +

                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} compSplit " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyBatchAggregate)} fpBatch ON fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.AdditionalSplitValue)} = :adapterVal AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.FeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id AND fpBatch.Batch_Id = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :trueBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Command)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} config " +
                $"ON config.{nameof(PSSServiceSettlementConfigurationTransaction.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id =  config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id AND config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id  = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :trueBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)} = :adapterVal " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(TransactionLog)} tranlg " +
                $"ON tranlg.{nameof(TransactionLog.Id)} = config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} fp " +
                $"ON fp.{nameof(PSSFeeParty.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} batch " +
                $"ON batch.{nameof(PSSSettlementBatch.Id)} = :batchId;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBolValue", true);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When compute has been done with fee parties that
        /// have additional splits and adapter value Zonal
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        public void MoveAdditionalSplitRecordsForZonalFromSplitComputeToBatchItems(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchItems)} " +
                $"({nameof(PSSSettlementBatchItems.SettlementFeeParty)}_Id, {nameof(PSSSettlementBatchItems.Invoice)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Batch)}_Id, {nameof(PSSSettlementBatchItems.FeeParty)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionLog)}_Id, {nameof(PSSSettlementBatchItems.Settlement)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.MDA)}_Id, {nameof(PSSSettlementBatchItems.RevenueHead)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionAmount)}, {nameof(PSSSettlementBatchItems.FeePercentage)}, " +
                $"{nameof(PSSSettlementBatchItems.AmountSettled)}, {nameof(PSSSettlementBatchItems.TransactionDate)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentDate)}, {nameof(PSSSettlementBatchItems.SynchronizationDate)}, " +
                $"{nameof(PSSSettlementBatchItems.GeneratedByCommand)}_Id, {nameof(PSSSettlementBatchItems.StateCommand)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.ZonalCommand)}_Id, {nameof(PSSSettlementBatchItems.Service)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Request)}_Id, {nameof(PSSSettlementBatchItems.State)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.LGA)}_Id, {nameof(PSSSettlementBatchItems.FeePartyName)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentProvider)}_Id, {nameof(PSSSettlementBatchItems.PaymentChannel)}, " +
                $"{nameof(PSSSettlementBatchItems.CreatedAtUtc)}, {nameof(PSSSettlementBatchItems.UpdatedAtUtc)}, " +
                $"{nameof(PSSSettlementBatchItems.SettlementDate)}, {nameof(PSSSettlementBatchItems.AdditionalSplitValue)}, {nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate)}_Id, {nameof(PSSSettlementBatchItems.SettlementCommand)}_Id) " +

                $"SELECT compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Invoice)}_Id, " +
                $":batchId, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, batch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.MDA)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}, tranlg.{nameof(TransactionLog.TransactionDate)}, " +
                $"tranlg.{nameof(TransactionLog.PaymentDate)}, tranlg.{nameof(TransactionLog.CreatedAtUtc)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Service)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, fp.{nameof(PSSFeeParty.Name)}, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Channel)}, " +
                $"GETDATE(), GETDATE(), GETDATE(), compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)}, fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Id)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id " +

                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} compSplit " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyBatchAggregate)} fpBatch ON fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.AdditionalSplitValue)} = :adapterVal AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.FeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id AND fpBatch.Batch_Id = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :trueBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Command)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} config " +
                $"ON config.{nameof(PSSServiceSettlementConfigurationTransaction.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id =  config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id AND config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id  = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :trueBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)} = :adapterVal " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(TransactionLog)} tranlg " +
                $"ON tranlg.{nameof(TransactionLog.Id)} = config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} fp " +
                $"ON fp.{nameof(PSSFeeParty.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} batch " +
                $"ON batch.{nameof(PSSSettlementBatch.Id)} = :batchId;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBolValue", true);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }

        /// <summary>
        /// When compute has been done with fee parties that
        /// have additional splits and adapter value in Adapter command table
        /// we move these records to the batch items table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        public void MoveAdditionalSplitRecordsFromSplitComputeToBatchItemsForAdapterCommand(long batchId, string adapterValue)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchItems)} " +
                $"({nameof(PSSSettlementBatchItems.SettlementFeeParty)}_Id, {nameof(PSSSettlementBatchItems.Invoice)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Batch)}_Id, {nameof(PSSSettlementBatchItems.FeeParty)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionLog)}_Id, {nameof(PSSSettlementBatchItems.Settlement)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.MDA)}_Id, {nameof(PSSSettlementBatchItems.RevenueHead)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionAmount)}, {nameof(PSSSettlementBatchItems.FeePercentage)}, " +
                $"{nameof(PSSSettlementBatchItems.AmountSettled)}, {nameof(PSSSettlementBatchItems.TransactionDate)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentDate)}, {nameof(PSSSettlementBatchItems.SynchronizationDate)}, " +
                $"{nameof(PSSSettlementBatchItems.GeneratedByCommand)}_Id, {nameof(PSSSettlementBatchItems.StateCommand)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.ZonalCommand)}_Id, {nameof(PSSSettlementBatchItems.Service)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Request)}_Id, {nameof(PSSSettlementBatchItems.State)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.LGA)}_Id, {nameof(PSSSettlementBatchItems.FeePartyName)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentProvider)}_Id, {nameof(PSSSettlementBatchItems.PaymentChannel)}, " +
                $"{nameof(PSSSettlementBatchItems.CreatedAtUtc)}, {nameof(PSSSettlementBatchItems.UpdatedAtUtc)}, " +
                $"{nameof(PSSSettlementBatchItems.SettlementDate)}, {nameof(PSSSettlementBatchItems.AdditionalSplitValue)}, {nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate)}_Id, {nameof(PSSSettlementBatchItems.SettlementCommand)}_Id) " +

                $"SELECT compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Invoice)}_Id, " +
                $":batchId, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, batch.{nameof(PSSSettlementBatch.PSSSettlement)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.MDA)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitPercentage)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SplitAmount)}, tranlg.{nameof(TransactionLog.TransactionDate)}, " +
                $"tranlg.{nameof(TransactionLog.PaymentDate)}, tranlg.{nameof(TransactionLog.CreatedAtUtc)}, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Command)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.StateCommand)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ZonalCommand)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Service)}_Id, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.State)}_Id, " +
                $"compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.LGA)}_Id, fp.{nameof(PSSFeeParty.Name)}, " +
                $"config.{nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider)}_Id, config.{nameof(PSSServiceSettlementConfigurationTransaction.Channel)}, " +
                $"GETDATE(), GETDATE(), GETDATE(), compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)}, fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Id)}, compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id " +

                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute)} compSplit " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyBatchAggregate)} fpBatch ON fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementFeeParty)}_Id AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.AdditionalSplitValue)} = :adapterVal AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.FeeParty)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id AND fpBatch.Batch_Id = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :trueBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id = :batchId AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Command)}_Id = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.SettlementCommand)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} config " +
                $"ON config.{nameof(PSSServiceSettlementConfigurationTransaction.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.ConfigTransaction)}_Id AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.Batch)}_Id =  config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id AND config.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id  = :batchId AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.HasAdditionalSplit)} = :trueBolValue AND compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.AdditionalSplitValue)} = :adapterVal " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(TransactionLog)} tranlg " +
                $"ON tranlg.{nameof(TransactionLog.Id)} = config.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} fp " +
                $"ON fp.{nameof(PSSFeeParty.Id)} = compSplit.{nameof(PSSSettlementFeePartyRequestTransactionCommandSplitCompute.FeeParty)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} batch " +
                $"ON batch.{nameof(PSSSettlementBatch.Id)} = :batchId;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.SetParameter("trueBolValue", true);
            query.SetParameter("adapterVal", adapterValue);
            query.ExecuteUpdate();
        }

        /// <summary>
        /// Moves records from PSS Settlement Fee Party Request Transaction To Settlement Batch Items
        /// </summary>
        /// <param name="batchId"></param>
        public void MoveHasNoCommandSplitRecordsFromFeePartyRequestTransactionToBatchItems(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementBatchItems)} " +
                $"({nameof(PSSSettlementBatchItems.SettlementFeeParty)}_Id, {nameof(PSSSettlementBatchItems.Invoice)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Batch)}_Id, {nameof(PSSSettlementBatchItems.FeeParty)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionLog)}_Id, {nameof(PSSSettlementBatchItems.Settlement)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.MDA)}_Id, {nameof(PSSSettlementBatchItems.RevenueHead)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.TransactionAmount)}, {nameof(PSSSettlementBatchItems.FeePercentage)}, " +
                $"{nameof(PSSSettlementBatchItems.AmountSettled)}, {nameof(PSSSettlementBatchItems.TransactionDate)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentDate)}, {nameof(PSSSettlementBatchItems.SynchronizationDate)}, " +
                $"{nameof(PSSSettlementBatchItems.Service)}_Id, " +
                $"{nameof(PSSSettlementBatchItems.Request)}_Id, {nameof(PSSSettlementBatchItems.FeePartyName)}, " +
                $"{nameof(PSSSettlementBatchItems.PaymentProvider)}_Id, {nameof(PSSSettlementBatchItems.PaymentChannel)}, " +
                $"{nameof(PSSSettlementBatchItems.CreatedAtUtc)}, {nameof(PSSSettlementBatchItems.UpdatedAtUtc)}, " +
                $"{nameof(PSSSettlementBatchItems.SettlementDate)}, {nameof(PSSSettlementBatchItems.SettlementFeePartyBatchAggregate)}_Id) " +

                $"SELECT T4.{nameof(PSSSettlementBatch.PSSSettlement)}_Id, T3.{nameof(PSSServiceSettlementConfigurationTransaction.Invoice)}_Id, " +
                $"T1.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id, T1.{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id," +
                $"T1.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id, T1.{nameof(PSSSettlementFeePartyRequestTransaction.TransactionLog)}_Id, " +
                $"T3.{nameof(PSSServiceSettlementConfigurationTransaction.MDA)}_Id, T3.{nameof(PSSServiceSettlementConfigurationTransaction.RevenueHead)}_Id, " +
                $"T3.{nameof(PSSServiceSettlementConfigurationTransaction.SettlementAmount)}, T1.{nameof(PSSSettlementFeePartyRequestTransaction.DeductioPercentage)}, " +
                $"T1.{nameof(PSSSettlementFeePartyRequestTransaction.AmountToSettle)}, T5.{nameof(TransactionLog.TransactionDate)}, T5.{nameof(TransactionLog.PaymentDate)}, " +
                $"T5.{nameof(TransactionLog.CreatedAtUtc)}, T3.{nameof(PSSServiceSettlementConfigurationTransaction.Service)}_Id, T3.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, " +
                $"T2.{nameof(PSSFeeParty.Name)}, T3.{nameof(PSSServiceSettlementConfigurationTransaction.PaymentProvider)}_Id, T3.{nameof(PSSServiceSettlementConfigurationTransaction.Channel)}, " +
                $"GETDATE(), GETDATE(), GETDATE(), fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Id)}  FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyRequestTransaction)} T1  " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} AS T3 ON " +
                $"T1.{nameof(PSSSettlementFeePartyRequestTransaction.ConfigTransaction)}_Id = T3.{nameof(PSSServiceSettlementConfigurationTransaction.Id)} " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementBatch)} AS T4 ON T4.{nameof(PSSSettlementBatch.Id)} = T1.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSFeeParty)} AS T2 ON T1.{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id = T2.{nameof(PSSFeeParty.Id)} " +
                $"INNER JOIN Parkway_CBS_Core_{nameof(TransactionLog)} AS T5 ON T5.{nameof(TransactionLog.Id)} = T1.{nameof(PSSSettlementFeePartyRequestTransaction.TransactionLog)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(PSSSettlementFeePartyBatchAggregate)} fpBatch ON fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.SettlementFeeParty)}_Id = T1.{nameof(PSSSettlementFeePartyRequestTransaction.SettlementFeeParty)}_Id AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.FeeParty)}_Id = T1.{nameof(PSSSettlementFeePartyRequestTransaction.FeeParty)}_Id AND fpBatch.{nameof(PSSSettlementFeePartyBatchAggregate.Batch)}_Id = T1.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id " +
                $"WHERE T1.{nameof(PSSSettlementFeePartyRequestTransaction.Batch)}_Id = :batchId; ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }
                     

        /// <summary>
        /// Move items to be settled from police collection log to settlement batch items table
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <param name="settlementBatchId"></param>
        /// <returns>bool</returns>
        public bool MoveRecordFromPoliceCollectionLogToSettlementBatchItems(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM, int settlementBatchId)
        {
            try
            {
                //Move records into PSSSettlementBatchItems table
                var queryText = $"INSERT INTO Parkway_CBS_Police_Core_PSSSettlementBatchItems (Name, SettlementBatch_Id, Service_Id, MDA_Id, RevenueHead_Id, Channel, PaymentProvider_Id, Request_Id, Invoice_Id, Amount, CreatedAtUtc, UpdatedAtUtc) SELECT rh.Name, :settlementBatchId, ps.Id, tl.MDA_Id, tl.RevenueHead_Id, tl.Channel, tl.PaymentProvider, pcl.Request_Id, tl.Invoice_Id, tl.AmountPaid, :dateSaved, :dateSaved FROM Parkway_CBS_Police_Core_PoliceCollectionLog pcl INNER JOIN Parkway_CBS_Core_TransactionLog tl ON tl.Id = pcl.TransactionLog_Id INNER JOIN Parkway_CBS_Police_Core_PSSRequest pr ON pr.Id = pcl.Request_Id INNER JOIN Parkway_CBS_Police_Core_PSService ps ON ps.Id = pr.Service_Id INNER JOIN Parkway_CBS_Police_Core_PSServiceRevenueHead prh ON prh.FlowDefinitionLevel_Id = :definitionLevelId INNER JOIN Parkway_CBS_Core_RevenueHead rh ON rh.Id = tl.RevenueHead_Id WHERE tl.MDA_Id = :mdaId AND tl.RevenueHead_Id = :revId AND tl.PaymentProvider = :provider AND tl.Channel = :channel AND pr.Service_Id = :serviceId AND tl.Settled = :settledVal AND cast (tl.CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("settlementBatchId", settlementBatchId);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("mdaId", pssServiceSettlement.MDAId);
                query.SetParameter("revId", pssServiceSettlement.RevenueHeadId);
                query.SetParameter("provider", pssServiceSettlement.PaymentProviderId);
                query.SetParameter("channel", pssServiceSettlement.Channel);
                query.SetParameter("serviceId", pssServiceSettlement.ServiceId);
                query.SetParameter("definitionLevelId", pssServiceSettlement.DefinitionLevelId);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd"));
                int numRecordInserted = query.ExecuteUpdate();

                //Update column IsSettled in TransactionLog table to true
                int numRecordUpdated = 0;
                if(numRecordInserted > 0)
                {
                    numRecordUpdated = UpdateTransactionLog(pssServiceSettlement, settlementRuleVM);
                }

                log.Info($"{numRecordInserted} record(s) inserted for params:::Rule Code => MDA Id => {pssServiceSettlement.MDAId}, Revenue Head Id => {pssServiceSettlement.RevenueHeadId}, Provider => {pssServiceSettlement.PaymentProviderId}, Channel => {pssServiceSettlement.Channel}, Service Id => {pssServiceSettlement.ServiceId}, Definition LevelId => {pssServiceSettlement.DefinitionLevelId}, Schudule Date => {settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd")}");

                log.Info($"{numRecordUpdated} record(s) updated for params:::Rule Code => MDA Id => {pssServiceSettlement.MDAId}, Revenue Head Id => {pssServiceSettlement.RevenueHeadId}, Provider => {pssServiceSettlement.PaymentProviderId}, Channel => {pssServiceSettlement.Channel}, Service Id => {pssServiceSettlement.ServiceId}, Definition LevelId => {pssServiceSettlement.DefinitionLevelId}, Schudule Date => {settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd")}");

                return true;
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// Save bundle of records
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <param name="settlementBatchId"></param>
        /// <param name="collectionLogVMs"></param>
        /// <param name="batchLimit"></param>
        public void SaveRecords(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM, int settlementBatchId, ConcurrentQueue<PoliceCollectionLogVM> collectionLogVMs, int batchLimit)
        {
            int chunkSize = batchLimit;
            var dataSize = collectionLogVMs.Count();

            double pageSize = ((double)dataSize / chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;

            List<DataTable> listOfDataTables = new List<DataTable> { };
            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(PSSSettlementBatchItems).Name);
                    dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("SettlementBatch_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("Service_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("MDA_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("RevenueHead_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("Channel", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("PaymentProvider_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("Request_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn("Invoice_Id", typeof(long)));
                    dataTable.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("IsDeduction", typeof(bool)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    collectionLogVMs.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["Name"] = x.RevenueHeadName;
                        row["SettlementBatch_Id"] = settlementBatchId;
                        row["Service_Id"] = pssServiceSettlement.ServiceId;
                        row["MDA_Id"] = pssServiceSettlement.MDAId;
                        row["RevenueHead_Id"] = pssServiceSettlement.RevenueHeadId;
                        row["Channel"] = pssServiceSettlement.Channel;
                        row["PaymentProvider_Id"] = pssServiceSettlement.PaymentProviderId;
                        row["Request_Id"] = x.RequestId;
                        row["Invoice_Id"] = x.InvoiceId;
                        row["Amount"] = x.AmountPaid;
                        row["IsDeduction"] = x.IsDeduction;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    listOfDataTables.Add(dataTable);
                    skip += chunkSize;
                    stopper++;
                }
                //we now have a collection of datatables, lets save the bunch together
                if (!SaveBundle(listOfDataTables, "Parkway_CBS_Police_Core_" + typeof(PSSSettlementBatchItems).Name))
                { throw new Exception("Error saving items into  PSSSettlementBatchItems for batch staging Id " + settlementBatchId); }

                //Update column IsSettled in TransactionLog table to true
                int numRecordUpdated = UpdateTransactionLog(pssServiceSettlement, settlementRuleVM);

               int numRecordInserted = MoveRecordFromSettlementBatchItemToPresettlementDeductions(pssServiceSettlement, settlementBatchId);

                log.Info($"{dataSize} record(s) inserted into PSSSettlementBatchItems for params:::Rule Code => MDA Id => {pssServiceSettlement.MDAId}, Revenue Head Id => {pssServiceSettlement.RevenueHeadId}, Provider => {pssServiceSettlement.PaymentProviderId}, Channel => {pssServiceSettlement.Channel}, Service Id => {pssServiceSettlement.ServiceId}, Definition LevelId => {pssServiceSettlement.DefinitionLevelId}, Schudule Date => {settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd")}");

                log.Info($"{numRecordUpdated} record(s) updated for params:::Rule Code => MDA Id => {pssServiceSettlement.MDAId}, Revenue Head Id => {pssServiceSettlement.RevenueHeadId}, Provider => {pssServiceSettlement.PaymentProviderId}, Channel => {pssServiceSettlement.Channel}, Service Id => {pssServiceSettlement.ServiceId}, Definition LevelId => {pssServiceSettlement.DefinitionLevelId}, Schudule Date => {settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd")}");

                log.Info($"{numRecordInserted} record(s) inserted into PSSPresettlementDeductions for params:::Rule Code => MDA Id => {pssServiceSettlement.MDAId}, Revenue Head Id => {pssServiceSettlement.RevenueHeadId}, Provider => {pssServiceSettlement.PaymentProviderId}, Channel => {pssServiceSettlement.Channel}, Service Id => {pssServiceSettlement.ServiceId}, Definition LevelId => {pssServiceSettlement.DefinitionLevelId}, Schudule Date => {settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd")}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update transaction log settled column to true after moving the items into settlement batch items table
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementRuleVM"></param>
        /// <returns>int</returns>
        private int UpdateTransactionLog(PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM)
        {
            try
            {
                //Update transaction log column IsSettled
                var queryText = $"UPDATE tl SET tl.Settled = :settledNewVal FROM Parkway_CBS_Core_TransactionLog tl INNER JOIN Parkway_CBS_Police_Core_PoliceCollectionLog pcl ON tl.Id = pcl.TransactionLog_Id INNER JOIN Parkway_CBS_Police_Core_PSSRequest pr ON pr.Id = pcl.Request_Id INNER JOIN Parkway_CBS_Police_Core_PSService ps ON ps.Id = pr.Service_Id INNER JOIN Parkway_CBS_Police_Core_PSServiceRevenueHead prh ON prh.FlowDefinitionLevel_Id = :definitionLevelId INNER JOIN Parkway_CBS_Core_RevenueHead rh ON rh.Id = tl.RevenueHead_Id WHERE tl.MDA_Id = :mdaId AND tl.RevenueHead_Id = :revId AND tl.PaymentProvider = :provider AND tl.Channel = :channel AND pr.Service_Id = :serviceId AND tl.Settled = :settledVal AND cast (tl.CreatedAtUtc as date) = :date";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("mdaId", pssServiceSettlement.MDAId);
                query.SetParameter("revId", pssServiceSettlement.RevenueHeadId);
                query.SetParameter("provider", pssServiceSettlement.PaymentProviderId);
                query.SetParameter("channel", pssServiceSettlement.Channel);
                query.SetParameter("serviceId", pssServiceSettlement.ServiceId);
                query.SetParameter("definitionLevelId", pssServiceSettlement.DefinitionLevelId);
                query.SetParameter("settledNewVal", true);
                query.SetParameter("settledVal", false);
                query.SetParameter("date", settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd"));

                return query.ExecuteUpdate(); ;
            }
            catch (Exception)
            { throw; }
        }

        /// <summary>
        /// Move items with IsDeduction column true from PSSSettlementBatchItems table to PSSPresettlementDeductions
        /// </summary>
        /// <param name="pssServiceSettlement"></param>
        /// <param name="settlementBatchId"></param>
        /// <returns>int</returns>
        private int MoveRecordFromSettlementBatchItemToPresettlementDeductions(PSSServiceSettlementConfigurationVM pssServiceSettlement, int settlementBatchId)
        {
            try
            {
                //Move records into PSSSettlementBatchItems table
                var queryText = $"INSERT INTO Parkway_CBS_Police_Core_PSSPresettlementDeductions (Name, SettlementBatch_Id, Service_Id, MDA_Id, RevenueHead_Id, Channel, PaymentProvider_Id, Request_Id, Invoice_Id, Amount, CreatedAtUtc, UpdatedAtUtc) SELECT Name, SettlementBatch_Id, Service_Id, MDA_Id, RevenueHead_Id, Channel, PaymentProvider_Id, Request_Id, Invoice_Id, Amount, :dateSaved, :dateSaved FROM Parkway_CBS_Police_Core_PSSSettlementBatchItems WHERE SettlementBatch_Id = :settlementBatchId AND MDA_Id = :mdaId AND RevenueHead_Id = :revId AND PaymentProvider_Id = :provider AND Channel = :channel AND Service_Id = :serviceId AND IsDeduction= :isDeduction";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("settlementBatchId", settlementBatchId);
                query.SetParameter("dateSaved", DateTime.Now.ToLocalTime());
                query.SetParameter("mdaId", pssServiceSettlement.MDAId);
                query.SetParameter("revId", pssServiceSettlement.RevenueHeadId);
                query.SetParameter("provider", pssServiceSettlement.PaymentProviderId);
                query.SetParameter("channel", pssServiceSettlement.Channel);
                query.SetParameter("serviceId", pssServiceSettlement.ServiceId);
                query.SetParameter("isDeduction", true);

                return query.ExecuteUpdate(); ;
            }
            catch (Exception)
            { throw; }
        }

        public dynamic GetBatchAggregateAmount(PSSServiceSettlementConfigurationVM pssServiceSettlement, int settlementBatchId)
        {
            try
            {
                var queryText = $" SELECT SUM(Amount) as totalAmountPaid, COUNT(Id) as totalTransactionCount FROM Parkway_CBS_Police_Core_PSSSettlementBatchItems WHERE SettlementBatch_Id = :settlementBatchId AND MDA_Id= :mdaId AND RevenueHead_Id= :revId AND IsDeduction= :isDeduction";

                var query = _uow.Session.CreateSQLQuery(queryText);
                query.SetParameter("settlementBatchId", settlementBatchId);
                query.SetParameter("revId", pssServiceSettlement.RevenueHeadId);
                query.SetParameter("mdaId", pssServiceSettlement.MDAId);
                query.SetParameter("isDeduction", false);
                query.ExecuteUpdate();

                var bn = query.UniqueResult() as IEnumerable<object>;
                decimal? amt = 0.00m;
                int cnt = 0;
                if (bn != null)
                {
                    var bsn = query.UniqueResult() as IEnumerable<object>;
                    if (bsn.ElementAt(0) == null) { amt = 0.00m; }
                    else { amt = (decimal)bsn.ElementAt(0); }

                    cnt = (int)bsn.ElementAt(1);
                }

                dynamic retObj = new ExpandoObject();
                retObj.TotalAmount = amt;
                retObj.TotalCount = cnt;
                return retObj;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

