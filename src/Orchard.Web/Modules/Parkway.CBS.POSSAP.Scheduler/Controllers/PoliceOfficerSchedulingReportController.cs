using Orchard.UI.Admin;
using System.Web.Mvc;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers
{
    [Admin]
    public class PoliceOfficerSchedulingReportController : Controller
    {
        public ActionResult Report()
        {
            return View();
        }
    }
}