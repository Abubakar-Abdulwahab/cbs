using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class AccountPaymentRequestManager : BaseManager<AccountPaymentRequest>, IAccountPaymentRequestManager<AccountPaymentRequest>
    {
        private readonly ITransactionManager _transactionManager;
        public AccountPaymentRequestManager(IRepository<AccountPaymentRequest> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
        }

        public AccountPaymentRequestVM GetWalletPaymentDetailByPaymentId(string paymentId)
        {
            return _transactionManager.GetSession().Query<AccountPaymentRequest>().Where(x => x.PaymentReference == paymentId).Select(x => new AccountPaymentRequestVM
            {
                Id = x.Id,
                FlowDefinitionLevelId = x.FlowDefinitionLevel.Id,
                FlowDefinitionLevelPosition = x.FlowDefinitionLevel.Position,
                FlowDefinitionId = x.FlowDefinitionLevel.Definition.Id,
                PaymentRequestStatus = x.PaymentRequestStatus,
                TotalAmount = x.AccountPaymentRequestItems.Sum(s => s.Amount),
                SourceAccount = x.AccountName

            }).FirstOrDefault();
        }

        /// <summary>
        /// Get details for sending request to the settlement engine
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public AccountPaymentSettlementRequestVM GetWalletPaymentDetailForSettlementByPaymentId(string paymentId)
        {
            return _transactionManager.GetSession().Query<AccountPaymentRequest>().Where(x => x.PaymentReference == paymentId).Select(x => new AccountPaymentSettlementRequestVM
            {
                Id = x.Id,
                SourceAccountNumber = x.AccountNumber,
                BankCode = x.Bank.Code,
                FlowDefinitionLevelPosition = x.FlowDefinitionLevel.Position,
                FlowDefinitionId = x.FlowDefinitionLevel.Definition.Id
            }).FirstOrDefault();
        }

        /// <summary>
        /// Gets the payment reference 
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <returns></returns>
        public string GetWalletPaymentReference(long paymentRequestId)
        {
            return _transactionManager.GetSession().Query<AccountPaymentRequest>().Where(x => x.Id == paymentRequestId).Select(x => x.PaymentReference).FirstOrDefault();
        }

        /// <summary>
        /// Gets the view model for the approval detail view
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public WalletPaymentRequestApprovalDetailVM GetWalletPaymentRequestApprovalDetailVM(string paymentId)
        {

            System.Collections.Generic.IEnumerable<AccountWalletPaymentRequestItemDetailVM> accountWalletPaymentRequestDetails = _transactionManager.GetSession().Query<AccountPaymentRequestItem>().Where(x => x.AccountPaymentRequest.PaymentReference == paymentId).Select(n => new AccountWalletPaymentRequestItemDetailVM
            {
                AccountName = n.AccountName,
                AccountNumber = n.AccountNumber,
                Amount = n.Amount,
                Bank = n.Bank.Name,
                BeneficiaryName = n.BeneficiaryName,
                ExpenditureHeadName = n.PSSExpenditureHead.Name
            }).ToFuture();

            WalletPaymentRequestApprovalDetailVM requestApprovalDetailVM = _transactionManager.GetSession().Query<AccountPaymentRequest>().Where(x => x.PaymentReference == paymentId).Select(x => new WalletPaymentRequestApprovalDetailVM
            {
                PaymentId = x.PaymentReference,
                ApprovalButtonName = x.FlowDefinitionLevel.ApprovalButtonName,
                DateInitiated = x.CreatedAtUtc,
                SourceAccount = x.AccountWalletConfiguration.CommandWalletDetail.Command.Name ?? x.AccountWalletConfiguration.PSSFeeParty.Name,
                SourceAccountNumber = x.AccountWalletConfiguration.CommandWalletDetail.AccountNumber ?? x.AccountWalletConfiguration.PSSFeeParty.AccountNumber,
                NoOfBeneficiaries = x.AccountPaymentRequestItems.Count(),
                TotalAmountWalletPaymentApprovalRequestRecord = x.AccountPaymentRequestItems.Sum(b => b.Amount),

            }).ToFuture().SingleOrDefault();

            requestApprovalDetailVM.WalletPaymentRequestItemDetails = new System.Collections.Generic.List<AccountWalletPaymentRequestItemDetailVM>();
            requestApprovalDetailVM.WalletPaymentRequestItemDetails.AddRange(accountWalletPaymentRequestDetails);

            return requestApprovalDetailVM;
        }

        /// <summary>
        /// Moves the payment to next flow definition level
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="newDefinitionLevelId"></param>
        /// <param name="status"></param>
        public void UpdatePaymentRequestFlowId(long paymentRequestId, int newDefinitionLevelId, PaymentRequestStatus status)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(AccountPaymentRequest).Name;
                string statusName = nameof(AccountPaymentRequest.PaymentRequestStatus);
                string updatedAtName = nameof(AccountPaymentRequest.UpdatedAtUtc);
                string requestIdName = nameof(AccountPaymentRequest.Id);
                string flowDefIdName = nameof(AccountPaymentRequest.FlowDefinitionLevel) + "_Id";

                var queryText = $"UPDATE APR SET APR.{statusName} = :statusVal, APR.{updatedAtName} = :updateDate, APR.{flowDefIdName} = :flowDefId FROM {tableName} APR WHERE {requestIdName} = :paymentRequestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("statusVal", (int)status);
                query.SetParameter("paymentRequestId", paymentRequestId);
                query.SetParameter("flowDefId", newDefinitionLevelId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating payment request id {0}, definition level Id {1}, Exception message {2}", paymentRequestId, newDefinitionLevelId, exception.Message));
                throw;
            }
        }
    }
}