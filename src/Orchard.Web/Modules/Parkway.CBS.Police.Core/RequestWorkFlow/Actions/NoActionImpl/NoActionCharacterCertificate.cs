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
    public class NoActionCharacterCertificate : IServiceNoActionImpl
    {
        public PSSServiceTypeDefinition GetServiceType => PSSServiceTypeDefinition.CharacterCertificate;
        private readonly ICoreCharacterCertificateService _coreCharacterCertificateService;
        public ILogger Logger { get; set; }
        public NoActionCharacterCertificate(ICoreCharacterCertificateService coreCharacterCertificateService)
        {
            _coreCharacterCertificateService = coreCharacterCertificateService;
            Logger = NullLogger.Instance;
        }


        public RequestFlowVM DoServiceImplementationWorkForNoAction(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, string approvalNumber)
        {
            return GenerateCertificate(requestDeets, approvalNumber);
        }

        /// <summary>
        /// Generates and saves character certificate
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="approvalNumber"></param>
        /// <returns></returns>
        public RequestFlowVM GenerateCertificate(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, string approvalNumber)
        {
            try
            {
                _coreCharacterCertificateService.CreateAndSaveCertificateDocument(requestDeets.ElementAt(0).Request.FileRefNumber);

                return new RequestFlowVM { Message = "Character Certificate has been generated and saved successfully." };

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}