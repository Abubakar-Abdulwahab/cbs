using Orchard;
using Orchard.Logging;
using Parkway.CBS.Police.Client.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Client.PSSServiceType
{
    public class GenericDetails : IPSSServiceTypeDetails
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.GenericPoliceServices;
        private readonly Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> _serviceRequestManager;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<IRequestStatusLogManager<RequestStatusLog>> _requestStatusLogManager;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;

        public GenericDetails(IOrchardServices orchardServices, Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> serviceRequestManager, Lazy<IRequestStatusLogManager<RequestStatusLog>> requestStatusLogManager, Lazy<IPSSRequestManager<PSSRequest>> requestManager)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _serviceRequestManager = serviceRequestManager;
            _requestStatusLogManager = requestStatusLogManager;
            _requestManager = requestManager;
        }


        /// <summary>
        /// This method get the direction of the request after profile has been confirmed
        /// <para>This marks the start of the request process</para>
        /// </summary>
        /// <returns>RouteNameAndStage</returns>
        public RouteNameAndStage GetDirectionAfterUserProfileConfirmation()
        {
            return new RouteNameAndStage { RouteName = "P.Generic.Police.Request", Stage = PSSUserRequestGenerationStage.PSSRequest };
        }



        public dynamic GetRequestDetails(string fileRefNumber, long taxEntityId)
        {
            try
            {
                IEnumerable<GenericRequestDetailsVM> serviceDetails = _serviceRequestManager.Value.GetGenericServiceRequestDetails(fileRefNumber, taxEntityId);
                IEnumerable<PSSRequestStatusLogVM> requestStatusLog = _requestStatusLogManager.Value.GetRequestStatusLogVMByFileRefNumber(fileRefNumber);
                GenericRequestDetailsVM requestDets = serviceDetails.FirstOrDefault();
                requestDets.FormControlValues = _requestManager.Value.GetFormDetails(requestDets.RequestId).ToList();
                return new RequestDetailsVM { RequestStatusLog = requestStatusLog, ServiceName = requestDets.ServiceName, ServiceVM = requestDets, TaxEntity = requestDets.TaxEntity, ViewName = "GenericDetailsPartial", FileRefNumber = requestDets.FileRefNumber, ApprovalNumber = requestDets.ApprovalNumber, Status = requestDets.Status };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public dynamic GetRequestInfo(long requestId)
        {
            try
            {
                IEnumerable<GenericRequestDetailsVM> serviceDetails = _serviceRequestManager.Value.GetGenericDocumentInfo(requestId);
                GenericRequestDetailsVM requestDets = serviceDetails.FirstOrDefault();
                requestDets.ViewName = "GenericDocumentInfo";
                requestDets.FormControlValues = _requestManager.Value.GetFormDetails(requestId).ToList();
                return requestDets;
            }
            catch (Exception) { throw; }
        }


        public CreateCertificateDocumentVM CreateCertificate(string fileRefNumber, long taxEntityId)
        {
            throw new NotImplementedException();
        }
    }
}