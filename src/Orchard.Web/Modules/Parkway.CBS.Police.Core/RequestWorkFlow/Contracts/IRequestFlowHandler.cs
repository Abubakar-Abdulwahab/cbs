using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Contracts
{
    public interface IRequestFlowHandler : IDependency
    {

        /// <summary>
        /// Move request to next stage
        /// </summary>
        /// <param name="requestDeet"></param>
        /// <returns>RequestFlowVM</returns>
        RequestFlowVM MoveRequestToNextStage(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeet);

    }
}
