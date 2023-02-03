using System;
using Orchard;
using System.Linq;
using Orchard.Logging;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.GenerateInvoiceImpl.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.GenerateInvoiceImpl
{
    public class GenerateInvoiceAction : IServiceGenerateInvoiceImpl
    {
        public PSSServiceTypeDefinition GetServiceType => PSSServiceTypeDefinition.Escort;

        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortDetailsManager;

        private readonly IOrchardServices _orchardServices;
        private readonly ITypeImplComposer _typeImpl;
        public ILogger Logger { get; set; }
        private readonly IProposedEscortOfficerManager<ProposedEscortOfficer> _proposedManager;
        private readonly IEscortFormationOfficerManager<EscortFormationOfficer> _escortFormationOfficerManager;

        public GenerateInvoiceAction(IOrchardServices orchardServices, ITypeImplComposer typeImpl, IProposedEscortOfficerManager<ProposedEscortOfficer> proposedManager, Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortDetailsManager, IEscortFormationOfficerManager<EscortFormationOfficer> escortFormationOfficerManager)
        {
            _orchardServices = orchardServices;
            _typeImpl = typeImpl;
            Logger = NullLogger.Instance;
            _proposedManager = proposedManager;
            _escortDetailsManager = escortDetailsManager;
            _escortFormationOfficerManager = escortFormationOfficerManager;
        }


        /// <summary>
        /// At this level we are generating the invoice for the escort service
        /// after approval
        /// </summary>
        /// <param name="requestDeets"></param>
        public InvoiceGenerationResponse DoServiceImplementationWorkForGenerateInvoice(PSServiceRevenueHeadVM parentServicerevenueHead, IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads, IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets)
        {
            if(serviceRevenueHeads.Count() != 1)
            {
                throw new DirtyFormDataException("Escort service does not support multiple revnue heads for escort fee calculation");
            }
            //get the escort details
            EscortDetailsDTO escortDetails = _escortDetailsManager.Value.GetEscortDetails(requestDeets.First().Request.Id);
            
            //here we check if the escort officers have been assigned
            //officers must have been assigned before amount is computed
            if (!escortDetails.OfficersHaveBeenAssigned)
            {
                throw new DirtyFormDataException("Escort service has not been assigned");
            }

            decimal totalAmountToPay = ComputeEscortFee(escortDetails, requestDeets.First().Request.Id);

            CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, requestDeets.First().Request.Id, requestDeets.First().TaxEntityCategoryId, requestDeets.First().Request.FileRefNumber, requestDeets.First().TaxEntityId, totalAmountToPay);

            return _typeImpl.GenerateInvoice(inputModel, new ExpertSystemVM { Id = requestDeets.First().ExpertSystemId }, new TaxEntityViewModel { Id = requestDeets.First().TaxEntityId });
        }



        /// <summary>
        /// Compute the escort fee
        /// </summary>
        /// <param name="id"></param>
        /// <returns>decimal</returns>
        private decimal ComputeEscortFee(EscortDetailsDTO escortDetails, long requestId)
        {
            //IEnumerable<ProposedEscortOffficerVM>  proposedOfficers = _proposedManager.GetProposedOfficersCollection(escortDetails.Id);
            IEnumerable<ProposedEscortOffficerVM>  proposedOfficers = _escortFormationOfficerManager.GetEscortOfficersRate(requestId);
            int numberOfDays = (escortDetails.EndDate - escortDetails.StartDate).Days + 1;
            decimal rateValue = 00.00m;
            decimal sum = 00.00m;

            foreach (var item in proposedOfficers)
            {
                rateValue = (item.DeploymentRate * numberOfDays);
                if (rateValue <= 0) { throw new DirtyFormDataException("No set rate for this rank with ID " + item.OfficerRankId + " and officer " + item.OfficerName); }
                sum += rateValue;
            }

            return sum;
        }



        /// <summary>
        /// create invoice input model
        /// </summary>
        /// <param name="serviceRevenueHead"></param>
        /// <param name="requestId"></param>
        /// <param name="result"></param>
        /// <param name="categoryId"></param>
        /// <param name="fileRefNumber"></param>
        /// <returns>CreateInvoiceUserInputModel</returns>
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM serviceRevenueHead, long requestId, int categoryId, string fileRefNumber, Int64 taxEntityId, decimal amountToPay)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = serviceRevenueHead.IsGroupHead ? serviceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = serviceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} : File Number {2}.", serviceRevenueHead.FeeDescription, serviceRevenueHead.ServiceName, fileRefNumber),
                CallBackURL = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == TenantConfigKeys.RequestFeeAPICallBack.ToString()).FirstOrDefault().Value + "/?requestToken=" + requestId,
                TaxEntityCategoryId = categoryId,
                AddSurcharge = true,
                TaxEntity = new CBS.Core.Models.TaxEntity { Id = taxEntityId },
                RevenueHeadModels = new List<RevenueHeadUserInputModel> { { new RevenueHeadUserInputModel
                {
                     AdditionalDescription = string.Format("{0} for {1} {2}", serviceRevenueHead.FeeDescription, serviceRevenueHead.ServiceName, fileRefNumber),
                    Amount = amountToPay,
                    Quantity = 1,
                    RevenueHeadId = serviceRevenueHead.RevenueHeadId
                } } }
            };
        }


    }
}