using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.UI.Navigation;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.HelperModels;
using System.IO;
using Parkway.CBS.Police.Admin.RouteName;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class GenerateRequestWithoutOfficersController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IGenerateRequestWithoutOfficersHandler _handler;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public GenerateRequestWithoutOfficersController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IGenerateRequestWithoutOfficersHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        public ActionResult GenerateRequestForDefaultBranchWithoutOfficersUpload(string profileId, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadBranchSubUsers);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                GenerateRequestForDefaultBranchWithoutOfficersUploadVM vm = _handler.GetGenerateRequestForDefaultBranchWithoutOfficersUploadVM(profileId, new GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams { Skip = skip, Take = take, PageData = true });
                if (vm == null)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString($"Entity with Profile ID {profileId} was not found"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalRecordCount);

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


        public ActionResult GenerateRequestForDefaultBranchWithoutOfficersFileUploadValidation(string profileId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadBranchSubUsers);
                if (string.IsNullOrEmpty(profileId?.Trim()))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Profile ID not specified"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                if (HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile") == null || HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile").ContentLength == 0)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Generate request without officers file not uploaded"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                if (Path.GetExtension(HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile").FileName) != ".xls" && Path.GetExtension(HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile").FileName) != ".xlsx")
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Uploaded file format not supported. Only .xls and .xlsx are supported."));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                List<ErrorModel> errors = new List<ErrorModel> { };
                if (_handler.ValidateGenerateRequestWithoutOfficersUploadFile(HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile"), ref errors))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                string batchToken = _handler.ProcessGenerateRequestWithoutOfficersUploadFileForEntity(HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile"), profileId);
                return RedirectToRoute(RouteName.GenerateRequestWithoutOfficers.GenerateRequestForDefaultBranchWithoutOfficersFileUploadValidationResult, new { batchToken = batchToken });
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


        public ActionResult GenerateRequestForDefaultBranchWithoutOfficersFileUploadValidationResult(string batchToken, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadBranchSubUsers);
                if (string.IsNullOrEmpty(batchToken?.Trim()))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("file batch token not specified"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                GenerateRequestWithoutOfficersFileUploadValidationResultVM vm = _handler.GetUploadValidationResult(batchToken, new GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams
                {
                    Skip = skip,
                    Take = take,
                    PageData = true
                });

                if (vm.BatchDetails.Status == (int)Core.Models.Enums.GenerateRequestWithoutOfficersUploadStatus.Completed)
                {
                    _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString("This file upload has already been processed."));
                }

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.BatchItemsReport.NumberOfRecords);

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


        public ActionResult GenerateRequestForBranchWithoutOfficersUpload(int branchId, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewBranchProfileDetail);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                GenerateRequestForBranchWithoutOfficersUploadVM vm = _handler.GetGenerateRequestForBranchWithoutOfficersUploadVM(new GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams { Skip = skip, Take = take, TaxEntityProfileLocationId = branchId, PageData = true });

                if (vm == null)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString($"Branch not found"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalRecordCount);

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


        public ActionResult GenerateRequestForBranchWithoutOfficersFileUploadValidation(int branchId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadBranchSubUsers);

                if (HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile") == null || HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile").ContentLength == 0)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Generate request without officers file not uploaded"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                if (Path.GetExtension(HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile").FileName) != ".xls" && Path.GetExtension(HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile").FileName) != ".xlsx")
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Uploaded file format not supported. Only .xls and .xlsx are supported."));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }

                List<ErrorModel> errors = new List<ErrorModel> { };
                if (_handler.ValidateGenerateRequestWithoutOfficersUploadFile(HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile"), ref errors))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                string batchToken = _handler.ProcessGenerateRequestWithoutOfficersUploadFileForBranch(HttpContext.Request.Files.Get("generateRequestWithoutOfficersFile"), branchId);
                return RedirectToRoute(RouteName.GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersFileUploadValidationResult, new { batchToken = batchToken });
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


        public ActionResult GenerateRequestForBranchWithoutOfficersFileUploadValidationResult(string batchToken, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadBranchSubUsers);
                if (string.IsNullOrEmpty(batchToken?.Trim()))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("file batch token not specified"));
                    return RedirectToRoute(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile);
                }
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                GenerateRequestWithoutOfficersFileUploadValidationResultVM vm = _handler.GetUploadValidationResult(batchToken, new GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams
                {
                    Skip = skip,
                    Take = take,
                    PageData = true
                });

                if (vm.BatchDetails.Status == (int)Core.Models.Enums.GenerateRequestWithoutOfficersUploadStatus.Completed)
                {
                    _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString("This file upload has already been processed."));
                }

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.BatchItemsReport.NumberOfRecords);

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

        public ActionResult GenerateBranchRequestWithoutOfficersDetail(string batchId, string profileId, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewEscortRequestDetail);
                if (!int.TryParse(batchId, out int batchIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid batch id"));
                    return RedirectToRoute(GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersUpload, new { id = profileId });
                }

                if (!int.TryParse(profileId, out int profileIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid profile id"));
                    return RedirectToRoute(GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersUpload, new { id = profileId });
                }

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                var model = _handler.GetGenerateRequestDetailVM(new GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams
                {
                    Skip = skip,
                    Take = take,
                    ProfileId = profileIdResult,
                    GenerateRequestWithoutOfficersUploadBatchStagingId = batchIdResult,
                    PageData = true
                });
                var pageShape = Shape.Pager(pager).TotalItemCount(model.GenerateRequestWithoutOfficersUploadBatchItemsReport.NumberOfRecords);

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

        public ActionResult GenerateDefaultRequestWithoutOfficersDetail(string batchId, string taxEntityId, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewEscortRequestDetail);
                if (!int.TryParse(batchId, out int batchIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid batch id"));
                    return RedirectToRoute(GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersUpload, new { id = taxEntityId });
                }

                if (!int.TryParse(taxEntityId, out int taxEntityIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid profile id"));
                    return RedirectToRoute(GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersUpload, new { id = taxEntityId });
                }

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                var model = _handler.GetGenerateRequestDetailVM(new GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams
                {
                    Skip = skip,
                    Take = take,
                    ProfileId = taxEntityIdResult,
                    GenerateRequestWithoutOfficersUploadBatchStagingId = batchIdResult,
                    PageData = true
                }, true);
                var pageShape = Shape.Pager(pager).TotalItemCount(model.GenerateRequestWithoutOfficersUploadBatchItemsReport.NumberOfRecords);

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