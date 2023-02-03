using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Parkway.CBS.Police.Client.Middleware;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAuthorized]
    [CheckIsAdministratorFilter(true)]
    public class PSSBranchesController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSBranchesHandler _handler;

        public PSSBranchesController(IHandler compHandler, IAuthenticationService authenticationService, IPSSBranchesHandler handler) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        public ActionResult Branches(PSSBranchVM userInput)
        {
            try
            {
                TempData = null;
                var user = GetLoggedInUserDetails();
                //DateTime startDate = DateTime.Now.AddMonths(-3);
                //DateTime endDate = DateTime.Now;

                //if (!string.IsNullOrEmpty(userInput.DateFilter))
                //{
                //    var dateFilterSplit = userInput.DateFilter.Split(new[] { '-' }, 2);
                //    if (dateFilterSplit.Length == 2)
                //    {
                //        startDate = DateTime.ParseExact(dateFilterSplit[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //        endDate = DateTime.ParseExact(dateFilterSplit[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    }
                //}

                ////get the date up until the final sec
                //endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = 10;
                int skip = 0;

                TaxEntityProfileLocationReportSearchParams searchParams = new TaxEntityProfileLocationReportSearchParams
                {
                    TaxEntityId = user.TaxPayerProfileVM.Id,
                    Take = take,
                    Skip = skip,
                    //EndDate = endDate,
                    //StartDate = startDate,
                    Name = userInput.FilteredName,
                    Address = userInput.FilteredAddress,
                    State = userInput.FilteredState,
                    LGA = userInput.FilteredLGA,
                };

                var branchRecords = _handler.GetBranches(searchParams);
                var vm = _handler.GetCreateBranchVM();
                branchRecords.BranchInfo = vm.BranchInfo;
                branchRecords.StateLGAs = vm.StateLGAs;
                branchRecords.FilterListLGAs = (userInput.FilteredState > 0) ? vm.StateLGAs.Where(x => x.Id == userInput.FilteredState).SingleOrDefault().LGAs.ToList() : null;
                branchRecords.HeaderObj = HeaderFiller(user);
                branchRecords.DataSize = Util.Pages(take, branchRecords.TotalRecordCount);
                return View(branchRecords);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData["Error"] = ErrorLang.genericexception().Text;
            }
            return RedirectToRoute(RouteName.SelectService.ShowSelectService);
        }


        [HttpGet]
        public ActionResult CreateBranch()
        {
            return RedirectToAction("Branches");
        }


        [HttpPost]
        public ActionResult CreateBranch(PSSBranchVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            UserDetailsModel user = null;
            try
            {
                TempData = null;
                user = GetLoggedInUserDetails();
                userInput.BranchInfo.TaxEntityId = user.TaxPayerProfileVM.Id;
                _handler.CreateNewBranch(userInput.BranchInfo, ref errors);
                return RedirectToAction("Branches");
            }
            catch (DirtyFormDataException)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }
                var vm = _handler.GetCreateBranchVM();
                int take = 10;
                var branches = _handler.GetBranches(new TaxEntityProfileLocationReportSearchParams
                {
                    TaxEntityId = user.TaxPayerProfileVM.Id,
                    Take = take,
                    Skip = 0,
                    //EndDate = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1),
                    //StartDate = DateTime.Now.AddMonths(-3),
                });
                //userInput.DateFilter = branches.DateFilter;
                userInput.Branches = branches.Branches;
                userInput.TotalRecordCount = branches.TotalRecordCount;
                userInput.DataSize = Util.Pages(take, branches.TotalRecordCount);
                userInput.Token = branches.Token;
                userInput.StateLGAs = vm.StateLGAs;
                userInput.HeaderObj = HeaderFiller(user);
                userInput.ListLGAs = (userInput.BranchInfo.State > 0) ? vm.StateLGAs.Where(x => x.Id == userInput.BranchInfo.State).SingleOrDefault().LGAs.ToList() : null;
                userInput.ShowCreateBranchModal = true;

                return View("Branches", userInput);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData["Error"] = ErrorLang.genericexception().Text;
            }
            return RedirectToRoute(RouteName.SelectService.ShowSelectService);
        }
    }
}