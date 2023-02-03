using Orchard.Logging;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.NoActionImpl.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.NoActionImpl
{
    public class NoActionExtract : IServiceNoActionImpl
    {
        public PSSServiceTypeDefinition GetServiceType => PSSServiceTypeDefinition.Extract;
        private readonly ICoreExtractService _coreExtractService;
        public ILogger Logger { get; set; }
        public NoActionExtract(ICoreExtractService coreExtractService)
        {
            _coreExtractService = coreExtractService;
            Logger = NullLogger.Instance;
        }


        public RequestFlowVM DoServiceImplementationWorkForNoAction(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, string approvalNumber)
        {
            return GenerateExtractDocument(requestDeets, approvalNumber);
        }

        /// <summary>
        /// Generates and saves extract document
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="approvalNumber"></param>
        /// <returns></returns>
        public RequestFlowVM GenerateExtractDocument(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, string approvalNumber)
        {
            try
            {
                _coreExtractService.CreateAndSaveExtractDocument(requestDeets.ElementAt(0).Request.FileRefNumber);

                return new RequestFlowVM { Message = "Extract has been generated and saved successfully." };

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}