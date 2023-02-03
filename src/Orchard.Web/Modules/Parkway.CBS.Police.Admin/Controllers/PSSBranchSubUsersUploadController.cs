using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;


namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSBranchSubUsersUploadController : Controller
    {
        private readonly IPSSBranchSubUsersUploadHandler _handler;
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public PSSBranchSubUsersUploadController(IOrchardServices orchardServices, IPSSBranchSubUsersUploadHandler handler, IShapeFactory shapeFactory)
        {
            _handler = handler;
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }

        [HttpGet]
        public ActionResult GetRegularizationProfile()
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewRegularizationProfile);
                return View();
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        [HttpPost]
        public ActionResult GetRegularizationProfile(string payerId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewRegularizationProfile);
                if (string.IsNullOrEmpty(payerId?.Trim())) 
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Profile ID not specified"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.ViewBranchDetails, new { payerId = payerId });
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        public ActionResult ViewBranchDetails(string payerId, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadBranchSubUsers);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSBranchDetailsVM branchDetailsVM = _handler.GetPSSBranchDetailsVM(payerId, new TaxEntityProfileLocationReportSearchParams {  Skip = skip, Take = take });
                if(branchDetailsVM == null)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString($"Entity with Profile ID {payerId} was not found"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                var pageShape = Shape.Pager(pager).TotalItemCount(branchDetailsVM.TotalRecordCount);

                branchDetailsVM.Pager = pageShape;
                return View(branchDetailsVM);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        [HttpPost]
        public ActionResult FileUploadValidation(string payerId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadBranchSubUsers);
                if (string.IsNullOrEmpty(payerId?.Trim())) 
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Profile ID not specified"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                if (HttpContext.Request.Files.Get("branchSubUsersFile") == null || HttpContext.Request.Files.Get("branchSubUsersFile").ContentLength == 0)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Branch Sub Users file not uploaded"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                if (Path.GetExtension(HttpContext.Request.Files.Get("branchSubUsersFile").FileName) != ".xls" && Path.GetExtension(HttpContext.Request.Files.Get("branchSubUsersFile").FileName) != ".xlsx")
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Uploaded Branch Sub Users file format not supported. Only .xls and .xlsx are supported."));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                List<ErrorModel> errors = new List<ErrorModel> { };
                if(_handler.ValidateBranchSubUserFile(HttpContext.Request.Files.Get("branchSubUsersFile"), ref errors))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                string batchToken = _handler.ProcessBranchSubUsersFileUpload(HttpContext.Request.Files.Get("branchSubUsersFile"), payerId);
                return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.FileUploadValidationResult, new { batchToken = batchToken});
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        
        public ActionResult FileUploadValidationResult(string batchToken, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadBranchSubUsers);
                if (string.IsNullOrEmpty(batchToken?.Trim())) 
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Branch Sub Users file batch token not specified"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSBranchSubUsersUploadValidationResultVM vm = _handler.GetUploadValidationResult(batchToken, new Core.VM.PSSBranchSubUsersUploadBatchItemsSearchParams
                {
                    Skip = skip,
                    Take = take,
                    PageData = true
                });

                if(vm.BatchDetails.Status == (int)Core.Models.Enums.PSSBranchSubUserUploadStatus.Completed)
                {
                    _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString("This file upload has already been processed."));
                }

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.PSSBranchSubUsersUploadBatchItemsReport.NumberOfRecords);

                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        [HttpPost]
        public ActionResult SaveBranchSubUsers(string batchToken)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanSaveUploadedBranchSubUsers);
                if (string.IsNullOrEmpty(batchToken?.Trim()))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Branch Sub Users file batch token not specified"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                string payerId = _handler.SaveBranchSubUsers(batchToken);
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString("Branch Sub Users Created Successfully."));
                return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.ViewBranchDetails, new { payerId = payerId });
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }
    }
}