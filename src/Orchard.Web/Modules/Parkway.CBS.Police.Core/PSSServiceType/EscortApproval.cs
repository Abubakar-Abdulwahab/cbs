using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class EscortApproval 
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Escort;

        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortDetailsManager;
        private readonly Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> _serviceRequestManager;
        private readonly Lazy<IPoliceRankingManager<PoliceRanking>> _policeRankingManager;
        private readonly Lazy<IPSSEscortOfficerDetailsManager<PSSEscortOfficerDetails>> _escortOfficerDetailsManager;
        private readonly Lazy<ITypeImplComposer> _compositionHandler;
        private readonly Lazy<ICoreStateAndLGA> _coreStateLGAService;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;


        public EscortApproval(IOrchardServices orchardServices, Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortDetailsManager, Lazy<ITypeImplComposer> compositionHandler, Lazy<IPoliceRankingManager<PoliceRanking>> policeRankingManager, Lazy<IPSSEscortOfficerDetailsManager<PSSEscortOfficerDetails>> escortOfficerDetailsManager, Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> serviceRequestManager, Lazy<ICoreStateAndLGA> coreStateLGAService)
        {
            _escortDetailsManager = escortDetailsManager;
            _compositionHandler = compositionHandler;
            Logger = NullLogger.Instance;
            _policeRankingManager = policeRankingManager;
            _escortOfficerDetailsManager = escortOfficerDetailsManager;
            _serviceRequestManager = serviceRequestManager;
            _orchardServices = orchardServices;
            _coreStateLGAService = coreStateLGAService;
        }


        /// <summary>
        /// Get the Escort view details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public dynamic GetServiceRequestViewDetails(long requestId)
        {
            PSSEscortDetails escort = _escortDetailsManager.Value.Get(x => x.Request.Id == requestId);

            return new EscortRequestDetailsVM
            {
                TaxEntity = new TaxEntityViewModel
                {
                    Recipient = escort.Request.TaxEntity.Recipient,
                    PhoneNumber = escort.Request.TaxEntity.PhoneNumber,
                    RCNumber = escort.Request.TaxEntity.RCNumber,
                    Address = escort.Request.TaxEntity.Address,
                    Email = escort.Request.TaxEntity.Email,
                    TaxPayerIdentificationNumber = escort.Request.TaxEntity.TaxPayerIdentificationNumber,
                    SelectedStateName = escort.Request.TaxEntity.StateLGA.State.Name,
                    SelectedState = escort.Request.TaxEntity.StateLGA.State.Id,
                    SelectedLGAName = escort.Request.TaxEntity.StateLGA.Name,
                    SelectedStateLGA = escort.Request.TaxEntity.StateLGA.Id
                },

                EscortInfo = new EscortRequestVM
                {
                    Address = escort.Address,
                    CommandAddress = escort.Request.Command.Address,
                    CommandName = escort.Request.Command.Name,
                    SelectedCommand = escort.Request.Command.Id,
                    NumberOfOfficers = escort.NumberOfOfficers,
                    StateName = escort.LGA.State.Name,
                    LGAName = escort.LGA.Name,
                    CommandStateName = escort.Request.Command.State.Name,
                    CommandLgaName = escort.Request.Command.LGA.Name,
                    StartDate = escort.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = escort.EndDate.ToString("dd/MM/yyyy"),
                    DurationNumber = escort.DurationNumber,
                    DurationType = escort.DurationType
                },
                ViewName = "PSSEscortDetails",
                PoliceRanks = _policeRankingManager.Value.GetPoliceRanks(),
                RequestId = escort.Request.Id,
                ServiceTypeId = escort.Request.Service.ServiceType,
                StateLGAs = _coreStateLGAService.Value.GetStates(),
            };
        }


        /// <summary>
        /// create invoice input model
        /// </summary>
        /// <param name="parentServiceRevenueHead"></param>
        /// <param name="requestId"></param>
        /// <param name="result"></param>
        /// <param name="categoryId"></param>
        /// <param name="fileRefNumber"></param>
        /// <returns>CreateInvoiceUserInputModel</returns>
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, long requestId, IEnumerable<PSServiceRevenueHeadVM> result, int categoryId, string fileRefNumber, decimal amountToPay)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} {2}", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber),
                CallBackURL = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == TenantConfigKeys.RequestFeeAPICallBack.ToString()).FirstOrDefault().Value + "/?requestToken=" + requestId.ToString(),
                TaxEntityCategoryId = categoryId,
                RevenueHeadModels = result.Where(r => !r.IsGroupHead).Select(r =>
                new RevenueHeadUserInputModel
                {
                    AdditionalDescription = string.Format("{0} for {1} {2}", r.FeeDescription, r.ServiceName, fileRefNumber),
                    Amount = amountToPay,
                    Quantity = 1,
                    RevenueHeadId = r.RevenueHeadId
                }).ToList()
            };
        }

        /// <summary>
        /// Save request details for this request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="sRequestFormDump"></param>
        /// <param name="taxPayerProfileVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        private InvoiceGenerationResponse GenerateRequestInvoiceAfterApproval(IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeadVMs, PSServiceRequestInvoiceValidationDTO pSServiceRequest, TaxEntity taxEntity, decimal amountToPay)
        {
            try
            {
                //get the revenue head
                IEnumerable<ExpertSystemVM> expertSystem = _compositionHandler.Value.GetExpertSystem();

                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeadVMs.Count() > 1 ? serviceRevenueHeadVMs.Where(r => r.IsGroupHead).Single() : serviceRevenueHeadVMs.ElementAt(0);

                TaxEntityViewModel entityVM = new TaxEntityViewModel { Id = taxEntity.Id, CategoryId = taxEntity.TaxEntityCategory.Id };
                CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, pSServiceRequest.Request.Id, serviceRevenueHeadVMs, entityVM.CategoryId, pSServiceRequest.Request.FileRefNumber, amountToPay);

                InvoiceGenerationResponse response = _compositionHandler.Value.GenerateInvoice(inputModel, expertSystem.ElementAt(0), entityVM);
                response.PaymentURL = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == TenantConfigKeys.BaseURL.ToString()).FirstOrDefault().Value + "/p/make-payment/" + response.InvoiceNumber;

                return response;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("{0}", exception.Message));
                _escortDetailsManager.Value.RollBackAllTransactions();
                throw;
            }
        }
    }
}