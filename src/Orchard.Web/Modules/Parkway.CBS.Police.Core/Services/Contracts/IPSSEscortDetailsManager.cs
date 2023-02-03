using Orchard;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSEscortDetailsManager<PSSEscortDetails> : IDependency, IBaseManager<PSSEscortDetails>
    {

        /// <summary>
        /// Get escort details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>EscortDetailsDTO</returns>
        EscortDetailsDTO GetEscortDetails(long requestId);


        /// <summary>
        /// Gets command type for escort request with specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        int GetCommandType(long requestId);


        /// <summary>
        /// Set the assigned officers value to true
        /// </summary>
        /// <param name="escortDetailsId"></param>
        /// <exception cref="Exception">throw exception if no record found</exception>
        void SetAssignedOfficersValueToTrue(long escortDetailsId);


        /// <summary>
        /// Get escort details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>EscortRequestDetailsVM</returns>
        EscortRequestDetailsVM GetEscortDetailsVM(long requestId);

        /// <summary>
        /// Get escort details
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>EscortDetailsDTO</returns>
        EscortDetailsDTO GetEscortDetailsVM(string fileNumber);


        /// <summary>
        /// Get escort request view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable<EscortDetailsVM></returns>
        IEnumerable<EscortDetailsVM> GetEscortRequestViewDetails(string fileRefNumber, long taxEntityId);


        /// <summary>
        /// Get escort document info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        IEnumerable<EscortDetailsVM> GetEscortDocumentInfo(long requestId);
    }    
}
