using Parkway.CBS.Core.Models;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSServiceSettlementConfigTranxCommandDAOManager : Repository<PSSSettlementRequestTransactionConfigCommand>, IPSSServiceSettlementConfigTranxCommandDAOManager
    {
        public PSSServiceSettlementConfigTranxCommandDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// insert into the PSSSettlementRequestTransactionConfigCommand table the 
        /// pairing of the transaction, request and the commands
        /// </summary>
        /// <param name="batchId"></param>
        public void PairTransactionWithCommand(long batchId)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSSSettlementRequestTransactionConfigCommand)} " +
                $"({nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommand.Batch)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommand.Request)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommand.State)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommand.LGA)}_Id, {nameof(PSSSettlementRequestTransactionConfigCommand.CreatedAtUtc)}, {nameof(PSSSettlementRequestTransactionConfigCommand.UpdatedAtUtc)}) " +
                $"SELECT sct.{nameof(PSSServiceSettlementConfigurationTransaction.TransactionLog)}_Id, sct.{nameof(PSSServiceSettlementConfigurationTransaction.Id)}, rqc.{nameof(RequestCommand.Command)}_Id, :batchId, rqc.{nameof(RequestCommand.Id)}, sct.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id, cmd.{nameof(Command.State)}_Id, cmd.{nameof(Command.LGA)}_Id, GETDATE(), GETDATE() " +
                $"FROM Parkway_CBS_Police_Core_{nameof(PSSServiceSettlementConfigurationTransaction)} sct " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(RequestCommand)} rqc " +
                $"ON rqc.{nameof(RequestCommand.Request)}_Id = sct.{nameof(PSSServiceSettlementConfigurationTransaction.Request)}_Id " +
                $"INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} cmd " +
                $"ON cmd.{nameof(Command.Id)} = rqc.{nameof(RequestCommand.Command)}_Id " +
                $"WHERE sct.{nameof(PSSServiceSettlementConfigurationTransaction.Batch)}_Id = :batchId ;";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("batchId", batchId);
            query.ExecuteUpdate();
        }

    }
}
