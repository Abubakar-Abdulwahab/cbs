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
    public class DeploymentAllowancePaymentRequestItemManager : BaseManager<DeploymentAllowancePaymentRequestItem>, IDeploymentAllowancePaymentRequestItemManager<DeploymentAllowancePaymentRequestItem>
    {
        private readonly IRepository<DeploymentAllowancePaymentRequestItem> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public DeploymentAllowancePaymentRequestItemManager(IRepository<DeploymentAllowancePaymentRequestItem> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get deployment allowance payment request items for deployment allowance payment request with specified request invoice id and account wallet configuration with specified command id
        /// </summary>
        /// <param name="requestInvoiceId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public List<DeploymentAllowancePaymentRequestItemDTO> GetDeploymentAllowancePaymentRequestItemsForBatchWithRequestInvoiceId(long requestInvoiceId, int commandId)
        {
            try
            {
                return _transactionManager.GetSession().Query<DeploymentAllowancePaymentRequestItem>().Where(x => (x.DeploymentAllowancePaymentRequest.PSSRequestInvoice.Id == requestInvoiceId) && (x.TransactionStatus != (int)Models.Enums.PaymentRequestStatus.DECLINED) && (x.TransactionStatus != (int)Models.Enums.PaymentRequestStatus.FAILED) && (x.DeploymentAllowancePaymentRequest.AccountWalletConfiguration.CommandWalletDetail.Command.Id == commandId)).Select(x => new DeploymentAllowancePaymentRequestItemDTO
                {
                    Id = x.Id,
                    CommandTypeId = x.CommandType.Id,
                    DayTypeId = x.DayType.Id,
                    Amount = x.Amount,
                    DeploymentAllowancePaymentRequestId = x.DeploymentAllowancePaymentRequest.Id
                }).ToList();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets items in deployment allowance payment request with specified payment reference
        /// </summary>
        /// <param name="paymentRef">Deployment Allowance Payment Request Payment Reference</param>
        /// <returns>IEnumerable<DeploymentAllowancePaymentRequestItemDTO></returns>
        public IEnumerable<DeploymentAllowancePaymentRequestItemDTO> GetItemsInDeploymentAllowancePaymentRequestWithPaymentRef(string paymentRef)
        {
            try
            {
                return _transactionManager.GetSession().Query<DeploymentAllowancePaymentRequestItem>().Where(x => x.DeploymentAllowancePaymentRequest.PaymentReference == paymentRef).Select(x => new DeploymentAllowancePaymentRequestItemDTO
                {
                    PaymentReference = x.PaymentReference,
                    AccountName = x.AccountName,
                    AccountNumber = x.AccountNumber,
                    Amount = x.Amount,
                    CommandTypeName = x.CommandType.Name,
                    DayTypeName = x.DayType.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Bank = new CBS.Core.HelperModels.BankVM { Code = x.Bank.Code }
                });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Updates the <see cref="DeploymentAllowancePaymentRequestItem.TransactionStatus"/> using <paramref name="status"/>
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="status"></param>
        public void UpdatePaymentRequestStatusId(long paymentRequestId, PaymentRequestStatus status)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(DeploymentAllowancePaymentRequestItem).Name;

                var queryText = $"UPDATE DAR SET DAR.{nameof(DeploymentAllowancePaymentRequestItem.TransactionStatus)} = :statusVal, DAR.{nameof(DeploymentAllowancePaymentRequestItem.UpdatedAtUtc)} = :updateDate FROM {tableName} DAR WHERE DAR.{nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest)}_Id = :paymentRequestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("statusVal", (int)status);
                query.SetParameter("paymentRequestId", paymentRequestId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating deployment allowance payment request item payment reference {0}, status {1}, Exception message {2}", paymentRequestId, exception.Message));
                throw;
            }
        }
    }
}