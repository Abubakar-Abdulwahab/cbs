using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAuthorized]
    [HasSubUsersFilter(true)]
    public class SubUsersRequestListController : BaseController
    {
        private readonly IRequestListHandler _handler;

        public new ILogger Logger { get; set; }

        readonly int oneMonth = 1;
        readonly int maximumListMonths = 6;

        public SubUsersRequestListController(IHandler compHandler, IAuthenticationService authenticationService, IRequestListHandler handler)
            : base(authenticationService, compHandler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        public ActionResult SubUsersRequestList()
        {
            DateTime startDate = DateTime.Now.Date.AddMonths(-3);
            DateTime endDate = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);
            string SelectedtServiceId = "0";
            int take = 10;
            int skip = 0;

            try
            {
                UserDetailsModel userDetails = GetLoggedInUserDetails();

                RequestsReportSearchParams searchParams = new RequestsReportSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = take,
                    Skip = skip,
                    TaxEntityId = userDetails.TaxPayerProfileVM.Id,
                    IsBranchAdmin = userDetails.CBSUserVM.IsAdministrator,
                    CBSUserId = userDetails.CBSUserVM.Id,
                    RequestOptions = new RequestOptions { RequestStatus = PSSRequestStatus.None },
                    SelectedServiceId = SelectedtServiceId,
                    IntValueSelectedServiceId = 0
                };

                RequestListVM requests = _handler.GetRequestBranchListVM(searchParams);

                requests.HeaderObj = HeaderFiller(userDetails);
                requests.requestStatus = (int)PSSRequestStatus.None;
                requests.startDateString = searchParams.StartDate.ToString("dd/MM/yyyy");
                requests.endDateString = searchParams.EndDate.ToString("dd/MM/yyyy");

                if (TempData.ContainsKey("Error"))
                {
                    requests.HasError = true;
                    requests.ErrorMessage = TempData["Error"].ToString();
                    TempData.Remove("Error");
                }

                return View(requests);
            }
            catch (Exception excep)
            {
                Logger.Error(excep, excep.Message);
                TempData.Add("Error", ErrorLang.genericexception());
                return RedirectToRoute(RouteName.SelectService.ShowSelectService);
            }

        }

        [HttpPost]
        public ActionResult SubUsersRequestList(string from, string end, int status, int? state, int? lga, int? branch)
        {
            DateTime startDate = DateTime.Now.Date.AddMonths(-3);
            DateTime endDate = DateTime.Now;
            string SelectedtServiceId = "0";
            int take = 10;
            int skip = 0;

            try
            {

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(end))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth; //Added a month to balance the substraction e.g Jannuary to April, 4-1 will be 3 months instead of 4
                        if (selectedMonth > maximumListMonths)
                        {
                            Logger.Debug("Max number of requests exceeded");
                            TempData.Add("Error", PoliceErrorLang.Request_List_Months_Duration_Exceeded(maximumListMonths));
                            return RedirectToRoute(RouteName.SubUsersRequestList.ShowSubUsersRequestList);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }


                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
                UserDetailsModel userDetails = GetLoggedInUserDetails();

                RequestsReportSearchParams searchParams = new RequestsReportSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = take,
                    Skip = skip,
                    State = state ?? 0,
                    LGA = lga ?? 0,
                    Branch = branch ?? 0,
                    IsBranchAdmin = userDetails.CBSUserVM.IsAdministrator,
                    TaxEntityId = userDetails.TaxPayerProfileVM.Id,
                    CBSUserId = userDetails.CBSUserVM.Id,
                    RequestOptions = new RequestOptions { RequestStatus = (status == 0) ? PSSRequestStatus.None : (PSSRequestStatus)status },
                    SelectedServiceId = SelectedtServiceId,
                    IntValueSelectedServiceId = 0
                };

                RequestListVM filteredRequests = _handler.GetRequestBranchListVM(searchParams);

                filteredRequests.HeaderObj = HeaderFiller(userDetails);
                filteredRequests.SelectedState = searchParams.State;
                filteredRequests.SelectedStateLGA = searchParams.LGA;
                filteredRequests.BranchName = searchParams.BranchName;
                filteredRequests.startDateString = searchParams.StartDate.ToString("dd/MM/yyyy");
                filteredRequests.endDateString = searchParams.EndDate.ToString("dd/MM/yyyy");
                filteredRequests.requestStatus = status;
                filteredRequests.SelectedBranch = searchParams.Branch;
                return View(filteredRequests);
            }
            catch (Exception excep)
            {
                Logger.Error(excep, excep.Message);
                TempData.Add("Error", ErrorLang.genericexception());
                return RedirectToRoute(RouteName.SelectService.ShowSelectService);
            }
        }


        public virtual JsonResult SubUsersRequestListMoveRight(int status, long operatorId, int page, string startDateFilter, string endDateFilter, int? state, int? lga, int branch)
        {
            Logger.Information(string.Format("getting page data for operator Id - {0} page - {1}", operatorId, page));

            try
            {
                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;
                PSSRequestStatus? requestStatus = (PSSRequestStatus)status;
                string SelectedtServiceId = "0";
                int take = 10;
                int skip = (page == 1) ? 0 : (page * take) - take;

                if (!string.IsNullOrEmpty(startDateFilter) && !string.IsNullOrEmpty(endDateFilter))
                {
                    startDate = DateTime.ParseExact(startDateFilter, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(endDateFilter, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth; //Added a month to balance the substraction e.g Jannuary to April, 4-1 will be 3 months instead of 4
                    if (selectedMonth > maximumListMonths)
                    {
                        Logger.Debug("Max number of requests exceeded");
                        return Json(new APIResponse { Error = true, ResponseObject = PoliceErrorLang.Request_List_Months_Duration_Exceeded(maximumListMonths) }, JsonRequestBehavior.AllowGet);
                    }
                }

                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                UserDetailsModel userDetails = GetLoggedInUserDetails();

                RequestsReportSearchParams searchParams = new RequestsReportSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = take,
                    Skip = skip,
                    State = state ?? 0,
                    IsBranchAdmin = userDetails.CBSUserVM.IsAdministrator,
                    CBSUserId = userDetails.CBSUserVM.Id,
                    LGA = lga ?? 0,
                    Branch = branch,
                    TaxEntityId = userDetails.TaxPayerProfileVM.Id,
                    RequestOptions = new RequestOptions { RequestStatus = requestStatus ?? PSSRequestStatus.None },
                    SelectedServiceId = SelectedtServiceId,
                    IntValueSelectedServiceId = 0
                };

                RequestListVM requestList = _handler.GetRequestBranchListVM(searchParams);

                return Json(new APIResponse { ResponseObject = requestList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception excep)
            {
                Logger.Debug(excep, excep.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}