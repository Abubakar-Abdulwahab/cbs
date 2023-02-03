using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.GenerateInvoiceImpl.Contracts
{
    public interface IServiceGenerateInvoiceImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceType { get; }


        InvoiceGenerationResponse DoServiceImplementationWorkForGenerateInvoice(PSServiceRevenueHeadVM parentServicerevenueHead, IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads, IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets);


    }
}
