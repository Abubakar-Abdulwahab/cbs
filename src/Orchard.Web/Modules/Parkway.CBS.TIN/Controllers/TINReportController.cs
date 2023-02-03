using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.TIN.Controllers.Handlers.Contracts;
using Parkway.CBS.TIN.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.TIN.Controllers
{
    [Admin]
    public class TINReportController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ITINHandler _handler;
        private readonly IOrchardServices _orchardService;
        private readonly ISiteService _siteService;
        private Localizer T { get; }
        dynamic Shape { get; set; }

        public TINReportController(ITINHandler handler, IAuthenticationService authenticationService,
            IOrchardServices orchardService, IShapeFactory shapeFactory, ISiteService siteService)
        {
            _handler = handler;
            _authenticationService = authenticationService;
            _orchardService = orchardService;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
            _siteService = siteService;
        }

        // GET: TINReport
        public ActionResult Index(PagerParameters pagerParameters, TINSearchParameters search)
        {
            //get and validate user
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser == null)
                return new HttpUnauthorizedResult();

            if (!_orchardService.Authorizer.Authorize(Permissions.ViewTINReport, T("Not authorized to Use This Feature")))
                return new HttpUnauthorizedResult();

            var applicantRecords = new List<TINApplicantReportModel>();

            //data to populate on first load
            if (string.IsNullOrWhiteSpace(search.PhoneNumber) &&
                string.IsNullOrWhiteSpace(search.FirstName) &&
                string.IsNullOrWhiteSpace(search.LastName) &&
                search.StartDate == null &&
                search.EndDate == null)
            {
                applicantRecords = _handler.GetTINApplicantReport().ToList();
            }

            // perform a search using the search parameters
            else
            {
                applicantRecords = _handler.TINApplicantReportSearch(search).ToList();
            }

            //set page size to the count of objects returned
            pagerParameters.PageSize = pagerParameters.PageSize == 0 ? (int)Math.Ceiling((double)applicantRecords.Count()) : pagerParameters.PageSize;

            //apply pagination
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            // Construct a Pager shape
            var pagerShape = Shape.Pager(pager).TotalItemCount(applicantRecords.Count());

            // Apply paging
            var applRecord = applicantRecords.Skip(pager.GetStartIndex()).Take(pager.PageSize);

            //populate the view model
            var applReport = new TINApplicantReportModel(applRecord, pagerShape);

            return View(applReport);
        }


    }
}