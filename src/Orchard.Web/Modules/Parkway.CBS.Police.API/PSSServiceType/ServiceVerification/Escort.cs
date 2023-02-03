using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.API.PSSServiceType.ServiceVerification.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Exceptions;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Net;

namespace Parkway.CBS.Police.API.PSSServiceType.ServiceVerification
{
    public class Escort : IPSSServiceTypeUSSDVerificationImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Escort;
        public ILogger Logger { get; set; }
        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortDetailsManager;
        private readonly Lazy<ITypeImplComposer> _typeImplComposer;
        private readonly IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> _escortOfficerDeploymentLogManager;
        private const int RequestStatusOption = 1;
        private const int DeploymentDetailOption = 2;
        private const int OfficerResumptionDateOption = 3;

        public Escort(Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortDetailsManager, Lazy<ITypeImplComposer> typeImplComposer, IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> escortOfficerDeploymentLogManager)
        {
            Logger = NullLogger.Instance;
            _escortDetailsManager = escortDetailsManager;
            _typeImplComposer = typeImplComposer;
            _escortOfficerDeploymentLogManager = escortOfficerDeploymentLogManager;
        }

        /// <summary>
        /// Process Character Certificate USSD service verification request
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

                case DeploymentDetailOption:
                    return GetDeploymentDetail(requestStage[1]);

                case OfficerResumptionDateOption:
                    return GetResumptionDate(requestStage[1]);
            }
            throw new DirtyFormDataException("Invalid input, please try again.");
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
        /// Get request deployment details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        private USSDAPIResponse GetDeploymentDetail(string fileNumber)
        {
            try
            {
                PSSRequestVM requestDet = _typeImplComposer.Value.ConfirmFileNumber(fileNumber);
                if (requestDet.Status != PSSRequestStatus.Approved)
                {
                    throw new PSSRequestNotApprovedException("Request is yet to be approved.");
                }

                EscortDetailsDTO escortDetails = _escortDetailsManager.Value.GetEscortDetailsVM(fileNumber);

                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = $"{escortDetails.NumberOfOfficers} officers requested / {_escortOfficerDeploymentLogManager.GetEscortOfficerAssignedNumber(fileNumber)} officer(s) approved." };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get offcier resumption date
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        private USSDAPIResponse GetResumptionDate(string fileNumber)
        {
            try
            {
                PSSRequestVM requestDet = _typeImplComposer.Value.ConfirmFileNumber(fileNumber);
                if (requestDet.Status != PSSRequestStatus.Approved)
                {
                    throw new PSSRequestNotApprovedException("Request is yet to be approved.");
                }

                EscortDetailsDTO escortDetails = _escortDetailsManager.Value.GetEscortDetailsVM(fileNumber);
                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = $"Request start date is {escortDetails.StartDate:dd/MM/yyyy}." };
            }
            catch (Exception)
            {
                throw;
            }
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
                //sb.Append("2.Deployment details\n");
                //sb.Append("3.Officer resumption date\n");
                //return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}