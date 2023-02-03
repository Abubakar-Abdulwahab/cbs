using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class PSSBranchesAJAXController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSBranchesHandler _handler;
        public PSSBranchesAJAXController(IPSSBranchesHandler handler, IAuthenticationService authenticationService, IHandler compHandler) : base(authenticationService, compHandler)
        {
            _authenticationService = authenticationService;
            _compHandler = compHandler;
            _handler = handler;
            Logger = NullLogger.Instance;
        }

        public JsonResult PSSBranchesMoveRight(string token, int? page)
        {
            try
            {
                Logger.Information(string.Format("getting pss branches page data for batch token - {0} page - {1}", "", page.ToString()));
                return Json(_handler.GetPagedBranchesData(token, page, GetLoggedInUserDetails().TaxPayerProfileVM.Id), JsonRequestBehavior.AllowGet);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = new { Message = ErrorLang.genericexception().ToString() } }, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}