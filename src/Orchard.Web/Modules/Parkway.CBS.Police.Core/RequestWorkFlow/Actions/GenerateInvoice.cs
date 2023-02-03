using Orchard;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.GenerateInvoiceImpl.Contracts;


namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions
{
    public class GenerateInvoice : IActionImpl
    {
        public RequestDirection GetRequestDirection => RequestDirection.GenerateInvoice;

        private readonly IPSServiceRevenueHeadManager<PSServiceRevenueHead> _revenueServiceMan;
        private readonly IOrchardServices _orchardServices;
        private readonly ITypeImplComposer _typeImpl;
        public ILogger Logger { get; set; }
        private readonly IEnumerable<IServiceGenerateInvoiceImpl> _generateInvoiceActionImpl;
        

        public GenerateInvoice(IOrchardServices orchardServices, IPSServiceRevenueHeadManager<PSServiceRevenueHead> revenueServiceMan, ITypeImplComposer typeImpl, IEnumerable<IServiceGenerateInvoiceImpl> generateInvoiceActionImpl)
        {
            _revenueServiceMan = revenueServiceMan;
            _orchardServices = orchardServices;
            _typeImpl = typeImpl;
            Logger = NullLogger.Instance;
            _generateInvoiceActionImpl = generateInvoiceActionImpl;
        }



        public RequestFlowVM MoveToNextDefinitionLevel(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel)
        {
            try
            {
                IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads = _revenueServiceMan.GetRevenueHead(requestDeets.First().ServiceId, nextDefinedLevel.Id);
                if (serviceRevenueHeads == null || !serviceRevenueHeads.Any())
                {
                    throw new NoBillingInformationFoundException("No billing info found for service Id " + requestDeets.First().ServiceId);
                }
                //now that we have gotten the next set of revenue heads to generate an invoice for
                //lets generate that invoice
                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeads.Count() > 1 ? serviceRevenueHeads.Where(r => r.IsGroupHead).Single() : serviceRevenueHeads.ElementAt(0);
                bool hasCustomImpl = false;
                InvoiceGenerationResponse response = null;

                foreach (var impl in _generateInvoiceActionImpl)
                {
                    if (impl.GetServiceType == requestDeets.First().ServiceType)
                    {
                        hasCustomImpl = true;
                        response = impl.DoServiceImplementationWorkForGenerateInvoice(parentServicerevenueHead, serviceRevenueHeads, requestDeets);
                        break;
                    }
                }

                if (response == null && !hasCustomImpl)
                {
                    CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, requestDeets.First().Request.Id, serviceRevenueHeads, requestDeets.First().TaxEntityCategoryId, requestDeets.First().Request.FileRefNumber, requestDeets.First().TaxEntityId);
                    response = _typeImpl.GenerateInvoice(inputModel, new ExpertSystemVM { Id = requestDeets.First().ExpertSystemId }, new TaxEntityViewModel { Id = requestDeets.First().TaxEntityId });
                }
                //create service request
                _typeImpl.SaveServiceRequest(requestDeets.First().Request, serviceRevenueHeads, requestDeets.First().ServiceId, response.InvoiceId, nextDefinedLevel.Id, PSSRequestStatus.PendingInvoicePayment);

                _typeImpl.AddRequestAndInvoice(requestDeets.First().Request, response.InvoiceId);

                _typeImpl.AddRequestStatusLog(new RequestStatusLog
                {
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                    Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                    Status = (int)PSSRequestStatus.PendingInvoicePayment,
                    StatusDescription = PSSRequestStatus.PendingInvoicePayment.ToDescription(),
                    Invoice = new CBS.Core.Models.Invoice { Id = response.InvoiceId },
                });

                //update request command workflow log
                _typeImpl.UpdateRequestCommandWorkFlowLog(requestDeets);

                //add request command workflow log
                _typeImpl.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog
                {
                    Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                    Command = new Command { Id = requestDeets.First().CommandId },
                    DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                    IsActive = true,
                    RequestPhaseId = (int)RequestPhase.New,
                    RequestPhaseName = nameof(RequestPhase.New)
                });

                _typeImpl.UpdateRequestDefinitionFlowLevel(requestDeets.First().Request.Id, nextDefinedLevel.Id, PSSRequestStatus.PendingInvoicePayment);

                //Send a sms notification to the payer
                _typeImpl.SendInvoiceSMSNotification(new SMSDetailVM { RevenueHead = parentServicerevenueHead.ServiceName, Amount = response.AmountDue.ToString("F"), Name = response.Recipient, PhoneNumber = response.PhoneNumber, TaxEntityId = requestDeets.First().TaxEntityId, InvoiceNumber = response.InvoiceNumber });

                return new RequestFlowVM
                {
                    Message = string.Format("An invoice with invoice number {0} has been generated as required by the defined workflow. No further action is required for now until the invoice has been fully paid for.", response.InvoiceNumber)
                };
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _typeImpl.RollBackAllTransactions();
                throw;
            }
        }



        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, long requestId, IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads, int categoryId, string fileRefNumber, long taxEntityId)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} : File Number {2}.", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber),
                CallBackURL = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == TenantConfigKeys.RequestFeeAPICallBack.ToString()).FirstOrDefault().Value + "/?requestToken=" + requestId,
                TaxEntityCategoryId = categoryId,
                AddSurcharge = true,
                TaxEntity = new CBS.Core.Models.TaxEntity { Id = taxEntityId },
                RevenueHeadModels = serviceRevenueHeads.Where(r => !r.IsGroupHead).Select(r =>
                new RevenueHeadUserInputModel
                {
                    AdditionalDescription = string.Format("{0} for {1} {2}", r.FeeDescription, r.ServiceName, fileRefNumber),
                    Amount = r.AmountToPay,
                    Quantity = 1,
                    RevenueHeadId = r.RevenueHeadId
                }).ToList()
            };
        }



    }
}