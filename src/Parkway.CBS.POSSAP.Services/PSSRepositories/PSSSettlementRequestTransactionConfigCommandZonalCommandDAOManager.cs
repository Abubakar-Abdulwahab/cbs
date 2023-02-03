using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementRequestTransactionConfigCommandZonalCommandDAOManager : Repository<PSSSettlementRequestTransactionConfigCommandZonalCommand>, IPSSSettlementRequestTransactionConfigCommandZonalCommandDAOManager
    {
        public PSSSettlementRequestTransactionConfigCommandZonalCommandDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// merge the command transaction request with the zonal command 
        /// pairing of the transaction, request, the commands, and zonal commands
        /// </summary>
        /// <param name="batchId"></param>
        public void MergeCommandTransactionRequestWithZonalCommand(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand)} " +
                $"({nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ZonalCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.RequestCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.RequestAndCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.UpdatedAtUtc)}) " +
                $"SELECT zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.TransactionLog)}_Id, zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id, cmd.{nameof(Command.ZonalCommand)}_Id, zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id, :batchId, zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Request)}_Id, zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} zcrtcc " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id = cmd.{nameof(Command.Id)} " +
                $"WHERE zcrtcc.{nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id = :batchId ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }


        /// <summary>
        /// When we have the records with the corresponding zonal command
        /// we need to update the state and LGA columns with the state and LGA of the 
        /// zonal commnad
        /// </summary>
        /// <param name="batchId"></param>
        public void UpdateBatchWithStateAndLGAOfZonalCommand(long batchId)
        {
            var queryText = $"UPDATE zrtcc " +
                $"SET {nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.State)}_Id = cmd.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.State)}_Id, " +
                $"{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.LGA)}_Id = cmd.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.LGA)}_Id " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand)} zrtcc " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON cmd.{nameof(Command.Id)} = zrtcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ZonalCommand)}_Id " +
                $"WHERE zrtcc.{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Batch)}_Id = :batchId;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }

    }
}
