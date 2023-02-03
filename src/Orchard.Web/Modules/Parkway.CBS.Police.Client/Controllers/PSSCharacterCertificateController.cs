using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class PSSCharacterCertificateController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSCharacterCertificateHandler _handler;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreHelperService _corehelper;

        public PSSCharacterCertificateController(IHandler compHandler, IAuthenticationService authenticationService, IPSSCharacterCertificateHandler handler, IOrchardServices orchardServices, ICoreHelperService corehelper)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
            _orchardServices = orchardServices;
            _corehelper = corehelper;
        }


        public ActionResult CharacterCertificateRequest()
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
                CharacterCertificateRequestVM viewModel = _handler.GetVMForCharacterCertificate(processStage.ServiceId);

                viewModel.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
                viewModel.FlashObj = processStage.FlashObj ?? new FlashObj { };
                viewModel.HasMessage = processStage.FlashObj == null ? false : true;
                viewModel.ServiceName = processStage.ServiceName;
                viewModel.ServiceNote = processStage.ServiceNote;
                return View(viewModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in CharacterCertificateRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        [HttpPost]
        public ActionResult CharacterCertificateRequest(CharacterCertificateRequestVM userInput)
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

                _handler.ValidateCharacterCertificateRequest(userInput, out errors);
                CommandVM command = _handler.GetSelectedCommand(userInput.SelectedState, userInput.SelectedCommand, processStage.ServiceId, ref errors);
                ValidateAndSaveUploadedFiles(userInput, ref errors);
                if (errors.Count() > 0) { throw new DirtyFormDataException(); }

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
                    RequestType = userInput.RequestType
                };

                string pssExtractToken = Util.LetsEncrypt(JsonConvert.SerializeObject(pssRequestTokenObj));

                processStage.Token = pssExtractToken;

                dynamic routeAndStage = _handler.GetNextDirectionForConfirmation();

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
                CharacterCertificateRequestVM vm = _handler.GetVMForCharacterCertificate(processStage.ServiceId);
                userInput.Countries = vm.Countries;
                userInput.CharacterCertificateReasonsForInquiry = vm.CharacterCertificateReasonsForInquiry;
                userInput.StateLGAs = vm.StateLGAs;
                userInput.ListOfCommands = userInput.SelectedState > 0 ? _handler.GetListOfCommandsForState(userInput.SelectedState, processStage.ServiceId).ToList() : null;
                userInput.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
                userInput.ServiceName = processStage.ServiceName;
                userInput.ServiceNote = processStage.ServiceNote;
                userInput.ViewedTermsAndConditionsModal = true;
                userInput.RequestTypes = vm.RequestTypes;
                userInput.RequestType = userInput.RequestType;
                return View(userInput);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in CharacterCertificateRequest get {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().Text);
            }
            return RedirectToRoute("P.SelectService");
        }


        private void ValidateAndSaveUploadedFiles(CharacterCertificateRequestVM userInput, ref List<ErrorModel> errors)
        {
            string fileName = string.Empty;
            string path = string.Empty;
            string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.CharacterCertificateFilePath.ToString()).FirstOrDefault();
            if (node == null || string.IsNullOrEmpty(node.Value))
            {
                Logger.Error(string.Format("Unable to get character certificate file upload path in config file"));
                throw new Exception();
            }
            ICollection<UploadedFileAndName> Uploads = new List<UploadedFileAndName>();
            Uploads.Add(new UploadedFileAndName { Upload = HttpContext.Request.Files.Get("passportphotographfile"), UploadName = "PassportPhotographFile" });

            var internationalPassport = HttpContext.Request.Files.Get("intpassportdatapagefile");
            if (userInput.RequestType == (int)PCCRequestType.International)
            {
                Uploads.Add(new UploadedFileAndName { Upload = HttpContext.Request.Files.Get("intpassportdatapagefile"), UploadName = "IntPassportDatapageFile" });
            }
            else
            {
                if (internationalPassport != null && internationalPassport.ContentLength != 0)
                {
                    Uploads.Add(new UploadedFileAndName { Upload = HttpContext.Request.Files.Get("intpassportdatapagefile"), UploadName = "IntPassportDatapageFile" });
                }
            }

            foreach (var upload in Uploads)
            {
                if (upload.Upload == null || upload.Upload.ContentLength == 0)
                {
                    errors.Add(new ErrorModel { FieldName = upload.UploadName, ErrorMessage = $"{Util.InsertSpaceBeforeUpperCase(upload.UploadName)} is not specified" });
                }
            }

            _corehelper.CheckFileSize(Uploads.ToList(), errors, 2048);
            _corehelper.CheckFileType(Uploads.Where(x => x.UploadName == "PassportPhotographFile").ToList(), errors, new List<string> { "jpg", "png", "jpeg" }, new List<string> { ".jpg", ".png", ".jpeg" });
            _corehelper.CheckFileType(Uploads.Where(x => x.UploadName == "IntPassportDatapageFile").ToList(), errors, new List<string> { "jpg", "png", "jpeg", "pdf" }, new List<string> { ".jpg", ".png", ".jpeg", ".pdf" });
            //save uploaded files if no errors
            if (errors.Count > 0) { return; }
            List<string> paths = new List<string>();
            int counter = 0;
            foreach (var uploadedFile in Uploads)
            {
                fileName = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString() + ++counter + Path.GetExtension(uploadedFile.Upload.FileName);
                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value + siteName);
                path = Path.Combine(basePath.FullName, fileName);
                paths.Add(path);
                //save file
                uploadedFile.Upload.SaveAs(path);
            }
            //assign upload names and paths to request vm
            userInput.PassportPhotographUploadName = Uploads.ElementAt(0).Upload.FileName;
            userInput.PassportPhotographUploadPath = paths.ElementAt(0);
            if (internationalPassport != null && internationalPassport.ContentLength != 0)
            {
                userInput.InternationalPassportDataPageUploadName = Uploads.ElementAt(1).Upload.FileName;
                userInput.InternationalPassportDataPageUploadPath = paths.ElementAt(1);
            }
        }

    }
}