using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.API.PSSServiceType.ServiceVerification.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Net;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class USSDVerificationHandler : IUSSDRequestTypeHandler
    {
        public ILogger Logger { get; set; }
        public USSDRequestType GetRequestType => USSDRequestType.Verification;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly IEnumerable<Lazy<IPSSServiceTypeUSSDVerificationImpl>> _pssServiceTypeUSSDVerificationImpl;
        private readonly Lazy<ITypeImplComposer> _typeImplComposer;

        public USSDVerificationHandler(Lazy<IPSSRequestManager<PSSRequest>> requestManager, IEnumerable<Lazy<IPSSServiceTypeUSSDVerificationImpl>> pssServiceTypeUSSDVerificationImpl, Lazy<ITypeImplComposer> typeImplComposer)
        {
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
            _pssServiceTypeUSSDVerificationImpl = pssServiceTypeUSSDVerificationImpl;
            _typeImplComposer = typeImplComposer;
        }

        /// <summary>
        /// Start USSD verification request processing
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        public USSDAPIResponse StartRequest(USSDRequestModel model)
        {
            try
            {
                string[] requestStage = model.Text.Split('|');
                if (requestStage.Length == (int)USSDProcessingStage.RequestType)
                {
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = FileNumberMenu() };
                }

                PSSRequestVM requestDet = _typeImplComposer.Value.ConfirmFileNumber(requestStage[1]);
                if (requestDet == null)
                {
                    throw new NoRecordFoundException("Request with the specified File Number not valid.");
                }

                foreach (var impl in _pssServiceTypeUSSDVerificationImpl)
                {
                    if (impl.Value.GetServiceTypeDefinition == (PSSServiceTypeDefinition)requestDet.ServiceTypeId)
                    {
                        return impl.Value.ProcessRequest(model);
                    }
                }

                throw new NoRecordFoundException("Service not found.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Prompt user to enter file number
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        private string FileNumberMenu()
        {
            return "Enter File Number\n"; ;
        }
    }
}