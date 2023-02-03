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
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class DeploymentAllowancePaymentRequestManager : BaseManager<DeploymentAllowancePaymentRequest>, IDeploymentAllowancePaymentRequestManager<DeploymentAllowancePaymentRequest>
    {
        private readonly IRepository<DeploymentAllowancePaymentRequest> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public DeploymentAllowancePaymentRequestManager(IRepository<DeploymentAllowancePaymentRequest> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets the payment reference 
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <returns></returns>
        public string GetWalletPaymentReference(long paymentRequestId)
        {
            return _transactionManager.GetSession().Query<DeploymentAllowancePaymentRequest>().Where(x => x.Id == paymentRequestId).Select(x => x.PaymentReference).SingleOrDefault();
        }


        /// <summary>
        /// Gets deployment allowance payment request details with specified payment reference for settlement engine api call
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <returns></returns>
        public DeploymentAllowancePaymentRequestDTO GetDeploymentAllowancePaymentRequestDetailsWithPaymentRefForSettlement(string paymentRef)
        {
            try
            {
                return _transactionManager.GetSession().Query<DeploymentAllowancePaymentRequest>().Where(x => x.PaymentReference == paymentRef).Select(x => new DeploymentAllowancePaymentRequestDTO
                {
                    Id = x.Id,
                    AccountName = x.AccountName,
                    AccountNumber = x.AccountNumber,
                    Bank = new CBS.Core.HelperModels.BankVM
                    {
                        Code = x.Bank.Code
                    },
                    PaymentReference = x.PaymentReference,
                    PSServiceRequestFlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevelDTO
                    {
                        Position = x.FlowDefinitionLevel.Position,
                        DefinitionId = x.FlowDefinitionLevel.Definition.Id
                    }
                }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
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
                string tableName = "Parkway_CBS_Police_Core_" + typeof(DeploymentAllowancePaymentRequest).Name;

                var queryText = $"UPDATE DAR SET DAR.{nameof(DeploymentAllowancePaymentRequest.PaymentRequestStatus)} = :statusVal, DAR.{nameof(DeploymentAllowancePaymentRequest.UpdatedAtUtc)} = :updateDate, DAR.{nameof(DeploymentAllowancePaymentRequest.FlowDefinitionLevel)}_Id = :flowDefId FROM {tableName} DAR WHERE DAR.{nameof(DeploymentAllowancePaymentRequest.Id)} = :paymentRequestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("statusVal", (int)status);
                query.SetParameter("paymentRequestId", paymentRequestId);
                query.SetParameter("flowDefId", newDefinitionLevelId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating deployment allowance payment request id {0}, definition level Id {1}, Exception message {2}", paymentRequestId, newDefinitionLevelId, exception.Message));
                throw;
            }
        }
    }
}