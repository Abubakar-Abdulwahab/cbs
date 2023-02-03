using Orchard.Logging;
using Parkway.CBS.Police.API.PSSServiceType.ServiceVerification.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Net;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;

namespace Parkway.CBS.Police.Core.PSSServiceType.ServiceVerification
{
    public class Extract : IPSSServiceTypeUSSDVerificationImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Extract;
        public ILogger Logger { get; set; }
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<ITypeImplComposer> _typeImplComposer;
        private const int RequestStatusOption = 1;

        public Extract(Lazy<IPSSRequestManager<PSSRequest>> requestManager, Lazy<ITypeImplComposer> typeImplComposer)
        {
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
            _typeImplComposer = typeImplComposer;
        }

        /// <summary>
        /// Process Extract USSD service verification request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        public USSDAPIResponse ProcessRequest(USSDRequestModel model)
        {
            try
            {
                string[] requestStage = model.Text.Split('|');
                if (requestStage.Length == (int)USSDProcessingStage.FileNumber)
                {
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = FileNumberStage(requestStage[1]) };
                }

                if (requestStage.Length > (int)USSDProcessingStage.FileNumber)
                {
                    return VerifySelectedItem(model);
                }

                throw new DirtyFormDataException("Invalid input, please try again.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get request status
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        private USSDAPIResponse GetRequestStatus(string fileNumber)
        {
            try
            {
                PSSRequestVM requestDet = _typeImplComposer.Value.ConfirmFileNumber(fileNumber);
                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = new { ApplicantName = requestDet.CustomerName, Service = requestDet.ServiceName, Status = requestDet.Status.ToDescription(), DocumentNumber = requestDet.ApprovalNumber } };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Verify the selected file number and returned the requested info based on user selected option
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        private USSDAPIResponse VerifySelectedItem(USSDRequestModel model)
        {
            string[] requestStage = model.Text.Split('|');
            if (string.IsNullOrEmpty(requestStage[1]))
            {
                throw new DirtyFormDataException("Invalid input, please try again.");
            }
            bool parsed = int.TryParse(requestStage[2], out int requestTypeId);
            if (!parsed)
            {
                throw new DirtyFormDataException("Invalid input, please try again.");
            }

            switch (requestTypeId)
            {
                case RequestStatusOption:
                    return GetRequestStatus(requestStage[1]);
            }
            throw new DirtyFormDataException("Invalid input, please try again.");
        }

        /// <summary>
        /// Prompt user to select action to be performed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string FileNumberStage(string fileNumber)
        {
            try
            {
                return _typeImplComposer.Value.ConfirmFileNumber(fileNumber).ServiceName;
                ////Remove this because we want to return the service name to the USSD provider
                //StringBuilder sb = new StringBuilder();
                //sb.Append("Please select the service process\n");
                //sb.Append("\n");
                //sb.Append("1.Status of request\n");
                //return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}