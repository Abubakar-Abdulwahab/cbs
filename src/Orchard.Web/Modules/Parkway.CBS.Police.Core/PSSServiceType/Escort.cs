using System;
using Orchard.Logging;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Newtonsoft.Json;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Utilities;
using Orchard;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Police.Core.Utilities;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class Escort : IPSSServiceTypeImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Escort;

        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortManager;
        private readonly Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> _escortSettings;
        private readonly IPSSEscortServiceCategoryManager<PSSEscortServiceCategory> _escortServiceCategoryManager;
        private readonly ICommandTypeManager<CommandType> _commandTypeManager;
        private readonly ITypeImplComposer _compositionHandler;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;


        public Escort(Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortManager, ITypeImplComposer compositionHandler, IOrchardServices orchardServices, Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> escortSettings, IPSSEscortServiceCategoryManager<PSSEscortServiceCategory> escortServiceCategoryManager, ICommandTypeManager<CommandType> commandTypeManager)
        {
            _escortManager = escortManager;
            _compositionHandler = compositionHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _escortSettings = escortSettings;
            _escortServiceCategoryManager = escortServiceCategoryManager;
            _commandTypeManager = commandTypeManager;
        }


        /// <summary>
        /// Here we get the model for request confirmation
        /// <para>This method is used to get the model to be displayed on the client side showing the details
        /// of the request so they can confirm</para>
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="objStringValue"></param>
        /// <returns>RequestConfirmationVM</returns>
        public RequestConfirmationVM GetModelForRequestConfirmation(int serviceId, string objStringValue)
        {
            EscortRequestVM requestVM = JsonConvert.DeserializeObject<EscortRequestVM>(objStringValue);
            if (requestVM == null) { throw new Exception("No session value found for ExtractRequestVM"); }
            //here we are getting the initial flow level for the workflow configured for escort services
            //get init level definition
            //here we get the init definition level, this line would require some modifications
            //when the workflow direction of this service is different from what was assigned
            //we will need to pass in some parameters such as the HasDifferentialWorkFlow
            //int initLevelId = _compositionHandler.GetInitFlow(serviceId, requestVM.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { DifferentialModelName = nameof(PSSEscortServiceCategory), DifferentialValue = requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(0) });
            int initLevelId = _compositionHandler.GetInitFlow(serviceId, requestVM.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { DifferentialModelName = nameof(CommandType), DifferentialValue = requestVM.SelectedCommandType });

            IEnumerable<PSServiceRevenueHeadVM> result = _compositionHandler.GetRevenueHeadDetails(serviceId, initLevelId);
            if (result == null || !result.Any())
            {
                throw new NoBillingInformationFoundException("No billing info found for service Id " + serviceId);
            }

            ICollection<PSSEscortServiceCategoryVM> serviceCategories = new List<PSSEscortServiceCategoryVM>();
            if(requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(1) > 0)
            {
                serviceCategories.Add(_escortServiceCategoryManager.GetEscortServiceCategoryWithId(requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(1)));
            }
            else { serviceCategories.Add(_escortServiceCategoryManager.GetEscortServiceCategoryWithId(requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(0))); }

            dynamic model = new ExpandoObject();
            model.Duration = string.Format("{0} - {1}", requestVM.ParsedStartDate.ToString("dd'/'MM'/'yyyy"), requestVM.ParsedEndDate.ToString("dd'/'MM'/'yyyy"));
            model.NumberOfOfficers = requestVM.NumberOfOfficers;
            //model.PrefferedPaymentSchedule = requestVM.PSBillingType.ToDescription();
            model.Address = requestVM.Address;
            model.ServiceCategoryName = (requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(1) > 0) ? serviceCategories.ElementAt(0).ParentName : serviceCategories.ElementAt(0).Name; 
            model.ServiceCategoryType = (requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(1) > 0) ? serviceCategories.ElementAt(0).Name : string.Empty;
            model.ShowExtraFields = requestVM.ShowExtraFieldsForServiceCategoryType;
            model.OriginStateName = requestVM.OriginStateName;
            model.OriginLGAName = requestVM.OriginLGAName;
            model.AddressOfOriginLocation = requestVM.AddressOfOriginLocation;
            model.DurationNumber = requestVM.DurationNumber;
            model.CommandTypeName = _commandTypeManager.GetCommandType(requestVM.SelectedCommandType).Name;

            return new RequestConfirmationVM
            {
                AmountDetails = result.Where(r => !r.IsGroupHead).Select(r => new AmountDetails { AmountToPay = (r.AmountToPay + r.Surcharge), FeeDescription = r.FeeDescription }).ToList(),
                HeaderObj = new HeaderObj { },
                NameOfPoliceCommand = string.Format("{0} ({1}, {2}, {3})", requestVM.CommandName, requestVM.CommandAddress, requestVM.LGAName, requestVM.StateName),
                ServiceRequested = result.ElementAt(0).ServiceName,
                Reason = requestVM.Reason,
                PartialName = "EscortConfirmationPartial",
                RequestSpecificModel = model
            };
        }



        /// <summary>
        /// Once the user has confirmed their details we need to save the request details
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="sRequestFormDump"></param>
        /// <param name="taxPayerProfileVM"></param>
        /// <returns></returns>
        public InvoiceGenerationResponse SaveRequestDetailsAfterConfirmation(PSSRequestStageModel processStage, string sRequestFormDump, TaxEntityViewModel taxPayerProfileVM)
        {
            try
            {
                //we want to save this request
                EscortRequestVM requestVM = JsonConvert.DeserializeObject<EscortRequestVM>(sRequestFormDump);
                if (requestVM == null) { throw new Exception("No session value found for ExtractRequestVM"); }

                IEnumerable<ExpertSystemVM> expertSystem = _compositionHandler.GetExpertSystem();
                //get the revenue head
                //based on the category that was selected we need to use the default or the differential workflow
                //PSServiceRequestFlowDefinitionLevelDTO initLevel = _compositionHandler.GetInitFlowLevel(processStage.ServiceId, requestVM.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { DifferentialModelName = nameof(PSSEscortServiceCategory), DifferentialValue = requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(0) });
                PSServiceRequestFlowDefinitionLevelDTO initLevel = _compositionHandler.GetInitFlowLevel(processStage.ServiceId, requestVM.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { DifferentialModelName = nameof(CommandType), DifferentialValue = requestVM.SelectedCommandType });

                IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads = _compositionHandler.GetRevenueHeadDetails(processStage.ServiceId, initLevel.Id);
                if (serviceRevenueHeads == null || !serviceRevenueHeads.Any())
                {
                    throw new NoBillingInformationFoundException("No billing info found for service Id " + processStage.ServiceId);
                }

                //get expected hash for this service
                requestVM.ExpectedHash = _compositionHandler.GetExpectedHash(processStage.ServiceId, initLevel.Id);

                PSSRequest request = _compositionHandler.SaveRequest(processStage, requestVM, taxPayerProfileVM, initLevel.Id, PSSRequestStatus.PendingInvoicePayment);

                string fileRefNumber = _compositionHandler.GetRequestFileRefNumber(request);

                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeads.Count() > 1 ? serviceRevenueHeads.Where(r => r.IsGroupHead).Single() : serviceRevenueHeads.ElementAt(0);

                string callbackParam = _compositionHandler.GetURLRequestTokenString(request.Id, requestVM.ExpectedHash);

                CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, request.Id, requestVM, serviceRevenueHeads, processStage.CategoryId, fileRefNumber, callbackParam);

                TaxEntityViewModel entityVM = new TaxEntityViewModel { Id = taxPayerProfileVM.Id };

                InvoiceGenerationResponse response = _compositionHandler.GenerateInvoice(inputModel, expertSystem.ElementAt(0), entityVM);
                //match request and invoice
                _compositionHandler.AddRequestAndInvoice(request, response.InvoiceId);
                //add to request status log
                RequestStatusLog statusLog = new RequestStatusLog
                {
                    Invoice = new Invoice { Id = response.InvoiceId },
                    StatusDescription = initLevel.PositionDescription,
                    Request = request,
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = initLevel.Id },
                    UserActionRequired = true,
                    Status = (int)PSSRequestStatus.PendingInvoicePayment
                };
                _compositionHandler.AddRequestStatusLog(statusLog);

                _compositionHandler.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog { Request = request, Command = new Command { Id = requestVM.SelectedCommand }, DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = initLevel.Id }, IsActive = true, RequestPhaseId = (int)RequestPhase.New, RequestPhaseName = nameof(RequestPhase.New) });
                //save service request
                _compositionHandler.SaveServiceRequest(request, serviceRevenueHeads, processStage.ServiceId, response.InvoiceId, initLevel.Id, PSSRequestStatus.PendingInvoicePayment);
                //get settings
                int workFlowdefinitionId = _compositionHandler.GetWorkFlowDefinition(initLevel.Id);

                if(workFlowdefinitionId < 1) { throw new NoRecordFoundException("No flow definition found"); }

                int escortSettingsId = _escortSettings.Value.GetEscortSettingsId(workFlowdefinitionId);
                if(escortSettingsId < 1) { throw new NoRecordFoundException("No escort settings found"); }

                if (!_escortManager.Value.Save(
                    new PSSEscortDetails
                    {
                        Request = request,
                        NumberOfOfficers = requestVM.NumberOfOfficers,
                        DurationNumber = requestVM.DurationNumber,
                        Address = requestVM.Address,
                        StartDate = requestVM.ParsedStartDate,
                        EndDate = requestVM.ParsedEndDate,
                        LGA = new LGA { Id = requestVM.SelectedStateLGA },
                        State = new StateModel { Id = requestVM.SelectedState },
                        PaymentPeriodType = (int)requestVM.PSBillingType,
                        TaxEntitySubCategory = new TaxEntitySubCategory { Id = processStage.SubCategoryId },
                        TaxEntitySubSubCategory = processStage.SubSubCategoryId == 0 ? null : new TaxEntitySubSubCategory { Id = processStage.SubSubCategoryId },
                        Settings = new PSSEscortSettings { Id = escortSettingsId },
                        ServiceCategory = new PSSEscortServiceCategory { Id = requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(0) },
                        CategoryType = (requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(1) > 0) ? new PSSEscortServiceCategory { Id = requestVM.SelectedEscortServiceCategories.ElementAtOrDefault(1) } : null,
                        OriginState = (requestVM.ShowExtraFieldsForServiceCategoryType) ? new StateModel { Id = requestVM.SelectedOriginState } : null,
                        OriginLGA = (requestVM.ShowExtraFieldsForServiceCategoryType) ? new LGA { Id = requestVM.SelectedOriginLGA } : null,
                        OriginAddress = (requestVM.ShowExtraFieldsForServiceCategoryType) ? requestVM.AddressOfOriginLocation : null,
                        CommandType = new CommandType { Id = requestVM.SelectedCommandType }
                    }))
                { throw new CouldNotSaveRecord("Could not save escort details"); }

                //Send a sms notification to the payer
                //_compositionHandler.SendInvoiceSMSNotification(new SMSDetailVM { RevenueHead = parentServicerevenueHead.ServiceName, Amount = response.AmountDue.ToString("F"), Name = taxPayerProfileVM.Recipient, PhoneNumber = (string.IsNullOrEmpty(request.ContactPersonPhoneNumber) ? taxPayerProfileVM.PhoneNumber : request.ContactPersonPhoneNumber), TaxEntityId = taxPayerProfileVM.Id, InvoiceNumber = response.InvoiceNumber });

                return response;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("{0} {1}", exception.Message, sRequestFormDump));
                _escortManager.Value.RollBackAllTransactions();
                throw;
            }
        }




        /// <summary>
        /// ge
        /// </summary>
        /// <param name="parentServiceRevenueHead"></param>
        /// <param name="requestId"></param>
        /// <param name="requestVM"></param>
        /// <param name="result"></param>
        /// <param name="categoryId"></param>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, long requestId, EscortRequestVM requestVM, IEnumerable<PSServiceRevenueHeadVM> result, int categoryId, string fileRefNumber, string callbackParam)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} {2}. Duration {3}. Number of Officers {4}.", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber, string.Format("{0} - {1}", requestVM.ParsedStartDate.ToString("dd'/'MM'/'yyyy"), requestVM.ParsedEndDate.ToString("dd'/'MM'/'yyyy")), requestVM.NumberOfOfficers),
                CallBackURL = PSSUtil.GetURLForFeeConfirmation(_orchardServices.WorkContext.CurrentSite.SiteName, callbackParam),
                TaxEntityCategoryId = categoryId,
                AddSurcharge = true,
                RevenueHeadModels = result.Where(r => !r.IsGroupHead).Select(r =>
                new RevenueHeadUserInputModel
                {
                    AdditionalDescription = string.Format("{0} for {1} {2}", r.FeeDescription, r.ServiceName, fileRefNumber),
                    Amount = (r.AmountToPay + r.Surcharge),
                    Quantity = 1,
                    RevenueHeadId = r.RevenueHeadId
                }).ToList()
            };
        }

    }
}