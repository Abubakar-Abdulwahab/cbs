using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IRequestCommandWorkFlowLogManager<RequestCommandWorkFlowLog> : IDependency, IBaseManager<RequestCommandWorkFlowLog>
    {
        /// <summary>
        /// Updates the request command workflow log
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="commandId"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        bool UpdateRequestCommandWorkFlowLog(long requestId, int commandId, int flowDefinitionLevelId, bool isActive);

        /// <summary>
        /// Sets all request command workflow logs for request with the specified id to inactive
        /// </summary>
        /// <param name="requestId"></param>
        void SetPreviousRequestCommandWorkflowLogsToInactive(long requestId);

        /// <summary>
        /// Adds request command workflow log
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="commandId"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns>bool</returns>
        bool AddRequestCommandWorkflowLog(long requestId, int commandId, int flowDefinitionLevelId);

        /// <summary>
        /// Get request command Id by file number and flow definition level id
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns>int</returns>
        int GetRequestCommandId(string fileNumber, int flowDefinitionLevelId);

    }
}
