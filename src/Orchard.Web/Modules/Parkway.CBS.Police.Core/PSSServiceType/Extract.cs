using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class Extract : IPSSServiceTypeImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Extract;

        private readonly Lazy<IExtractDetailsManager<ExtractDetails>> _extractManager;
        private readonly ITypeImplComposer _compositionHandler;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly IExtractRequestFilesManager<ExtractRequestFiles> _extractFilesManager;
        private readonly IExtractCategoryManager<ExtractCategory> _extractCategoryManager;
        private readonly IPSSRequestExtractDetailsCategoryManager<PSSRequestExtractDetailsCategory> _pssRequestExtractDetailsCategoryManager;

        public Extract(Lazy<IExtractDetailsManager<ExtractDetails>> extractManager, ITypeImplComposer compositionHandler, IOrchardServices orchardServices, IExtractRequestFilesManager<ExtractRequestFiles> extractFilesManager, IExtractCategoryManager<ExtractCategory> extractCategoryManager, IPSSRequestExtractDetailsCategoryManager<PSSRequestExtractDetailsCategory> pssRequestExtractDetailsCategoryManager)
        {
            _extractManager = extractManager;
            _compositionHandler = compositionHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _extractFilesManager = extractFilesManager;
            _extractCategoryManager = extractCategoryManager;
            _pssRequestExtractDetailsCategoryManager = pssRequestExtractDetailsCategoryManager;
        }


        /// <summary>
        /// This method gets the model for displaying the confirmation details of a request 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="objStringValue"></param>
        /// <returns>RequestConfirmationVM</returns>
        public RequestConfirmationVM GetModelForRequestConfirmation(int serviceId, string objStringValue)
        {
            ExtractRequestVM requestVM = JsonConvert.DeserializeObject<ExtractRequestVM>(objStringValue);
            if (requestVM == null) { throw new Exception("No session value found for ExtractRequestVM"); }
            //get init level definition
            //get init level definition
            //here we get the init definition level, this line would require some modifications
            //when the workflow direction of this service is different from what was assigned
            //we will need to pass in some parameters such as the HasDifferentialWorkFlow
            int initLevelId = _compositionHandler.GetInitFlow(serviceId, requestVM.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { });

            IEnumerable<PSServiceRevenueHeadVM> result = _compositionHandler.GetRevenueHeadDetails(serviceId, initLevelId);
            if (result == null || !result.Any())
            {
                throw new NoBillingInformationFoundException("No billing info found for service Id " + serviceId);
            }

            List<ExtractCategoryVM> extractCategories = new List<ExtractCategoryVM>();

            foreach(var selectedCategoryAndSubCategory in requestVM.SelectedCategoriesAndSubCategoriesDeserialized)
            {
                extractCategories.AddRange(_extractCategoryManager.GetActiveSubCategoriesList(selectedCategoryAndSubCategory.Key));
            }

            dynamic model = new ExpandoObject();
            model.IsIncidentReported = requestVM.IsIncidentReported;
            model.IncidentReportedDate = requestVM.IncidentReportedDate;
            model.AffidavitNumber = requestVM.AffidavitNumber;
            model.AffidavitDateOfIssuance = requestVM.AffidavitDateOfIssuance;
            model.extractCategories = extractCategories;
            model.selectedCategoriesAndSubCategories = requestVM.SelectedCategoriesAndSubCategoriesDeserialized;
            model.otherReason = requestVM.Reason;
            return new RequestConfirmationVM
            {
                AmountDetails = result.Where(r => !r.IsGroupHead).Select(r => new AmountDetails { AmountToPay = (r.AmountToPay + r.Surcharge), FeeDescription = r.FeeDescription }).ToList(),
                HeaderObj = new HeaderObj { },
                NameOfPoliceCommand = string.Format("{0} ({1}, {2}, {3})", requestVM.CommandName, requestVM.CommandAddress, requestVM.LGAName, requestVM.StateName),
                ServiceRequested = result.ElementAt(0).ServiceName,
                Reason = string.Empty,
                PartialName = "ExtractConfirmationPartial",
                RequestSpecificModel = model
            };
        }



        /// <summary>
        /// Save request details for this request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="sRequestFormDump"></param>
        /// <param name="taxPayerProfileVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        public InvoiceGenerationResponse SaveRequestDetailsAfterConfirmation(PSSRequestStageModel processStage, string sRequestFormDump, TaxEntityViewModel taxPayerProfileVM)
        {
            try
            {
                //we want to save this request
                ExtractRequestVM requestVM = JsonConvert.DeserializeObject<ExtractRequestVM>(sRequestFormDump);
                if (requestVM == null) { throw new Exception("No session value found for ExtractRequestVM"); }

                //get the revenue head
                IEnumerable<ExpertSystemVM> expertSystem = _compositionHandler.GetExpertSystem();
                //get init level definition
                PSServiceRequestFlowDefinitionLevelDTO initLevel = _compositionHandler.GetInitFlowLevel(processStage.ServiceId, requestVM.HasDifferentialWorkFlow, new ServiceWorkFlowDifferentialDataParam { });

                IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeads = _compositionHandler.GetRevenueHeadDetails(processStage.ServiceId, initLevel.Id);
                if (serviceRevenueHeads == null || !serviceRevenueHeads.Any())
                {
                    throw new NoBillingInformationFoundException("No billing info found for service Id " + processStage.ServiceId);
                }

                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeads.Count() > 1 ? serviceRevenueHeads.Where(r => r.IsGroupHead).Single() : serviceRevenueHeads.ElementAt(0);
                //get expected hash for this service
                requestVM.ExpectedHash = _compositionHandler.GetExpectedHash(processStage.ServiceId, initLevel.Id);

                PSSRequest request = _compositionHandler.SaveRequest(processStage, requestVM, taxPayerProfileVM, initLevel.Id, PSSRequestStatus.PendingInvoicePayment);
                _compositionHandler.SaveCommandDetails(new List<RequestCommand> { { new RequestCommand { Command = new Command { Id = requestVM.SelectedCommand }, Request = request } } });

                string fileRefNumber = _compositionHandler.GetRequestFileRefNumber(request);

                string callbackParam = _compositionHandler.GetURLRequestTokenString(request.Id, requestVM.ExpectedHash);

                CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, request.Id, serviceRevenueHeads, processStage.CategoryId, fileRefNumber, callbackParam);

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
                _compositionHandler.SaveServiceRequest(request, serviceRevenueHeads.Where(sr => !sr.IsGroupHead), processStage.ServiceId, response.InvoiceId, initLevel.Id, PSSRequestStatus.PendingInvoicePayment);

                ExtractDetails extractDetails = new ExtractDetails { Request = request, RequestReason = string.Empty, IsIncidentReported = requestVM.IsIncidentReported, AffidavitNumber = requestVM.AffidavitNumber, IncidentReportedDate = requestVM.IncidentReportedDate, AffidavitDateOfIssuance = requestVM.AffidavitDateOfIssuanceParsed };
                if (!_extractManager.Value.Save(extractDetails))
                { throw new CouldNotSaveRecord("Could not save extract details"); }

                SaveExtractCategoriesAndSubCategories(requestVM, request.Id, extractDetails.Id);

                if (requestVM.HasFileUpload)
                {
                    if (!_extractFilesManager.Save(new ExtractRequestFiles { ExtractDetails = extractDetails, FileName = requestVM.FileUploadName, FilePath = requestVM.FileUploadPath, ContentType = Util.GetFileContentType(requestVM.FileUploadName), Blob = Convert.ToBase64String(File.ReadAllBytes(requestVM.FileUploadPath)) }))
                    { throw new CouldNotSaveRecord("Could not save extract file details"); }
                }

                //Send a sms notification to the payer
                //_compositionHandler.SendInvoiceSMSNotification(new SMSDetailVM { RevenueHead = parentServicerevenueHead.ServiceName, Amount = response.AmountDue.ToString("F"), Name = taxPayerProfileVM.Recipient, PhoneNumber = (string.IsNullOrEmpty(request.ContactPersonPhoneNumber) ? taxPayerProfileVM.PhoneNumber : request.ContactPersonPhoneNumber), TaxEntityId = taxPayerProfileVM.Id, InvoiceNumber = response.InvoiceNumber });

                return response;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("{0} {1}", exception.Message, sRequestFormDump));
                _extractManager.Value.RollBackAllTransactions();
                throw;
            }
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
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, long requestId, IEnumerable<PSServiceRevenueHeadVM> result, int categoryId, string fileRefNumber, string callbackParam)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} {2}", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber),
                CallBackURL = PSSUtil.GetURLForFeeConfirmation(_orchardServices.WorkContext.CurrentSite.SiteName,callbackParam),
                TaxEntityCategoryId = categoryId,
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


        /// <summary>
        /// Saves extract categories and sub categories selection
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="requestId"></param>
        /// <param name="extractDetailsId"></param>
        private void SaveExtractCategoriesAndSubCategories(ExtractRequestVM requestVM, long requestId, long extractDetailsId)
        {
            List<ExtractCategoryVM> extractCategories = new List<ExtractCategoryVM>();

            foreach (var selectedCategoryAndSubCategory in requestVM.SelectedCategoriesAndSubCategoriesDeserialized)
            {
                extractCategories.AddRange(_extractCategoryManager.GetActiveSubCategoriesList(selectedCategoryAndSubCategory.Key));
            }

            var dataTable = new DataTable("Parkway_CBS_Police_Core_" + typeof(PSSRequestExtractDetailsCategory).Name);
            dataTable.Columns.Add(new DataColumn("Request_Id", typeof(long)));
            dataTable.Columns.Add(new DataColumn("ExtractDetails_Id", typeof(long)));
            dataTable.Columns.Add(new DataColumn("ExtractCategory_Id", typeof(int)));
            dataTable.Columns.Add(new DataColumn("ExtractSubCategory_Id", typeof(int)));
            dataTable.Columns.Add(new DataColumn("RequestReason", typeof(string)));
            dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

            foreach(var selectedCategory in extractCategories)
            {
                if (selectedCategory.FreeForm)
                {
                    var row = dataTable.NewRow();
                    row["Request_Id"] = requestId;
                    row["ExtractDetails_Id"] = extractDetailsId;
                    row["ExtractCategory_Id"] = selectedCategory.Id;
                    row["ExtractSubCategory_Id"] = 0;
                    row["RequestReason"] = string.Format("{0}:{1}", selectedCategory.Name, requestVM.Reason);
                    row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                    row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                    continue;
                }
                IEnumerable<int> selectedSubCategories;
                if(!requestVM.SelectedCategoriesAndSubCategoriesDeserialized.TryGetValue(selectedCategory.Id, out selectedSubCategories)) { throw new Exception("Unable to parse selected sub categories list"); }
                foreach(var subCategory in selectedCategory.SubCategories)
                {
                    if (selectedSubCategories.Contains(subCategory.Id))
                    {
                        var row = dataTable.NewRow();
                        row["Request_Id"] = requestId;
                        row["ExtractDetails_Id"] = extractDetailsId;
                        row["ExtractCategory_Id"] = selectedCategory.Id;
                        row["ExtractSubCategory_Id"] = subCategory.Id;
                        row["RequestReason"] = string.Format("{0}:{1}",selectedCategory.Name, subCategory.Name);
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    }
                }
            }


            if (!_pssRequestExtractDetailsCategoryManager.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(PSSRequestExtractDetailsCategory).Name))
            {
                throw new CouldNotSaveRecord("Could not save extract category and sub category selection");
            }

        }

    }
}