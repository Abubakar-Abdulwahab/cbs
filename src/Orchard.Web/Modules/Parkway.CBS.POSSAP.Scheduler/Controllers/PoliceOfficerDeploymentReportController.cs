using Orchard.UI.Admin;
using System.Web.Mvc;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers
{
    [Admin]
    public class PoliceOfficerDeploymentReportController : Controller
    {

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Create(string name)
        {
            //Persons.Add(new Person { Name = name, Contact = contact });
            return View();
        }

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult ReplaceOfficer()
        {
            return View();
        }

        public ActionResult EndDeployment()
        {
            return View();
        }

        public ActionResult DeploymentHistory()
        {
            return View();
        }
    }
}