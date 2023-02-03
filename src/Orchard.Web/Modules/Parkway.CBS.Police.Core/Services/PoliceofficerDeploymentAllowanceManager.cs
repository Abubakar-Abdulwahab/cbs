using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Services
{
    public class PoliceofficerDeploymentAllowanceManager : BaseManager<PoliceofficerDeploymentAllowance>, IPoliceofficerDeploymentAllowanceManager<PoliceofficerDeploymentAllowance>
    {
        private readonly IRepository<PoliceofficerDeploymentAllowance> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public PoliceofficerDeploymentAllowanceManager(IRepository<PoliceofficerDeploymentAllowance> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get the deployment allowance view details using deployment allowance request id
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <returns>EscortDeploymentRequestDetailsVM</returns>
        public EscortDeploymentRequestDetailsVM GetRequestViewDetails(long deploymentAllowanceRequestId)
        {
            return _transactionManager.GetSession().Query<PoliceofficerDeploymentAllowance>().Where(x => x.Id == deploymentAllowanceRequestId)
                .Select(x => new EscortDeploymentRequestDetailsVM
                {
                    EscortInfo = new EscortRequestVM
                    {
                        StartDate = x.EscortDetails.StartDate.ToString("dd/MM/yyyy"),
                        EndDate = x.EscortDetails.EndDate.ToString("dd/MM/yyyy"),
                        DurationNumber = x.EscortDetails.DurationNumber
                    },
                    RequestId = x.Request.Id,
                    DeploymentAllowanceRequestId = deploymentAllowanceRequestId,
                    PoliceOfficerName = x.PoliceOfficerLog.Name,
                    AccountNumber = x.PoliceOfficerLog.AccountNumber,
                    BankCode = x.PoliceOfficerLog.BankCode,
                    ServiceNumber = x.PoliceOfficerLog.IdentificationNumber,
                    IPPISNumber = x.PoliceOfficerLog.IPPISNumber,
                    Amount = x.Amount,
                    AmountContributed = x.ContributedAmount,
                    Narration = x.Narration,
                    RankName = x.PoliceOfficerLog.Rank.ExternalDataCode,
                    InvoiceNumber = x.Invoice.InvoiceNumber
                }).FirstOrDefault();
        }

        /// <summary>
        /// Update the transaction status for a particular deployment allowance payment reference
        /// </summary>
        /// <param name="reference"></param>
        public void UpdateDeploymentAllowanceTransactionStatus(string reference, int status)
        {
            try
            {
                var tableName = "Parkway_CBS_Police_Core_" + typeof(PoliceofficerDeploymentAllowance).Name;
                var queryText = $"UPDATE {tableName} SET {nameof(PoliceofficerDeploymentAllowance.Status)} = {status}, {nameof(PoliceofficerDeploymentAllowance.UpdatedAtUtc)} = :updateDate WHERE {nameof(PoliceofficerDeploymentAllowance.SettlementReferenceNumber)} = '{reference}'";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Error {0}, Exception message {1}", reference, exception.Message));
                throw;
            }
        }
    }
}