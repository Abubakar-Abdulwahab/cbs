using Orchard.UI.Admin;
using System.Web.Mvc;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers
{
    [Admin]
    public class PoliceOfficerSchedulingRequestController : Controller
    {
        public ActionResult RequestDetails()
        {
            return View();
        }
    }
}