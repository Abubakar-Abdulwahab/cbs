using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAuthorized]
    [CheckIsAdministratorFilter(true)]
    public class PSSSubUserController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSSubUserHandler _handler;

        public PSSSubUserController(IHandler compHandler, IAuthenticationService authenticationService, IPSSSubUserHandler handler) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        public ActionResult SubUsers(PSSSubUserVM userInput)
        {
            try
            {
                FlashObj flashObj = null;
                try
                {
                    if (TempData.ContainsKey("Success"))
                    {
                        flashObj = new FlashObj { MessageTitle = "Success", Message = TempData["Success"].ToString(), FlashType = CBS.Core.Models.Enums.FlashType.Success };
                        TempData.Remove("Success");
                    }
                }
                catch (Exception exception) { Logger.Error(exception, "Error getting success value from temp data " + exception.Message); }
                TempData = null;
                UserDetailsModel user = GetLoggedInUserDetails();
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

                //get the date up until the final sec
                //endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = 10;
                int skip = 0;

                CBSUserTaxEntityProfileLocationReportSearchParams searchParams = new CBSUserTaxEntityProfileLocationReportSearchParams
                {
                    TaxEntityId = user.TaxPayerProfileVM.Id,
                    Take = take,
                    Skip = skip,
                    //EndDate = endDate,
                    //StartDate = startDate,
                    SubUserName = userInput.FilteredName,
                    Branch = userInput.FilteredBranch,
                };

                PSSSubUserVM subUserRecords = _handler.GetSubUsers(searchParams);
                PSSSubUserVM vm = _handler.GetCreateSubUserVM(user.TaxPayerProfileVM.Id);
                subUserRecords.Branches = vm.Branches;
                subUserRecords.SubUserInfo = vm.SubUserInfo;
                subUserRecords.HeaderObj = HeaderFiller(user);
                subUserRecords.FlashObj = flashObj;
                subUserRecords.DataSize = Util.Pages(take, subUserRecords.TotalRecordCount);
                return View(subUserRecords);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData["Error"] = ErrorLang.genericexception().Text;
            }
            return RedirectToRoute(RouteName.SelectService.ShowSelectService);
        }


        [HttpGet]
        public ActionResult CreateSubUser()
        {
            return RedirectToRoute(RouteName.PSSSubUser.SubUsers);
        }


        [HttpPost]
        public ActionResult CreateSubUser(PSSSubUserVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            UserDetailsModel user = null;
            try
            {
                TempData = null;
                user = GetLoggedInUserDetails();
                _handler.CreateSubUser(userInput, user.TaxPayerProfileVM.Id, user.CategoryVM.Id, ref errors);
                TempData["Success"] = "Sub User created successfully";
                return RedirectToRoute(RouteName.PSSSubUser.SubUsers);
            }
            catch (DirtyFormDataException)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }

                PSSSubUserVM vm = _handler.GetCreateSubUserVM(user.TaxPayerProfileVM.Id);
                int take = 10;
                var subUsers = _handler.GetSubUsers(new CBSUserTaxEntityProfileLocationReportSearchParams
                {
                    TaxEntityId = user.TaxPayerProfileVM.Id,
                    Take = take,
                    Skip = 0,
                    //EndDate = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1),
                    //StartDate = DateTime.Now.AddMonths(-3),
                });
                //userInput.DateFilter = subUsers.DateFilter;
                userInput.SubUsers = subUsers.SubUsers;
                userInput.TotalRecordCount = subUsers.TotalRecordCount;
                userInput.DataSize = Util.Pages(take, subUsers.TotalRecordCount);
                userInput.Token = subUsers.Token;
                userInput.Branches = vm.Branches;
                userInput.ShowCreateSubUserModal = true;
                userInput.HeaderObj = HeaderFiller(user);
                return View(nameof(RouteName.PSSSubUser.SubUsers), userInput);
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