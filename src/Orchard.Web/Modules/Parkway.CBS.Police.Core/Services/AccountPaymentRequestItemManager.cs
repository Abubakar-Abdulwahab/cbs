using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class AccountPaymentRequestItemManager : BaseManager<AccountPaymentRequestItem>, IAccountPaymentRequestItemManager<AccountPaymentRequestItem>
    {
        private readonly ITransactionManager _transactionManager;
        public AccountPaymentRequestItemManager(IRepository<AccountPaymentRequestItem> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Get request items for processing
        /// </summary>
        /// <param name="paymentId"></param>
        public IEnumerable<AccountWalletPaymentRequestItemDTO> GetPaymentRequestItems(string paymentId)
        {
            return _transactionManager.GetSession().Query<AccountPaymentRequestItem>().Where(x => x.AccountPaymentRequest.PaymentReference == paymentId).Select(n => new AccountWalletPaymentRequestItemDTO
            {
                Id = n.Id,
                AccountName = n.AccountName,
                AccountNumber = n.AccountNumber,
                Amount = n.Amount,
                BankCode = n.Bank.Code,
                BeneficiaryName = n.BeneficiaryName,
                ExpenditureHeadName = n.PSSExpenditureHead.Name,
                PaymentReference = n.PaymentReference

            }).ToFuture();

        }

        /// <summary>
        /// Updates the <see cref="AccountPaymentRequestItem.TransactionStatus"/> using <paramref name="status"/>
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="status"></param>
        public void UpdatePaymentRequestStatusId(long paymentRequestId, PaymentRequestStatus status)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(AccountPaymentRequestItem).Name;
                string statusName = nameof(AccountPaymentRequestItem.TransactionStatus);
                string updatedAtName = nameof(AccountPaymentRequestItem.UpdatedAtUtc);
                string paymentRequestIdName = nameof(AccountPaymentRequestItem.AccountPaymentRequest) + "_Id";

                var queryText = $"UPDATE APR SET APR.{statusName} = :statusVal, APR.{updatedAtName} = :updateDate FROM {tableName} APR WHERE {paymentRequestIdName} = :paymentRequestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("statusVal", (int)status);
                query.SetParameter("paymentRequestId", paymentRequestId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating payment reference {0}, status {1}, Exception message {2}", paymentRequestId, exception.Message));
                throw;
            }
        }
    }
}