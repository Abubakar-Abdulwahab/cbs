using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.OSGOF.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.OSGOF.Admin.Controllers
{
    [Admin]
    public class OperatorController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public IOperatorHandler _handler;
        public ICellSitesHandler _cellSitesHandler;
        private Localizer T { get; }

        public OperatorController(IOrchardServices orchardServices, ICellSitesHandler cellSitesHandler, IOperatorHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            _cellSitesHandler = cellSitesHandler;
        }


        /// <summary>
        /// Get Tax Entity Categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddOperator()
        {
            try
            {
                SiteOperatorViewModel vm = new SiteOperatorViewModel();

                vm.Categories = _handler.GetTaxCategories();

                return View(vm);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                //_notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute("MDA.Dashboard");
            }
        }


        /// <summary>
        /// Save Payee details
        /// Create Payee as a CBS user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOperator(SiteOperatorViewModel model)
        {
            try
            {

               // _cellSitesHandler.CheckForPermission(Permissions.AddOperator);

                var response = _handler.CreateCBSUser(model);
                //TODO this would break here, you are not nulling the response, because
                //in the handler you are newing it, so for what ever reason, 
                //it would never enter this code branch
                if (response == null)
                {
                    _notifier.Add(NotifyType.Information, ErrorLang.genericexception());
                    return RedirectToRoute("MDA.Dashboard");
                }
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return RedirectToRoute("MDA.Dashboard");
            }
            catch (NoCategoryFoundException exception)
            {
                Logger.Error(exception.Message, exception);
                _notifier.Add(NotifyType.Error, ErrorLang.categorynotfound());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return RedirectToRoute("MDA.Dashboard");
        }

        [HttpGet]
        public ActionResult SearchOperator()
        {
            OperatorProfileVM vm = new OperatorProfileVM();
            vm.Operators = _handler.GetCellSiteOperators();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchOperator(string payerId)
        {
            try
            {
                if (!string.IsNullOrEmpty(payerId))
                {
                    return RedirectToRoute("AddCellSites", new { payerId });
                }
                Logger.Error(string.Format("Tax payer not found {0}", payerId));
                _notifier.Add(NotifyType.Error, ErrorLang.notaxpayerrecord404(payerId));
                return RedirectToRoute("MDA.Dashboard");
                //return RedirectToRoute("SearchOperator");
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute("MDA.Dashboard");
                //return RedirectToRoute("SearchOperator");
            }
        }
    }

}


