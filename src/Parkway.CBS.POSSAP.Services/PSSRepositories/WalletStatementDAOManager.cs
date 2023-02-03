using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class WalletStatementDAOManager : Repository<WalletStatement>, IWalletStatementDAOManager
    {
        public WalletStatementDAOManager(IUoW uow) : base(uow)
        { }



        /// <summary>
        /// Moves wallet statements with specified reference from staging table to main table
        /// </summary>
        /// <param name="reference"></param>
        public void MoveStatementsFromStagingToMain(string reference)
        {
            var queryText = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(WalletStatement)} ({nameof(WalletStatement.WalletId)}, {nameof(WalletStatement.WalletIdentifierType)}, " +
                $"{nameof(WalletStatement.TransactionTypeId)}, {nameof(WalletStatement.Narration)}, {nameof(WalletStatement.TransactionReference)}, {nameof(WalletStatement.Amount)}, " +
                $"{nameof(WalletStatement.TransactionDate)}, {nameof(WalletStatement.ValueDate)}, {nameof(WalletStatement.CreatedAtUtc)}, {nameof(WalletStatement.UpdatedAtUtc)}) " +
                $"SELECT {nameof(WalletStatementStaging.WalletId)}, {nameof(WalletStatementStaging.WalletIdentifierType)}, {nameof(WalletStatementStaging.TransactionTypeId)}, " +
                $"{nameof(WalletStatementStaging.Narration)}, {nameof(WalletStatementStaging.TransactionReference)}, {nameof(WalletStatementStaging.Amount)}, " +
                $"{nameof(WalletStatementStaging.TransactionDate)}, {nameof(WalletStatementStaging.ValueDate)}, GETDATE(), GETDATE() FROM Parkway_CBS_Police_Core_{nameof(WalletStatementStaging)} " +
                $"WHERE {nameof(WalletStatementStaging.Reference)} = :reference ";

            var query = _uow.Session.CreateSQLQuery(queryText);
            query.SetParameter("reference", reference);
            query.ExecuteUpdate();
        }
    }
}
