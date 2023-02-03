using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IServiceWorkflowDifferentialManager<ServiceWorkflowDifferential> : IDependency, IBaseManager<ServiceWorkflowDifferential>
    {

        /// <summary>
        /// Get the differential first level definitionId
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differentialVM">ServiceWorkFlowDifferentialDataParam</param>
        /// <returns>int</returns>
        int GetFirstLevelDefinitionId(int serviceId, ServiceWorkFlowDifferentialDataParam differentialVM);


        /// <summary>
        /// Get the work flow definition for based off the differential data param
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differential"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        PSServiceRequestFlowDefinitionLevelDTO GetFirstLevelDefinitionObj(int serviceId, ServiceWorkFlowDifferentialDataParam differential);


        /// <summary>
        /// Get the last work flow definition with specified workflow action value based off the differential data param
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differential"></param>
        /// <returns>PSServiceRequestFlowDefinitionLevelDTO</returns>
        PSServiceRequestFlowDefinitionLevelDTO GetLastFlowDefinitionLevelObjWithWorkflowActionValue(int serviceId, ServiceWorkFlowDifferentialDataParam differential, Models.Enums.RequestDirection actionValue);


        /// <summary>
        /// Gets flow definition for service type with specified id
        /// </summary>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        IEnumerable<PSServiceRequestFlowDefinitionDTO> GetFlowDefinitionForServiceType(int serviceTypeId);

    }

}
