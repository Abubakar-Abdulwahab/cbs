using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;
using System.Globalization;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Client.Web.ViewModels;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class TCCRequestHistoryController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICommonBaseHandler _baseHandler;
        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ITCCRequestHistoryHandler _tccRequestHistoryHandler;

        public TCCRequestHistoryController(ICommonBaseHandler baseHandler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ITCCRequestHistoryHandler tccRequestHistoryHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, baseHandler)
        {
            _baseHandler = baseHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
            _tccRequestHistoryHandler = tccRequestHistoryHandler;
        }


        /// <summary>
        /// Fetches etcc requests
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="dateFilter"></param>
        /// <returns></returns>
        public ActionResult RequestHistory(TCCRequestHistoryVM userInput, string dateFilter)
        {
            try
            {
                //TempData = null;
                var user = GetLoggedInUserDetails();
                string message = string.Empty;

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!String.IsNullOrEmpty(userInput.DateFilter))
                {
                    var dateFilterSplit = userInput.DateFilter.Split(new[] { '-' }, 2);
                    if (dateFilterSplit.Length == 2)
                    {
                        startDate = DateTime.ParseExact(dateFilterSplit[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(dateFilterSplit[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                }

                try
                {
                    if (TempData.ContainsKey("Message"))
                    {
                        message = TempData["Message"].ToString();
                        TempData.Remove("Message");
                    }
                }
                catch (Exception) { }

                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = 10;
                int skip = 0;

                TCCReportSearchParams searchParams = new TCCReportSearchParams
                {
                    TaxEntityId = user.Entity.Id,
                    Take = take,
                    Skip = skip,
                    EndDate = endDate,
                    StartDate = startDate,
                    ApplicationNumber = userInput.ApplicationNumber,
                    ApplicantName = userInput.ApplicantName,
                    PayerId = userInput.TIN,
                    SelectedStatus = userInput.Status,
                };

                TCCRequestHistoryVM model = _tccRequestHistoryHandler.GetRequests(searchParams);
                model.HeaderObj = HeaderFiller(user);

                var dataSize = model.TotalRequestRecord;
                double pageSize = ((double)dataSize / (double)take);
                int pages = 0;

                if (pageSize < 1 && dataSize >= 1) { pages = 1; }
                else { pages = (int)Math.Ceiling(pageSize); }

                model.DataSize = pages;
                model.Message = message;

                return View(model);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return RedirectToRouteX(Module.Web.RouteName.Collection.GenerateInvoice);
            }
        }

    }
}