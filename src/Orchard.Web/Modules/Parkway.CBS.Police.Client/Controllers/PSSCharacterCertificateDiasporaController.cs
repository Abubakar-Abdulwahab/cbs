using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class PSSCharacterCertificateDiasporaController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPCCDiasporaHandler _handler;
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSCharacterCertificateHandler _pccHandler;

        public PSSCharacterCertificateDiasporaController(IHandler compHandler, IAuthenticationService authenticationService, IPCCDiasporaHandler handler, IOrchardServices orchardServices, IPSSCharacterCertificateHandler pccHandler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
            _orchardServices = orchardServices;
            _pccHandler = pccHandler;
        }


        public ActionResult CharacterCertificateDiasporaRequest()
        {
            string errorMessage = string.Empty;
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSCharacterCertificateRequest, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in CharacterCertificateRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                return View(GetViewModelForDiaspora(processStage));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in CharacterCertificateRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        /// <summary>
        /// GEt view model for diaspora form
        /// </summary>
        /// <param name="processStage"></param>
        /// <returns></returns>
        private PCCDiasporaUserInputVM GetViewModelForDiaspora(PSSRequestStageModel processStage)
        {
            PCCDiasporaUserInputVM viewModel = _handler.GetVMForCharacterCertificate(processStage.ServiceId, processStage.TaxEntityId, processStage.CategoryId);

            viewModel.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
            viewModel.FlashObj = processStage.FlashObj ?? new FlashObj { };
            viewModel.HasMessage = processStage.FlashObj == null ? false : true;
            viewModel.ServiceName = processStage.ServiceName;
            viewModel.ServiceNote = processStage.ServiceNote;
            return viewModel;
        }


        [HttpPost]
        public ActionResult CharacterCertificateDiasporaRequest(PCCDiasporaUserInputVM userInput)
        {
            string errorMessage = string.Empty;
            PSSRequestStageModel processStage = null;
            List<ErrorModel> errors = new List<ErrorModel>();
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);
                
                processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSCharacterCertificateRequest, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in ExtractRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                //check that this is diaspora
                if ((processStage.OptionType != nameof(CharacterCertificateOption.Diaspora)))
                {
                    Logger.Information(string.Format("Stage mismatch in CharacterCertificateDiasporaRequest. Stage is sess: option type {0}" + processStage.Stage, processStage.OptionType));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                _handler.ValidateCharacterCertificateRequest(userInput, ref errors);
                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                _handler.ValidateIdentity(userInput, ref errors, processStage.TaxEntityId, processStage.CategoryId);

                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                ICollection<UploadedFileAndName> uploads = new List<UploadedFileAndName>(4)
                {
                    new UploadedFileAndName { Upload = HttpContext.Request.Files.Get("passportPhotographFile"), UploadName = "PassportPhotographFile", MaxSize = 2048, AcceptedMimes = new List<string> { "jpg", "png", "jpeg" }, AcceptedExtensions =  new List<string> { ".jpg", ".png", ".jpeg" } },

                    new UploadedFileAndName { Upload = HttpContext.Request.Files.Get("intPassportDatapageFile"), UploadName = "IntPassportDatapageFile", MaxSize = 2048, AcceptedMimes = new List<string> { "jpg", "png", "jpeg", "pdf" }, AcceptedExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".pdf" } },

                    new UploadedFileAndName { Upload = HttpContext.Request.Files.Get("proofOfResidenceFile"), UploadName = "ProofOfResidenceFile", MaxSize = 2048, AcceptedMimes = new List<string> { "jpg", "png", "jpeg", "pdf" }, AcceptedExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".pdf" }},

                    //new UploadedFileAndName { Upload = HttpContext.Request.Files.Get("fingerPrintFile"), UploadName = "FingerPrintfile", MaxSize = 2048, AcceptedMimes = new List<string> { "pdf" }, AcceptedExtensions = new List<string> { ".pdf" } },
                };

                _handler.ValidateCharacterCertificateFileInput(userInput, uploads, ref errors);

                if (errors.Count() > 0) { throw new DirtyFormDataException(); }

                CommandVM command = _handler.GetCPCCRCommand();

                CharacterCertificateRequestVM pssRequestTokenObj = new CharacterCertificateRequestVM 
                {
                    CharacterCertificateReasonForInquiry = userInput.CharacterCertificateReasonForInquiry,
                    ReasonForInquiryValue = userInput.ReasonForInquiryValue,
                    SelectedCountryOfOrigin = userInput.SelectedCountryOfOrigin,
                    SelectedStateOfOrigin = userInput.SelectedStateOfOrigin,
                    SelectedStateOfOriginValue = userInput.SelectedStateOfOriginValue,
                    PlaceOfBirth = userInput.PlaceOfBirth,
                    DateOfBirth = userInput.DateOfBirth,
                    DateOfBirthParsed = userInput.DateOfBirthParsed,
                    DestinationCountry = userInput.DestinationCountry,
                    SelectedCountryOfPassport = userInput.SelectedCountryOfPassport,
                    PassportNumber = userInput.PassportNumber,
                    PlaceOfIssuance = userInput.PlaceOfIssuance,
                    DateOfIssuance = userInput.DateOfIssuance,
                    DateOfIssuanceParsed = userInput.DateOfIssuanceParsed,
                    PreviouslyConvicted = userInput.PreviouslyConvicted,
                    PreviousConvictionHistory = userInput.PreviousConvictionHistory,
                    SelectedState = userInput.SelectedState,
                    SelectedCommand = command.Id,
                    CommandName = command.Name,
                    CommandAddress = command.Address,
                    LGAName = command.LGAName,
                    StateName = command.StateName,
                    PassportPhotographUploadName = userInput.PassportPhotographUploadName,
                    PassportPhotographUploadPath = userInput.PassportPhotographUploadPath,
                    InternationalPassportDataPageUploadName = userInput.InternationalPassportDataPageUploadName,
                    InternationalPassportDataPageUploadPath = userInput.InternationalPassportDataPageUploadPath,
                    HasDifferentialWorkFlow = processStage.HasDifferentialWorkFlow,
                    RequestType = (int)PCCRequestType.International,
                };

                processStage.Token = Util.LetsEncrypt(JsonConvert.SerializeObject(pssRequestTokenObj));

                dynamic routeAndStage = _pccHandler.GetNextDirectionForConfirmation();

                processStage.Stage = routeAndStage.Stage;

                Session.Add("PSSRequestStage", JsonConvert.SerializeObject(processStage));

                return RedirectToRoute(routeAndStage.RouteName);

            }
            catch (DirtyFormDataException)
            {
                foreach(var error in errors)
                {
                    this.ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }
                
                PCCDiasporaUserInputVM vm = GetViewModelForDiaspora(processStage);
                userInput.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
                userInput.ServiceName = processStage.ServiceName;
                userInput.ServiceNote = processStage.ServiceNote;
                userInput.ViewedTermsAndConditionsModal = true;
                userInput.RequestType = userInput.RequestType;
                userInput.CharacterCertificateReasonsForInquiry = vm.CharacterCertificateReasonsForInquiry;
                userInput.Countries = vm.Countries;
                userInput.RequestTypes = vm.RequestTypes;
                userInput.Caveat = vm.Caveat;
                userInput.IdentityTypeList = vm.IdentityTypeList;

                return View(userInput);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in CharacterCertificateRequest get {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().Text);
            }
            return RedirectToRoute("P.SelectService");
        }
        
    }
}