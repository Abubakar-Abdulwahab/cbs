using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services
{
    public class RequestStatusLogManager : BaseManager<RequestStatusLog>, IRequestStatusLogManager<RequestStatusLog>
    {
        private readonly IRepository<RequestStatusLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public RequestStatusLogManager(IRepository<RequestStatusLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }



        /// <summary>
        /// When payment has been confirmed we need to set the fulfilled flag to true
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="definitionLevelId"></param>
        /// <param name="invoiceId"></param>
        public void UpdateStatusToFulfilledAfterPayment(long requestId, int definitionLevelId, long invoiceId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(RequestStatusLog).Name;
                string fulfilledName = nameof(RequestStatusLog.Fulfilled);
                string updatedAtName = nameof(RequestStatusLog.UpdatedAtUtc);
                string requestIdName = nameof(RequestStatusLog.Request) + "_Id";
                string invoiceIdName = nameof(RequestStatusLog.Invoice) + "_Id";
                string flowDefIdName = nameof(RequestStatusLog.FlowDefinitionLevel) + "_Id";

                var queryText = $"UPDATE prsl SET prsl.{fulfilledName} = :approvedVal, prsl.{updatedAtName} = :updateDate FROM {tableName} prsl WHERE {requestIdName} = :requestId AND {invoiceIdName} = :invoiceId AND {flowDefIdName} = :flowDefId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("approvedVal", true);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("requestId", requestId);
                query.SetParameter("invoiceId", invoiceId);
                query.SetParameter("flowDefId", definitionLevelId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// When payment has been confirmed we need to set the fulfilled flag to true
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceId"></param>
        public void UpdateStatusToFulfilledAfterPayment(long requestId, long invoiceId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(RequestStatusLog).Name;
                string fulfilledName = nameof(RequestStatusLog.Fulfilled);
                string updatedAtName = nameof(RequestStatusLog.UpdatedAtUtc);
                string requestIdName = nameof(RequestStatusLog.Request) + "_Id";
                string invoiceIdName = nameof(RequestStatusLog.Invoice) + "_Id";

                var queryText = $"UPDATE prsl SET prsl.{fulfilledName} = :approvedVal, prsl.{updatedAtName} = :updateDate FROM {tableName} prsl WHERE {requestIdName} = :requestId AND {invoiceIdName} = :invoiceId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("approvedVal", true);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("requestId", requestId);
                query.SetParameter("invoiceId", invoiceId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Get list of request status log VMs for request with specified Id
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public List<PSSRequestStatusLogVM> GetRequestStatusLogVMById(long reqId)
        {
            return _transactionManager.GetSession().Query<RequestStatusLog>().OrderByDescending(x => x.Id).Where(log => log.Request == new PSSRequest { Id = reqId }).Select(log => new PSSRequestStatusLogVM
            {
                Id = log.Id,
                StatusDescription = log.StatusDescription,
                Invoice = new RequestInvoiceVM { InvoiceNumber = log.Invoice.InvoiceNumber, InvoiceUrl = log.Invoice.InvoiceURL },
                Status = log.Status,
                UserActionRequired = log.UserActionRequired,
                Fulfilled = log.Fulfilled,
                PositionName = log.FlowDefinitionLevel.PositionName,
                Position = log.FlowDefinitionLevel.Position,
                UpdatedAt = log.UpdatedAtUtc,
                DefinitionId = log.FlowDefinitionLevel.Definition.Id,
                WorkFlowActionValue = log.FlowDefinitionLevel.WorkFlowActionValue
            }).ToList();
        }



        /// <summary>
        /// Get list of request status log VMs for request with specified fileRefNumber
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public IEnumerable<PSSRequestStatusLogVM> GetRequestStatusLogVMByFileRefNumber(string fileRefNumber)
        {
            return _transactionManager.GetSession().Query<RequestStatusLog>().OrderByDescending(x => x.Id).Where(log => log.Request.FileRefNumber ==  fileRefNumber).Select(log => new PSSRequestStatusLogVM
            {
                Id = log.Id,
                StatusDescription = log.StatusDescription,
                Invoice = new RequestInvoiceVM { InvoiceNumber = log.Invoice.InvoiceNumber, InvoiceUrl = log.Invoice.InvoiceURL },
                Status = log.Status,
                UserActionRequired = log.UserActionRequired,
                Fulfilled = log.Fulfilled,
                PositionName = log.FlowDefinitionLevel.PositionName,
                Position = log.FlowDefinitionLevel.Position,
                UpdatedAt = log.UpdatedAtUtc,
                DefinitionId = log.FlowDefinitionLevel.Definition.Id,
                WorkFlowActionValue = log.FlowDefinitionLevel.WorkFlowActionValue
            }).ToFuture();
        }

    }
}