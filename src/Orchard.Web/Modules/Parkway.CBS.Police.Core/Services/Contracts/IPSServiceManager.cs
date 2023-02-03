using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSServiceManager<PSService> : IDependency, IBaseManager<PSService>
    {
        /// <summary>
        /// Get id of service with specified service type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        PSServiceVM GetServiceWithServiceType(Models.Enums.PSSServiceTypeDefinition serviceType);


        /// <summary>
        /// Gets services 
        /// </summary>
        /// <returns></returns>
        IEnumerable<PSServiceVM> GetServices();


        /// <summary>
        /// Checks if the <paramref name="serviceId"/> exist
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        bool CheckIfServiceIdExist(int serviceId);

        /// <summary>
        /// Get the list of PSS services
        /// </summary>
        /// <returns>IEnumerable<PSSRequestTypeVM></returns>
        IEnumerable<PSSRequestTypeVM> GetAllServices();


        /// <summary>
        /// Get the first level defintiion Id
        /// <para>this method get the level Id attached to this service, it is used to 
        /// determine what direction and revenue heads to use for invoice generation</para>
        /// </summary>
        /// <param name="serviceId"></param>
        int GetFirstLevelDefinitionId(int serviceId);


        /// <summary>
        /// Get the first level defintiion
        /// <para>this method get the level attached to this service, it is used to 
        /// determine what direction and revenue heads to use for invoice generation. Return only the Id and the Postion description</para>
        /// </summary>
        /// <param name="serviceId"></param>
        PSServiceRequestFlowDefinitionLevelDTO GetFirstLevelDefinition(int serviceId);


        /// <summary>
        /// Get the last level defintiion with the specified workflow action value
        /// <para>this method get the level attached to this service, it is used to 
        /// determine what direction and revenue heads to use for invoice generation. Return only the Id and the Postion description</para>
        /// </summary>
        /// <param name="serviceId"></param>
        PSServiceRequestFlowDefinitionLevelDTO GetLastLevelDefinitionWithWorkflowActionValue(int serviceId, Models.Enums.RequestDirection actionValue);


        /// <summary>
        /// Gets services with no differential workflow
        /// </summary>
        /// <returns></returns>
        IEnumerable<PSServiceVM> GetServicesWithNoDifferentialWorkflow();


        /// <summary>
        /// Gets flow definition for service type with specified id
        /// </summary>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        IEnumerable<PSServiceRequestFlowDefinitionDTO> GetFlowDefinitionForServiceType(int serviceTypeId);
    }

}
