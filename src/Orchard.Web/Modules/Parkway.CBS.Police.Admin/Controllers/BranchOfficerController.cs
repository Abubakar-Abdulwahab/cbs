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
using Parkway.CBS.Police.Admin.RouteName;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Exceptions;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class BranchOfficerController : Controller
    {
        private readonly IBranchOfficerHandler _branchOfficerHandler;
        private readonly INotifier _notifier;
        private readonly IOrchardServices _orchardServices;

        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public BranchOfficerController(IOrchardServices orchardServices, IBranchOfficerHandler branchOfficerHandler, IShapeFactory shapeFactory)
        {
            _branchOfficerHandler = branchOfficerHandler;
            Logger = NullLogger.Instance;
            _notifier = orchardServices.Notifier;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
        }

        [HttpGet]
        public ActionResult GenerateEscortRequest(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id?.Trim()) || !long.TryParse(id, out long batchId))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid file upload batch id"));
                    return Redirect("~/Admin");
                }
                _branchOfficerHandler.CheckForPermission(Permissions.CanViewBranchProfileDetail);
                return View(_branchOfficerHandler.GetGenerateEscortRequestVM(batchId));
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (PSSRequestAlreadyExistsException)
            {
                Logger.Error($"A request has already been generated for the branch");
                _notifier.Add(NotifyType.Information, PoliceErrorLang.ToLocalizeString("A request has already been generated for the branch"));
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
        public ActionResult GenerateEscortRequest(PSSBranchGenerateEscortRequestVM userInputModel)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            try
            {
                _branchOfficerHandler.CheckForPermission(Permissions.CanViewBranchProfileDetail);

                var response = _branchOfficerHandler.GenerateEscortRequest(ref errors, userInputModel);
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString($"A request with invoice number {response.InvoiceNumber} has been generated successfully."));
                return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = userInputModel.BranchDetails.Id });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                foreach(var error in errors)
                {
                    ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }
                _branchOfficerHandler.PopulateGenerateEscortRequestVMForPostback(userInputModel);

                return View(userInputModel);
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, exception.Message);
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

        [HttpGet]
        public ActionResult BranchProfileDetail(string id, PagerParameters pagerParameters)
        {
            try
            {
                _branchOfficerHandler.CheckForPermission(Permissions.CanViewBranchProfileDetail);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                if (!int.TryParse(id, out int profileLocationId))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid profile id"));
                    return Redirect("~/Admin");
                }

                var branchDetailsVM = _branchOfficerHandler.GetBranchProfileDetailVM(new PSSBranchOfficersUploadBatchStagingReportSearchParams { Skip = skip, Take = take, ProfileLocationId = profileLocationId });

                if (branchDetailsVM == null)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString($"Entity with Profile ID {id} was not found"));
                    return RedirectToRoute(PSSBranchSubUsersUpload.GetRegularizationProfile);
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
        public ActionResult UploadBranchOfficer(string profileId)
        {
            try
            {
                _branchOfficerHandler.CheckForPermission(Permissions.CanViewBranchProfileDetail);

                if (HttpContext.Request.Files.Get("branchOfficerFile") == null || HttpContext.Request.Files.Get("branchOfficerFile").ContentLength == 0)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Branch Officers file not uploaded"));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = profileId });
                }

                if (Path.GetExtension(HttpContext.Request.Files.Get("branchOfficerFile").FileName) != ".xls" && Path.GetExtension(HttpContext.Request.Files.Get("branchOfficerFile").FileName) != ".xlsx")
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Uploaded Branch Officer file format not supported. Only .xls and .xlsx are supported."));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = profileId });
                }
                List<ErrorModel> errors = new List<ErrorModel> { };

                if (_branchOfficerHandler.ValidateBranchOfficerFile(HttpContext.Request.Files.Get("branchOfficerFile"), ref errors))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = profileId });
                }

                if (!int.TryParse(profileId, out int profileIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid profile id"));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = profileId });
                }

                string batchToken = _branchOfficerHandler.ProcessBranchOfficerUpload(HttpContext.Request.Files.Get("branchOfficerFile"), profileIdResult);
                return RedirectToRoute(BranchOfficer.OfficerValidationResult, new { profileId, batchToken });

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

        [HttpGet]
        public ActionResult OfficerValidationResult(string profileId, string batchToken, PagerParameters pagerParameters)
        {
            try
            {
                if (!int.TryParse(profileId, out int profileIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid profile id"));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = profileId });
                }

                if (string.IsNullOrEmpty(batchToken?.Trim()))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Branch officer file batch token not specified"));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { profileId });
                }

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                var model = _branchOfficerHandler.GetOfficerValidationResultVM(new PSSBranchOfficersUploadBatchItemsStagingReportSearchParams
                {
                    Skip = skip,
                    Take = take,
                    ProfileLocationId = profileIdResult,
                    BatchId = _branchOfficerHandler.GetFileProcessModel(batchToken).Id
                });
                var pageShape = Shape.Pager(pager).TotalItemCount(model.PSSBranchOfficerUploadBatchItemsReport.NumberOfRecords);

                model.BatchToken = batchToken;
                model.Pager = pageShape;
                return View(model);
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
        public ActionResult DownloadOfficerRequest(string id)
        {
            try
            {
                return RedirectToRoute(BranchOfficer.OfficerValidationResult, new { id = "Batch-ref" });
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

        public ActionResult EscortRequestDetail(string batchId, string profileId, PagerParameters pagerParameters)
        {
            try
            {
                _branchOfficerHandler.CheckForPermission(Permissions.CanViewEscortRequestDetail);
                if (!int.TryParse(batchId, out int batchIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid profile id"));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = batchId });
                }

                if (!int.TryParse(profileId, out int profileIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid profile id"));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = batchId });
                }

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                var model = _branchOfficerHandler.GetEscortRequestDetailVM(new PSSBranchOfficersUploadBatchItemsStagingReportSearchParams
                {
                    Skip = skip,
                    Take = take,
                    ProfileLocationId = profileIdResult,
                    BatchId = batchIdResult
                });
                var pageShape = Shape.Pager(pager).TotalItemCount(model.PSSBranchOfficerUploadBatchItemsReport.NumberOfRecords);

                model.Pager = pageShape;
                return View(model);
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