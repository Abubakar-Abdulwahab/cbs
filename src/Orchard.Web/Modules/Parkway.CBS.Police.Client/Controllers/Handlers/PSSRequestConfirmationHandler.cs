using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSRequestConfirmationHandler : IPSSRequestConfirmationHandler
    {
        private readonly IEnumerable<Lazy<IPSSServiceTypeImpl>> _serviceTypeImpl;
        private readonly ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> _iCBSUserTaxEntityProfileLocationManager;

        public PSSRequestConfirmationHandler(IEnumerable<Lazy<IPSSServiceTypeImpl>> serviceTypeImpl, ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> iCBSUserTaxEntityProfileLocationManager)
        {
            _serviceTypeImpl = serviceTypeImpl;
            _iCBSUserTaxEntityProfileLocationManager = iCBSUserTaxEntityProfileLocationManager;
        }        


        /// <summary>
        /// Get VM confirmation for service type
        /// </summary>
        /// <param name="serviceTypeId"></param>
        /// <param name="sRequestFormDump"></param>
        /// <returns>RequestConfirmationVM</returns>
        public RequestConfirmationVM GetVMForRequestConfirmationPage(int serviceId, int serviceTypeId, string sRequestFormDump)
        {
            foreach (var impl in _serviceTypeImpl)
            {
                if ((PSSServiceTypeDefinition)serviceTypeId == impl.Value.GetServiceTypeDefinition)
                {
                    return impl.Value.GetModelForRequestConfirmation(serviceId, sRequestFormDump);
                }
            }
            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + serviceId);
        }


        /// <summary>
        /// Save details for this request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="sRequestFormDump"></param>
        public InvoiceGenerationResponse SaveRequestDetails(PSSRequestStageModel processStage, string sRequestFormDump, TaxEntityViewModel taxPayerProfileVM)
        {
            foreach (var impl in _serviceTypeImpl)
            {
                if ((PSSServiceTypeDefinition)processStage.ServiceType == impl.Value.GetServiceTypeDefinition)
                {
                    return impl.Value.SaveRequestDetailsAfterConfirmation(processStage, sRequestFormDump, taxPayerProfileVM);
                }
            }
            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + processStage.ServiceId);
        }


        /// <summary>
        /// Gets branch location for cbs user with specified id
        /// </summary>
        /// <param name="cbsUserId"></param>
        /// <returns></returns>
        public TaxEntityProfileLocationVM GetCBSUserLocation(long cbsUserId)
        {
            return _iCBSUserTaxEntityProfileLocationManager.GetCBSUserLocationWithId(cbsUserId);
        }
    }
}