using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.ReadyCashService;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreReadyCashService : ICoreReadyCashService
    {
        public ILogger Logger { get; set; }

        public CoreReadyCashService()
        {
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get customer account balance
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <returns>decimal</returns>
        public decimal GetCustomerAccountBalance(string walletIdentifier)
        {
            try
            {
                ThirdpartyServiceClient thirdpartyServiceClient = new ThirdpartyServiceClient();
                return thirdpartyServiceClient.getAgentBalance(walletIdentifier);
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (AggregateException)
            {
                throw;
            }
            catch (Exception exception) { Logger.Error(exception, $"Unable to retrieve account balance for {walletIdentifier}. Exception message --- {exception.Message}"); throw; }
        }


        /// <summary>
        /// Get wallet account statement
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns>List<AccountStatementItemModel></returns>
        public List<AccountStatementItemModel> GetCustomerAccountTransactions(string walletIdentifier, DateTime startDate, DateTime endDate, int pageSize, int pageIndex)
        {
            List<AccountStatementItemModel>  statementItems = new List<AccountStatementItemModel>();
            string froDate = string.Format("{0:dd-MMM-yyyy}", startDate).ToUpper();
            string toDate = string.Format("{0:dd-MMM-yyyy}", endDate).ToUpper();

            try
            {
                ThirdpartyServiceClient thirdpartyServiceClient = new ThirdpartyServiceClient();
                glEntry[] accountEntries = thirdpartyServiceClient.findAgentTransactions(walletIdentifier, startDate, endDate, pageIndex + 1, pageSize);
                if (accountEntries != null)
                {
                    for (int count = 0; count < accountEntries.Length; count++)
                    {
                        AccountStatementItemModel item = new AccountStatementItemModel();
                        var rec = accountEntries[count];
                        item.Description = rec.detail;
                        item.ReferenceNo = rec.id.ToString();
                        item.TransactionDate = rec.transaction.timestamp;
                        item.ValueDate = rec.transaction.timestamp;
                        item.TransAmount = rec.amount;
                        if (rec.credit)
                        {
                            item.TransactionType = TransactionType.Credit;
                        }
                        else if (rec.debit)
                        {
                            item.TransactionType = TransactionType.Debit;
                        }
                        statementItems.Add(item);
                    }
                }

                return statementItems;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Get an agent account balance
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <returns>decimal</returns>
        public decimal GetAgentAccountBalance(string walletIdentifier)
        {
            try
            {
                ThirdpartyServiceClient thirdpartyServiceClient = new ThirdpartyServiceClient();
                return thirdpartyServiceClient.getAgentBalance(walletIdentifier);
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (AggregateException)
            {
                throw;
            }
            catch (Exception exception) { Logger.Error(exception, $"Unable to retrieve account balance for {walletIdentifier}. Exception message --- {exception.Message}"); throw; }
        }

        /// <summary>
        /// Get an agent account statement
        /// </summary>
        /// <param name="walletIdentifier"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns>List<AccountStatementItemModel></returns>
        public List<AccountStatementItemModel> GetAgentAccountTransactions(string walletIdentifier, DateTime startDate, DateTime endDate, int pageSize, int pageIndex)
        {
            List<AccountStatementItemModel> statementItems = new List<AccountStatementItemModel>();
            string froDate = string.Format("{0:dd-MMM-yyyy}", startDate).ToUpper();
            string toDate = string.Format("{0:dd-MMM-yyyy}", endDate).ToUpper();

            try
            {
                ThirdpartyServiceClient thirdpartyServiceClient = new ThirdpartyServiceClient();
                glEntry[] accountEntries = thirdpartyServiceClient.findAgentTransactions(walletIdentifier, startDate, endDate, pageIndex + 1, pageSize);
                if (accountEntries != null)
                {
                    for (int count = 0; count < accountEntries.Length; count++)
                    {
                        AccountStatementItemModel item = new AccountStatementItemModel();
                        var rec = accountEntries[count];
                        item.Description = rec.detail;
                        item.ReferenceNo = rec.id.ToString();
                        item.TransactionDate = rec.transaction.timestamp;
                        item.ValueDate = rec.transaction.timestamp;
                        item.TransAmount = rec.amount;
                        if (rec.credit)
                        {
                            item.TransactionType = TransactionType.Credit;
                        }
                        else if (rec.debit)
                        {
                            item.TransactionType = TransactionType.Debit;
                        }
                        statementItems.Add(item);
                    }
                }

                return statementItems;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}