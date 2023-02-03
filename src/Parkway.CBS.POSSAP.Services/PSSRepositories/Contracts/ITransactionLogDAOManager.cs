using System;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface ITransactionLogDAOManager : IRepository<TransactionLog>
    {
        void MarkSettledTransaction(long batchId, DateTime settlementDate);
    }
}
