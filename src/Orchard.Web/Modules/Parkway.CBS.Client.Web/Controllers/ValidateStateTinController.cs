using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Client.Web.RouteName;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [Themed]
    public class ValidateStateTinController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IStateTINValidationHandler _handler;
        private readonly ICommonHandler _commonHandler;

        public ValidateStateTinController(IStateTINValidationHandler handler, ICommonHandler commonHandler)
        {
            Logger = NullLogger.Instance;
            _handler = handler;
            _commonHandler = commonHandler;
        }

        // GET: ValidateStateTin
        [BrowserHeaderFilter]
        public ActionResult ValidateStateTin()
        {
            string message = string.Empty;
            bool hasError = false;
            string stateTIN = null;
            try
            {
                if (TempData.ContainsKey("NoStateTIN"))
                {
                    stateTIN = TempData["NoStateTIN"].ToString();
                    message = ErrorLang.stateTIN404(stateTIN).ToString();
                    hasError = true;
                    TempData.Remove("NoStateTIN");
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Unable to load state TIN validation form " + exception.Message);
                hasError = true;
            }
            StateTINValidationVM viewModel = new StateTINValidationVM { HeaderObj = _commonHandler.GetHeaderObj() , HasErrors = hasError, ErrorMessage = message };
            return View(viewModel);
        }

        [HttpPost, ActionName("ValidateStateTin")]
        public ActionResult ValidateStateTin(StateTINValidationVM model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.StateTIN))
                {
                    return RedirectToRoute(StateTINValidation.StateTINDetails, new { stateTIN = model.StateTIN });
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in get {0}", exception.Message));
                model.HasErrors = true;
                model.ErrorMessage = ErrorLang.genericexception().ToString();
            }

            Logger.Error("State TIN not found " + model.StateTIN);
            model.HasErrors = true;
            model.ErrorMessage = ErrorLang.stateTIN404(model.StateTIN).ToString();
            return View();
        }


        [BrowserHeaderFilter]
        public ActionResult StateTINDetails(string stateTIN)
        {
            try
            {
                TaxPayerWithDetails stateTINDetails = _handler.ValidateStateTIN(stateTIN);
                return View(new StateTINValidationVM { HeaderObj = _commonHandler.GetHeaderObj(), StateTINViewModel = stateTINDetails, StateTIN = stateTIN });
            }
            catch (NoRecordFoundException ex)
            {
                Logger.Error(ex, string.Format("No record found for State TIN {0} {1}", stateTIN, ex.Message));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error searching for State TIN {0} {1}", stateTIN, exception.Message));
            }

            Logger.Error("State TIN not found " + stateTIN);
            TempData = null;
            TempData.Add("NoStateTIN", stateTIN);
            return RedirectToRoute(StateTINValidation.ValidateStateTIN);
        }


    }
}