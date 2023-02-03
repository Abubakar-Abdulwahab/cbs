using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class CharacterCertificate : IPSSServiceTypeImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.CharacterCertificate;

        private readonly Lazy<IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>> _characterCertificateManager;
        private readonly Lazy<IPSSCharacterCertificateDetailsLogManager<PSSCharacterCertificateDetailsLog>> _characterCertificateDetailsLogManager;

        private readonly Lazy<IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob>> _characterCertificateDetailsBlobManager;
        private readonly Lazy<IPSSCharacterCertificateDetailsBlobLogManager<PSSCharacterCertificateDetailsBlobLog>> _characterCertificateDetailsBlobLogManager;

        private readonly ICountryManager<Country> _countryManager;
        private readonly ITypeImplComposer _compositionHandler;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSCharacterCertificateRequestTypeManager<PSSCharacterCertificateRequestType> _requestTypeManager;

        public CharacterCertificate(Lazy<IPSSCharacterCertificateDetailsManager<PSSCharacterCertificateDetails>> characterCertificateManager, ICountryManager<Country> countryManager, ITypeImplComposer compositionHandler, IOrchardServices orchardServices, IPSSCharacterCertificateRequestTypeManager<PSSCharacterCertificateRequestType> requestTypeManager, Lazy<IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob>> characterCertificateDetailsBlobManager, Lazy<IPSSCharacterCertificateDetailsLogManager<PSSCharacterCertificateDetailsLog>> characterCertificateDetailsLogManager, Lazy<IPSSCharacterCertificateDetailsBlobLogManager<PSSCharacterCertificateDetailsBlobLog>> characterCertificateDetailsBlobLogManager)
        {
            _characterCertificateManager = characterCertificateManager;
            _countryManager = countryManager;
            _compositionHandler = compositionHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _requestTypeManager = requestTypeManager;
            _characterCertificateDetailsBlobManager = characterCertificateDetailsBlobManager;
            _characterCertificateDetailsLogManager = characterCertificateDetailsLogManager;
            _characterCertificateDetailsBlobLogManager = characterCertificateDetailsBlobLogManager;
        }


        /// <summary>
        /// This method gets the model for displaying the confirmation details of a request 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="objStringValue"></param>
        /// <returns>RequestConfirmationVM</returns>
        public RequestConfirmationVM GetModelForRequestConfirmation(int serviceId, string objStringValue)
        {
            CharacterCertificateRequestVM requestVM = JsonConvert.DeserializeObject<CharacterCertificateRequestVM>(objStringValue);
            if (requestVM == null) { throw new Exception("No session value found for CharacterCertificateRequestVM"); }
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

            dynamic model = new ExpandoObject();
            model.ReasonForInquiryValue = requestVM.ReasonForInquiryValue;
            model.StateOfOrigin = requestVM.SelectedStateOfOriginValue;
            model.PlaceOfBirth = requestVM.PlaceOfBirth;
            model.DateOfBirth = requestVM.DateOfBirth;
            model.DestinationCountry = requestVM.DestinationCountry > 0 ? _countryManager.GetCountry(requestVM.DestinationCountry).Name : null;
            model.PassportNumber = requestVM.PassportNumber;
            model.PlaceOfIssuance = requestVM.PlaceOfIssuance;
            model.DateOfIssuance = requestVM.DateOfIssuance;
            model.PreviouslyConvicted = requestVM.PreviouslyConvicted;
            model.PreviousConvictionHistory = requestVM.PreviousConvictionHistory;
            model.RequestType = _requestTypeManager.GetRequestType(requestVM.RequestType).Name;
            model.CountryOfOrigin = _countryManager.GetCountry(requestVM.SelectedCountryOfOrigin).Name;
            model.CountryOfPassport = requestVM.SelectedCountryOfPassport > 0 ? _countryManager.GetCountry(requestVM.SelectedCountryOfPassport).Name : null;

            return new RequestConfirmationVM
            {
                AmountDetails = result.Where(r => !r.IsGroupHead).Select(r => new AmountDetails { AmountToPay = (r.AmountToPay + r.Surcharge), FeeDescription = r.FeeDescription }).ToList(),
                HeaderObj = new HeaderObj { },
                NameOfPoliceCommand = string.Format("{0} ({1}, {2}, {3})", requestVM.CommandName, requestVM.CommandAddress, requestVM.LGAName, requestVM.StateName),
                ServiceRequested = result.ElementAt(0).ServiceName,
                Reason = requestVM.Reason,
                PartialName = "CharacterCertificateConfirmationPartial",
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
                CharacterCertificateRequestVM requestVM = JsonConvert.DeserializeObject<CharacterCertificateRequestVM>(sRequestFormDump);
                if (requestVM == null) { throw new Exception("No session value found for CharacterCertificateRequestVM"); }

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
                //save command into request command table
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

                SaveCharacterCertificateDetails(requestVM, request);

                //Send a sms notification to the payer
                //_compositionHandler.SendInvoiceSMSNotification(new SMSDetailVM { RevenueHead = parentServicerevenueHead.ServiceName, Amount = response.AmountDue.ToString("F"), Name = taxPayerProfileVM.Recipient, PhoneNumber = (string.IsNullOrEmpty(request.ContactPersonPhoneNumber) ? taxPayerProfileVM.PhoneNumber : request.ContactPersonPhoneNumber), TaxEntityId = taxPayerProfileVM.Id, InvoiceNumber = response.InvoiceNumber });

                return response;

            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("{0} {1}", exception.Message, sRequestFormDump));
                _characterCertificateManager.Value.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Save the details for character certificate, save the details and the blob entries along with the log entries
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="request"></param>
        private void SaveCharacterCertificateDetails(CharacterCertificateRequestVM requestVM, PSSRequest request)
        {
            PSSCharacterCertificateDetails characterCertificateDetails = GetCharacterCertificateDetails<PSSCharacterCertificateDetails>(requestVM, request);

            if (!_characterCertificateManager.Value.Save(characterCertificateDetails))
            { throw new CouldNotSaveRecord("Could not save character certificate details"); }

            //save logs
            PSSCharacterCertificateDetailsLog certDetailslogEntry = SaveCharacterCertificateDetailsLogs(requestVM, request, characterCertificateDetails);

            PSSCharacterCertificateDetailsBlob characterCertificateDetailsBlob = GetCharacterCertificateDetailsBlob<PSSCharacterCertificateDetailsBlob>(requestVM, request, characterCertificateDetails);

            if (!_characterCertificateDetailsBlobManager.Value.Save(characterCertificateDetailsBlob))
            { throw new CouldNotSaveRecord("Could not save character certificate details blob"); }

            SaveCharacterCertifcateDetailsBlobLogs(requestVM, request, characterCertificateDetails, certDetailslogEntry, characterCertificateDetailsBlob);

        }


        /// <summary>
        /// Save blob log
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="request"></param>
        /// <param name="characterCertificateDetails"></param>
        /// <param name="certDetailsLogEntry"></param>
        /// <param name="certBlobCetailsLogEntry"></param>
        private void SaveCharacterCertifcateDetailsBlobLogs(CharacterCertificateRequestVM requestVM, PSSRequest request, PSSCharacterCertificateDetails characterCertificateDetails, PSSCharacterCertificateDetailsLog certDetailsLogEntry, PSSCharacterCertificateDetailsBlob certBlobCetailsLogEntry)
        {
            PSSCharacterCertificateDetailsBlobLog blobLog = GetCharacterCertificateDetailsBlob<PSSCharacterCertificateDetailsBlobLog>(requestVM, request, characterCertificateDetails);
            blobLog.PSSCharacterCertificateDetailsLog = certDetailsLogEntry;
            blobLog.PSSCharacterCertificateDetailsBlob = certBlobCetailsLogEntry;

            if (!_characterCertificateDetailsBlobLogManager.Value.Save(blobLog))
            { throw new CouldNotSaveRecord("Could not save character certificate blob details log"); }
        }


        /// <summary>
        /// Save certificate details log
        /// </summary>
        /// <param name="requestVM"></param>
        /// <param name="request"></param>
        /// <param name="certDetails"></param>
        /// <returns>PSSCharacterCertificateDetailsLog</returns>
        private PSSCharacterCertificateDetailsLog SaveCharacterCertificateDetailsLogs(CharacterCertificateRequestVM requestVM, PSSRequest request, PSSCharacterCertificateDetails certDetails)
        {
            PSSCharacterCertificateDetailsLog log = GetCharacterCertificateDetails<PSSCharacterCertificateDetailsLog>(requestVM, request);
            log.PSSCharacterCertificateDetails = certDetails;
            
            if (!_characterCertificateDetailsLogManager.Value.Save(log))
            { throw new CouldNotSaveRecord("Could not save character certificate details log"); }

            return log;
        }


        /// <summary>
        /// This method instantiates T which inherits from the PSSCharacterCertificateDetailsBaseClass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestVM"></param>
        /// <param name="request"></param>
        /// <param name="characterCertificateDetails"></param>
        /// <returns>T where T inherits from PSSCharacterCertificateDetailsBaseClass</returns>
        private T GetCharacterCertificateDetailsBlob<T>(CharacterCertificateRequestVM requestVM, PSSRequest request, PSSCharacterCertificateDetails characterCertificateDetails) where T : PSSCharacterCertificateDetailsBlobBase, new()
        {
            return new T
            {
                Request = request,
                PSSCharacterCertificateDetails = new PSSCharacterCertificateDetails { Id = characterCertificateDetails.Id },
                PassportPhotographContentType = Util.GetFileContentType(requestVM.PassportPhotographUploadName),
                PassportPhotographFilePath = requestVM.PassportPhotographUploadPath,
                PassportPhotographOriginalFileName = requestVM.PassportPhotographUploadName,
                PassportPhotographBlob = Convert.ToBase64String(File.ReadAllBytes(requestVM.PassportPhotographUploadPath)),
                InternationalPassportDataPageContentType = string.IsNullOrEmpty(requestVM.InternationalPassportDataPageUploadName) ? null : Util.GetFileContentType(requestVM.InternationalPassportDataPageUploadName),
                InternationalPassportDataPageFilePath = string.IsNullOrEmpty(requestVM.InternationalPassportDataPageUploadPath) ? null : requestVM.InternationalPassportDataPageUploadPath,
                InternationalPassportDataPageOriginalFileName = string.IsNullOrEmpty(requestVM.InternationalPassportDataPageUploadName) ? null : requestVM.InternationalPassportDataPageUploadName,
                InternationalPassportDataPageBlob = string.IsNullOrEmpty(requestVM.InternationalPassportDataPageUploadPath) ? null : Convert.ToBase64String(File.ReadAllBytes(requestVM.InternationalPassportDataPageUploadPath)),
            };
        }


        /// <summary>
        /// This method instantiates T which inherits from PSSCharacterCertificateDetailsBase
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestVM"></param>
        /// <param name="request"></param>
        /// <returns>T instance</returns>
        private T GetCharacterCertificateDetails<T>(CharacterCertificateRequestVM requestVM, PSSRequest request) where T : PSSCharacterCertificateDetailsBase, new()
        {
            return new T
            {
                Request = request,
                Reason = new CharacterCertificateReasonForInquiry { Id = requestVM.CharacterCertificateReasonForInquiry },
                ReasonValue = requestVM.ReasonForInquiryValue,
                StateOfOrigin = requestVM.SelectedStateOfOrigin > 0 ? new StateModel { Id = requestVM.SelectedStateOfOrigin } : null,
                StateOfOriginValue = requestVM.SelectedStateOfOrigin > 0 ? requestVM.SelectedStateOfOriginValue : null,
                PlaceOfBirth = requestVM.PlaceOfBirth,
                DateOfBirth = requestVM.DateOfBirthParsed,
                DestinationCountry = requestVM.DestinationCountry > 0 ? new Country { Id = requestVM.DestinationCountry } : null,
                DestinationCountryValue = requestVM.DestinationCountry > 0 ? _countryManager.GetCountry(requestVM.DestinationCountry).Name : null,
                PassportNumber = requestVM.PassportNumber,
                PlaceOfIssuance = requestVM.PlaceOfIssuance,
                DateOfIssuance = requestVM.DateOfIssuanceParsed,
                PreviouslyConvicted = requestVM.PreviouslyConvicted,
                PreviousConvictionHistory = requestVM.PreviousConvictionHistory,
                RequestType = new PSSCharacterCertificateRequestType { Id = requestVM.RequestType },
                CountryOfPassport = requestVM.SelectedCountryOfPassport > 0 ? new Country { Id = requestVM.SelectedCountryOfPassport } : null,
                CountryOfPassportValue = requestVM.SelectedCountryOfPassport > 0 ? _countryManager.GetCountry(requestVM.SelectedCountryOfPassport).Name : null,
                CountryOfOrigin = new Country { Id = requestVM.SelectedCountryOfOrigin },
                CountryOfOriginValue = _countryManager.GetCountry(requestVM.SelectedCountryOfOrigin).Name,
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
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, long requestId, IEnumerable<PSServiceRevenueHeadVM> result, int categoryId, string fileRefNumber, string callbackParam)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} {2}", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber),
                CallBackURL = PSSUtil.GetURLForFeeConfirmation(_orchardServices.WorkContext.CurrentSite.SiteName, callbackParam),
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

    }
}