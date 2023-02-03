using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreReadyCashService : IDependency
    {
        /// <summary>
        /// Get customer account balance
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <returns>decimal</returns>
        decimal GetCustomerAccountBalance(string walletIdentifier);

        /// <summary>
        /// Get customer account statement
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns>List<AccountStatementItemModel></returns>
        List<AccountStatementItemModel> GetCustomerAccountTransactions(string walletIdentifier, DateTime startDate, DateTime endDate, int pageSize, int pageIndex);

        /// <summary>
        /// Get an agent account balance
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <returns>decimal</returns>
        decimal GetAgentAccountBalance(string walletIdentifier);

        /// <summary>
        /// Get an agent account statement
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns>List<AccountStatementItemModel></returns>
        List<AccountStatementItemModel> GetAgentAccountTransactions(string walletIdentifier, DateTime startDate, DateTime endDate, int pageSize, int pageIndex);
    }
}
