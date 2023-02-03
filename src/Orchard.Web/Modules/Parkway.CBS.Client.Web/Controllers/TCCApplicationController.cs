using Orchard;
using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [Themed]
    public class TCCApplicationController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ITCCApplicationHandler _handler;
        private readonly IOrchardServices _orchardServices;
        private readonly ICommonHandler _commonHandler;

        public TCCApplicationController(ITCCApplicationHandler handler, IOrchardServices orchardServices, ICommonHandler commonHandler)
        {
            Logger = NullLogger.Instance;
            _handler = handler;
            _orchardServices = orchardServices;
            _commonHandler = commonHandler;
        }

        [BrowserHeaderFilter]
        public ActionResult TCCApplication()
        {
            string message = string.Empty;
            TCCApplicationRequestVM viewModel = new TCCApplicationRequestVM();
            try
            {
                try
                {
                    if (TempData.ContainsKey("Message"))
                    {
                        message = TempData["Message"].ToString();
                        TempData.Remove("Message");
                    }
                }
                catch (Exception) { }

                HeaderObj headerObj = _commonHandler.GetHeaderObj();
                if (!headerObj.IsLoggedIn)
                {
                    TempData.Add("Error", ErrorLang.requiressignin().ToString());
                    return RedirectToRoute("C.SignIn");
                }

                viewModel.Message = message;
                viewModel.HeaderObj = headerObj;
                return View(viewModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Unable to load application form " + exception.Message);
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute("C.SelfAssessment");
            }
        }

        [HttpPost, ActionName("TCCApplication")]
        public ActionResult TCCApplication(TCCApplicationRequestVM model)
        {
            List<ErrorModel> validationErrors = new List<ErrorModel> { };
            try
            {
                if (!this.TryValidateModel(model)) { throw new DirtyFormDataException(); }

                string message = string.Empty;
                model.HeaderObj = _commonHandler.GetHeaderObj();
                StateConfig siteConfig = null;

                if (model.ApplicationYear < (DateTime.Now.Year - 20) || model.ApplicationYear >= DateTime.Now.Year)
                {
                    model.HasErrors = true;
                    model.ErrorMessage = "Application year not valid.";
                    return View(model);
                }

                ////do form validation
                _handler.DoValidationForFormFields(model, validationErrors);
                if (validationErrors.Count > 0)
                {
                    model.HasErrors = true;
                    foreach (var err in validationErrors)
                    {
                        model.ErrorMessage += @"" + err.ErrorMessage + "\n";
                    }
                    return View(model);
                }

                //Confirm file upload
                var accountStatement = HttpContext.Request.Files.Get("accountstatement");
                var exemptionCertificate = HttpContext.Request.Files.Get("exemptioncertificate");
                var schoolCertificate = HttpContext.Request.Files.Get("schoolcertificate");

                _handler.DoFileUploadValidation(accountStatement, exemptionCertificate, schoolCertificate, model.ExemptionTypeId, validationErrors);
                if (validationErrors.Count > 0)
                {
                    model.HasErrors = true;
                    foreach (var err in validationErrors)
                    {
                        model.ErrorMessage += @"" + err.ErrorMessage + "\n";
                    }
                    return View(model);
                }

                model.TaxEntityId = _handler.ValidateStateTIN(model.StateTIN).Id;

                int developmentLevyRevenueHeadId = 0;
                siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.DevelopmentLevyRevenueHeadID.ToString())?.FirstOrDefault();
                if (node == null) { throw new Exception("Unable to fetch DevelopmentLevyRevenueHeadID from StateConfig in TCCApplicationController"); }
                if (!int.TryParse(node.Value, out developmentLevyRevenueHeadId))
                {
                    Logger.Error(string.Format("Unable to convert configured Development Levy Revenue head config value"));
                    model.HasErrors = true;
                    model.ErrorMessage = $"Unable to get Development Levy details";
                    return View(model);
                }

                if (_handler.CheckDevelopmentLevyInvoiceUsage(model.DevelopmentLevyInvoice))
                {
                    model.HasErrors = true;
                    model.ErrorMessage = $"Invoice {model.DevelopmentLevyInvoice} is no longer valid";
                    return View(model);
                }

                model.DevelopmentLevyInvoiceId = _handler.ValidateDevelopmentLevyInvoice(model.DevelopmentLevyInvoice, developmentLevyRevenueHeadId);
                if (model.DevelopmentLevyInvoiceId < 1)
                {
                    model.HasErrors = true;
                    model.ErrorMessage = $"Invalid development levy invoice ({model.DevelopmentLevyInvoice})";
                    return View(model);
                }

                //Save the request
                _handler.SaveTCCRequest(model, accountStatement, exemptionCertificate, schoolCertificate, siteConfig);
                TempData.Add("Message", $"Application submitted successfully");
                return RedirectToRoute(RouteName.TCCRequestHistory.RequestHistory);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, string.Format("Exception in get {0}", exception.Message));
                model.HasErrors = true;
                model.ErrorMessage = exception.Message;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in get {0}", exception.Message));
                model.HasErrors = true;
                model.ErrorMessage = ErrorLang.genericexception().ToString();
            }
            return View(model);
        }
    }
}