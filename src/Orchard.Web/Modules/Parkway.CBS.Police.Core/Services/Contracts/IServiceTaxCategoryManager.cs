using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Services.Contracts;


namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IServiceTaxCategoryManager<ServiceTaxCategory> : IDependency, IBaseManager<ServiceTaxCategory>
    {

        /// <summary>
        /// Get the list of PSS services
        /// </summary>
        /// <returns>IEnumerable<PSSRequestTypeVM></returns>
        IEnumerable<PSSRequestTypeVM> GetAllServices();


        /// <summary>
        /// Get the list of active PSS services
        /// </summary>
        /// <param name="categoryId">Category Id</param>
        /// <returns>IEnumerable<PSSRequestTypeVM></returns>
        IEnumerable<PSSRequestTypeVM> GetAllActiveServices(int categoryId);


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
        /// Get service type
        /// </summary>
        /// <param name="id">ID of the service</param>
        /// <param name="categoryId">categoryId of the tax payer</param>
        /// <returns>PSServiceVM</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        PSServiceVM GetServiceType(int id, int categoryId);


        /// <summary>
        /// Get flow definition Id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>int</returns>
        int GetFlowDefinitionId(int serviceId);


    }

}
