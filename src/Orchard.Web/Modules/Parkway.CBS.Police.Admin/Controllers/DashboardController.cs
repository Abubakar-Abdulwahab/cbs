using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class DashboardController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly IDashboardHandler _handler;


        public DashboardController(IOrchardServices orchardServices, IDashboardHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _handler = handler;
        }

        /// <summary>
        /// Get POSSAP Dashoboard
        /// </summary>
        /// <returns></returns>
        public ActionResult MainDashboard()
        {
            try
            {
                return View(_handler.GetDashboardView());
            }
            #region catch clauses
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
            }
            #endregion
            return RedirectToAction("PSSRequestList", "RequestApproval", new { });
        }

    }
}