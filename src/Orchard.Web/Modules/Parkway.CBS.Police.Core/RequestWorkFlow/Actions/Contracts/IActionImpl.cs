using System;
using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.Contracts
{
    public interface IActionImpl : IDependency
    {
        RequestDirection GetRequestDirection { get; }


        RequestFlowVM MoveToNextDefinitionLevel(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel);

    }
}
