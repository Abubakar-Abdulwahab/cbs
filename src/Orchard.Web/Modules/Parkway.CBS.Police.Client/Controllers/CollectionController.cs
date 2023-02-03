using System;
using System.Web.Mvc;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class CollectionController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IHandler _handler;


        public CollectionController(IHandler handler, IAuthenticationService authenticationService)
            : base(authenticationService, handler)
        {
            _handler = handler;
            _authenticationService = authenticationService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual HeaderObj GetIndexVM()
        {
            var userDetails = GetLoggedInUserDetails();
            HeaderObj obj = HeaderFiller(userDetails);
            return obj;
        }


        /// <summary>
        /// home page for pss
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Index()
        {
            try
            {
                return View(GetIndexVM());
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}