using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System.Globalization;
using System.IO;
using System.Web;
using Orchard;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAuthorized]
    public class PSSExtractController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSExtractHandler _handler;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreHelperService _corehelper;


        public PSSExtractController(IHandler compHandler, IAuthenticationService authenticationService, IPSSExtractHandler handler, IOrchardServices orchardServices, ICoreHelperService coreh)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
            _orchardServices = orchardServices;
            _corehelper = coreh;
        }


        /// <summary>
        /// Route Name: P.ExtractRequest
        /// </summary>
        public ActionResult ExtractRequest()
        {
            string errorMessage = string.Empty;
            string categoryType = string.Empty;
            ExtractRequestVM viewModel = new ExtractRequestVM();

            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSExtractRequest, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in ExtractRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                viewModel = _handler.GetVMForPoliceExtract(processStage.ServiceId);
                UserDetailsModel userDetails = GetLoggedInUserDetails();

                if (userDetails != null)
                {
                    viewModel.HeaderObj.IsLoggedIn = true;
                    viewModel.HeaderObj.ShowSignin = true;
                    viewModel.HeaderObj.DisplayText = userDetails.Name;
                }
                viewModel.FlashObj = processStage.FlashObj ?? new FlashObj { };
                viewModel.HasMessage = processStage.FlashObj == null ? false : true;
                viewModel.ServiceName = processStage.ServiceName;
                viewModel.ServiceNote = processStage.ServiceNote;
                return View(viewModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ExtractRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        [HttpPost]
        /// <summary>
        /// Route Name: P.ExtractRequest
        /// </summary>
        public ActionResult ExtractRequest(ExtractRequestVM userInput)
        {
            string errorMessage = string.Empty;
            UserDetailsModel userDetails = GetLoggedInUserDetails();
            PSSRequestStageModel processStage = null;
            try
            {
                //throw new DirtyFormDataException();
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                processStage = GetDeserializedSessionObj(ref errorMessage);
                var courtAffidavitFile = HttpContext.Request.Files.Get("courtaffidavitfile");

                if (!IsStageCorrect(PSSUserRequestGenerationStage.PSSExtractRequest, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in ExtractRequest. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                //do validation
                if (userInput.SelectedState <= 0) { this.ModelState.AddModelError("SelectedState", "State is required."); throw new DirtyFormDataException(); }
                //if (userInput.SelectedStateLGA <= 0) { this.ModelState.AddModelError("SelectedStateLGA", "State LGA is required."); throw new DirtyFormDataException(); }

                if (userInput.SelectedCommand <= 0)
                {
                    this.ModelState.AddModelError("SelectedCommand", "Select a valid Command."); throw new DirtyFormDataException();
                }

                if (userInput.IsIncidentReported)
                {
                    //if (string.IsNullOrEmpty(userInput.IncidentReportedDate)) { this.ModelState.AddModelError("IncidentReportedDate", "Incident Reported Date is required."); throw new DirtyFormDataException(); }
                    if (!string.IsNullOrEmpty(userInput.IncidentReportedDate))
                    {
                        DateTime incidentReportedDate = new DateTime { };
                        try
                        {
                            incidentReportedDate = DateTime.ParseExact(userInput.IncidentReportedDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            this.ModelState.AddModelError("IncidentReportedDate", "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020."); throw new DirtyFormDataException();
                        }

                        if (incidentReportedDate > DateTime.Now) { this.ModelState.AddModelError("IncidentReportedDate", "Incident Reported Date cannot be a future date."); throw new DirtyFormDataException(); }
                    }
                    if (courtAffidavitFile != null && courtAffidavitFile.ContentLength != 0)
                    {
                        ValidateCourtAffidavitFile(courtAffidavitFile);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(userInput.AffidavitNumber))
                    {
                        if (_handler.CheckIfExistingAffidavitNumber(userInput.AffidavitNumber?.Trim(), userDetails.Entity.Id))
                        {
                            ModelState.AddModelError("AffidavitNumber", "Affidavit Number already exist."); throw new DirtyFormDataException();
                        }
                    }

                    if (string.IsNullOrEmpty(userInput.AffidavitDateOfIssuance)) { this.ModelState.AddModelError("AffidavitDateOfIssuance", "Affidavit Date of Issaunce is required."); throw new DirtyFormDataException(); }
                    else
                    {
                        try
                        {
                            userInput.AffidavitDateOfIssuanceParsed = DateTime.ParseExact(userInput.AffidavitDateOfIssuance.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            this.ModelState.AddModelError("AffidavitDateOfIssuance", "Please input a valid date. Expected date format dd/MM/yyyy i.e. 31/09/2020."); throw new DirtyFormDataException();
                        }

                        if (userInput.AffidavitDateOfIssuanceParsed > DateTime.Now) { this.ModelState.AddModelError("AffidavitDateOfIssuance", "Affidavit Date of Issuance cannot be a future date."); throw new DirtyFormDataException(); }
                    }

                    ValidateCourtAffidavitFile(courtAffidavitFile);
                }

                //do validation for extract category

                //string reason = _handler.ValidateExtractCategory(this, userInput.SelectedCategory, userInput.SelectedSubCategory, userInput.Reason);
                _handler.ValidateExtractCategoriesAndSubCategories(this, userInput, userInput.Reason);
                string reason = userInput.Reason;

                CommandVM command = _handler.ValidateSelectedCommand(this, userInput.SelectedState, userInput.SelectedStateLGA, userInput.SelectedCommand);

                string fileName = string.Empty;
                string path = string.Empty;
                bool hasFileUpload = false;
                if (courtAffidavitFile != null && courtAffidavitFile.ContentLength != 0)
                {
                    string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                    fileName = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString() + Path.GetExtension(courtAffidavitFile.FileName);
                    StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
                    Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.ExtractFilePath.ToString()).FirstOrDefault();
                    if (node == null || string.IsNullOrEmpty(node.Value))
                    {
                        Logger.Error(string.Format("Unable to get extract file upload path in a config file"));
                        this.ModelState.AddModelError("CourtAffidavitFile", "Error uploading court affidavit file.");
                        throw new DirtyFormDataException();
                    }

                    DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value + siteName);
                    path = Path.Combine(basePath.FullName, fileName);
                    //save file
                    courtAffidavitFile.SaveAs(path);
                    hasFileUpload = true;
                }

                ExtractRequestVM pssRequestTokenObj = new ExtractRequestVM { SelectedStateLGA = userInput.SelectedStateLGA, SelectedState = userInput.SelectedState, SelectedCommand = userInput.SelectedCommand, Reason = reason, LGAName = command.LGAName, StateName = command.StateName, CommandName = command.Name, CommandAddress = command.Address, SelectedCategoriesAndSubCategoriesDeserialized = userInput.SelectedCategoriesAndSubCategoriesDeserialized, AffidavitNumber = userInput.AffidavitNumber, IsIncidentReported = userInput.IsIncidentReported, IncidentReportedDate = userInput.IncidentReportedDate, FileUploadName = fileName, FileUploadPath = path, HasFileUpload = hasFileUpload, HasDifferentialWorkFlow = processStage.HasDifferentialWorkFlow, AffidavitDateOfIssuance = userInput.AffidavitDateOfIssuance, AffidavitDateOfIssuanceParsed = userInput.AffidavitDateOfIssuanceParsed };

                string pssExtractToken = Util.LetsEncrypt(JsonConvert.SerializeObject(pssRequestTokenObj));

                processStage.Token = pssExtractToken;

                dynamic routeAndStage = _handler.GetNextDirectionForConfirmation();

                processStage.Stage = routeAndStage.Stage;

                Session.Add("PSSRequestStage", JsonConvert.SerializeObject(processStage));

                return RedirectToRoute(routeAndStage.RouteName);
            }
            catch (DirtyFormDataException)
            {
                List<string> errorArray = new List<string>();
                foreach (var item in this.ModelState.Keys)
                {
                    var modelState = this.ModelState[item];
                    if (modelState.Errors.Count > 0 && modelState.Value != null) { errorArray.Add(modelState.Errors.FirstOrDefault().ErrorMessage); }
                }
                ExtractRequestVM vm = _handler.GetVMForPoliceExtract(processStage.ServiceId);
                vm.SelectedCommand = userInput.SelectedCommand;
                vm.SelectedState = userInput.SelectedState;
                vm.SelectedStateLGA = userInput.SelectedStateLGA;
                vm.ShowFreeForm = userInput.ShowFreeForm;
                vm.IsIncidentReported = userInput.IsIncidentReported;
                vm.IncidentReportedDate = userInput.IncidentReportedDate;
                vm.AffidavitNumber = userInput.AffidavitNumber;
                vm.AffidavitDateOfIssuance = userInput.AffidavitDateOfIssuance;
                vm.ViewedTermsAndConditionsModal = true;
                vm.SelectedCategories = userInput.SelectedCategories;
                vm.SelectedSubCategories = userInput.SelectedSubCategories;
                vm.SelectedCategoriesAndSubCategories = userInput.SelectedCategoriesAndSubCategories;
                if (userInput.SelectedState > 0)
                {
                    var state = vm.StateLGAs.Where(s => s.Id == userInput.SelectedState).SingleOrDefault();
                    vm.ListLGAs = state?.LGAs.ToList();
                }

                if (userInput.SelectedStateLGA > 0)
                {
                    vm.ListOfCommands = _handler.GetListOfCommands(userInput.SelectedStateLGA);
                }
                if (userInput.SelectedStateLGA == 0 && userInput.SelectedState > 0)
                {
                    vm.ListOfCommands = _handler.GetListOfCommandsByStateId(userInput.SelectedState);
                }

                if (userInput.SelectedCategories.Count() > 0)
                {
                    vm.SelectedExtractCategoriesWithSubCategories = _handler.GetExtractCategoriesList(vm.SelectedCategories).ToList();
                }

                vm.HeaderObj = HeaderFiller(userDetails);
                vm.Reason = userInput.Reason;
                vm.ServiceName = processStage.ServiceName;
                vm.ServiceNote = processStage.ServiceNote;
                return View(vm);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ExtractRequest get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }

        /// <summary>
        /// Validate court affidavit file and save at a path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="isFileRequired"></param>
        private void ValidateCourtAffidavitFile(HttpPostedFileBase file)
        {
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();

            { filesAndFileNames.Add(new UploadedFileAndName { Upload = file, UploadName = "CourtAffidavitFile" }); }
            List<ErrorModel> errors = new List<ErrorModel> { };
            _corehelper.CheckFileSize(filesAndFileNames, errors);
            _corehelper.CheckFileType(filesAndFileNames, errors, new List<string> { "jpg", "png", "gif", "jpeg", "pdf" }, new List<string> { ".jpg", ".png", ".gif", ".jpeg", ".pdf" });

            if (errors.Count > 0)
            {
                this.ModelState.AddModelError("CourtAffidavitFile", "Uploaded court affidavit file format not supported. Only .pdf,.png, .jpeg and .jpg are supported.");
                throw new DirtyFormDataException();
            }
        }
    }
}