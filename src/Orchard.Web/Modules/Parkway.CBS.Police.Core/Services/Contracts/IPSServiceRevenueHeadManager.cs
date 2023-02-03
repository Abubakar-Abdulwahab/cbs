using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSServiceRevenueHeadManager<PSServiceRevenueHead> : IDependency, IBaseManager<PSServiceRevenueHead>
    {

        /// <summary>
        /// Get service revenuehead details for request
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="isApplicationInvoice"></param>
        /// <returns>IEnumerable<PSServiceRevenueHeadVM></returns>
        IEnumerable<PSServiceRevenueHeadVM> GetRevenueHead(int serviceId, int appStage);


        /// <summary>
        /// Get the revenue head details and the corresponding forms assigned to the revenue heads
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="appStage"><see cref="Models.Enums.PSSRevenueServiceStep"/></param>
        /// <returns>IEnumerable{PSServiceRevenueHeadVM}</returns>
        IEnumerable<PSServiceRevenueHeadVM> GetRevenueHeadAndFormDetails(int serviceId, int appStage);

    }
}
