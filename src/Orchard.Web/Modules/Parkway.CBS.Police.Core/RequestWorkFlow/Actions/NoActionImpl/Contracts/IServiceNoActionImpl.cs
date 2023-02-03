using Orchard;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.NoActionImpl.Contracts
{
    public interface IServiceNoActionImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceType { get; }


        RequestFlowVM DoServiceImplementationWorkForNoAction(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, string approvalNumber);

    }
}
