using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class RequestCommandWorkFlowLogManager : BaseManager<RequestCommandWorkFlowLog>, IRequestCommandWorkFlowLogManager<RequestCommandWorkFlowLog>
    {
        private readonly IRepository<RequestCommandWorkFlowLog> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public RequestCommandWorkFlowLogManager(IRepository<RequestCommandWorkFlowLog> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Updates the request command workflow log
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="commandId"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public bool UpdateRequestCommandWorkFlowLog(long requestId, int commandId, int flowDefinitionLevelId, bool isActive)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(RequestCommandWorkFlowLog).Name;
                string queryString = $"UPDATE {tableName} SET {nameof(RequestCommandWorkFlowLog.IsActive)} = :isActive, {nameof(RequestCommandWorkFlowLog.UpdatedAtUtc)} = GETDATE(), {nameof(RequestCommandWorkFlowLog.RequestPhaseId)} = :requestPhaseId, {nameof(RequestCommandWorkFlowLog.RequestPhaseName)} = :requestPhaseName WHERE {nameof(RequestCommandWorkFlowLog.Request)}_Id = :requestId AND {nameof(RequestCommandWorkFlowLog.DefinitionLevel)}_Id = :definitionLevelId AND {nameof(RequestCommandWorkFlowLog.Command)}_Id = :commandId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
                query.SetParameter("requestId", requestId);
                query.SetParameter("definitionLevelId", flowDefinitionLevelId);
                query.SetParameter("commandId", commandId);
                query.SetParameter("isActive", isActive);
                query.SetParameter("requestPhaseId", (int)RequestPhase.Ongoing);
                query.SetParameter("requestPhaseName", nameof(RequestPhase.Ongoing));
                query.ExecuteUpdate();
                return true;

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return false;
        }


        /// <summary>
        /// Sets all request command workflow logs for request with the specified id to inactive
        /// </summary>
        /// <param name="requestId"></param>
        public void SetPreviousRequestCommandWorkflowLogsToInactive(long requestId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(RequestCommandWorkFlowLog).Name;
                string queryString = $"UPDATE {tableName} SET {nameof(RequestCommandWorkFlowLog.IsActive)} = :isActive, {nameof(RequestCommandWorkFlowLog.UpdatedAtUtc)} = GETDATE(), {nameof(RequestCommandWorkFlowLog.RequestPhaseId)} = :requestPhaseId, {nameof(RequestCommandWorkFlowLog.RequestPhaseName)} = :requestPhaseName WHERE {nameof(RequestCommandWorkFlowLog.Request)}_Id = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
                query.SetParameter("requestId", requestId);
                query.SetParameter("isActive", false);
                query.SetParameter("requestPhaseId", (int)RequestPhase.Ongoing);
                query.SetParameter("requestPhaseName", nameof(RequestPhase.Ongoing));
                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, $"Unable to set previous request command workflow logs to inactive for request with id {requestId}. Exception message - {exception.Message}");
                throw;
            }
        }


        /// <summary>
        /// Adds request command workflow log
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="commandId"></param>
        /// <param name="flowDefinitionLevelId"></param>
        public bool AddRequestCommandWorkflowLog(long requestId, int commandId, int flowDefinitionLevelId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(RequestCommandWorkFlowLog).Name;
                string queryString = $"INSERT INTO {tableName}({nameof(RequestCommandWorkFlowLog.Request)}_Id, {nameof(RequestCommandWorkFlowLog.Command)}_Id, {nameof(RequestCommandWorkFlowLog.DefinitionLevel)}_Id, {nameof(RequestCommandWorkFlowLog.IsActive)}, {nameof(RequestCommandWorkFlowLog.CreatedAtUtc)}, {nameof(RequestCommandWorkFlowLog.UpdatedAtUtc)}, {nameof(RequestCommandWorkFlowLog.RequestPhaseId)}, {nameof(RequestCommandWorkFlowLog.RequestPhaseName)}) VALUES(:requestId, :commandId, :definitionLevelId, :isActive, GETDATE(), GETDATE(), :requestPhaseId, :requestPhaseName)";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
                query.SetParameter("requestId", requestId);
                query.SetParameter("definitionLevelId", flowDefinitionLevelId);
                query.SetParameter("commandId", commandId);
                query.SetParameter("isActive", true);
                query.SetParameter("requestPhaseId", (int)RequestPhase.New);
                query.SetParameter("requestPhaseName", nameof(RequestPhase.New));
                query.ExecuteUpdate();
                return true;
            }
            catch(Exception exception)
            {
                Logger.Error(exception, $"Error adding request command workflow log for request with id {requestId}, flow definition level with id {flowDefinitionLevelId} & command with id {commandId}. Exception message - {exception.Message}");
            }
            return false;
        }

        /// <summary>
        /// Get request command Id by file number and flow definition level id
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns>int</returns>
        public int GetRequestCommandId(string fileNumber, int flowDefinitionLevelId)
        {
            return _transactionManager.GetSession().Query<RequestCommandWorkFlowLog>().Where(req => req.Request.FileRefNumber == fileNumber && req.DefinitionLevel.Id == flowDefinitionLevelId && req.IsActive).Select(req => req.Command.Id).SingleOrDefault();
        }

    }
}