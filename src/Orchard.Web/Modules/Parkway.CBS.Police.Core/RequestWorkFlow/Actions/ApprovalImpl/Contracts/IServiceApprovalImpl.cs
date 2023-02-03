using Orchard;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.ApprovalImpl.Contracts
{
    public interface IServiceApprovalImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceType { get; }

        /// <summary>
        /// Do extra work specific to a particular service
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="nextDefinedLevel"></param>
        void DoServiceImplementationWorkForApproval(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel);
    }
}
