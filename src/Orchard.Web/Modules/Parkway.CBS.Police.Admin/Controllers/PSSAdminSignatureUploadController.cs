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
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSAdminSignatureUploadController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSAdminSignatureUploadHandler _handler;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public PSSAdminSignatureUploadController(IOrchardServices orchardServices, IPSSAdminSignatureUploadHandler handler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        [HttpGet]
        public ActionResult UploadSignature()
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanUploadSignatures);
                return View(new PSSAdminSignatureUploadVM { });
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
        public ActionResult UploadSignature(PSSAdminSignatureUploadVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                _handler.CheckForPermission(Permissions.CanUploadSignatures);
                if (HttpContext.Request.Files.Get("signatureFile") == null || HttpContext.Request.Files.Get("signatureFile").ContentLength == 0)
                {
                    errors.Add(new ErrorModel { FieldName = "SignatureFile", ErrorMessage = "No signature file specified." });
                    throw new DirtyFormDataException();
                }

                if (!_handler.ValidateSignatureFile(HttpContext.Request.Files.Get("signatureFile"), ref errors)) 
                {
                    _handler.SaveSignature(HttpContext.Request.Files.Get("signatureFile"), ref errors);
                    _notifier.Add(NotifyType.Information, PoliceLang.pssadminsignaturesavedsuccessfully);
                    return RedirectToRoute(RouteName.PSSAdminSignatureUpload.SignaturesList);
                }
                throw new DirtyFormDataException();
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errors.First().ErrorMessage));
                return View(userInput);
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


        public ActionResult SignaturesList(PSSAdminSignaturesListVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadedSignatures);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInput.Start) && !string.IsNullOrEmpty(userInput.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInput.Start, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSAdminSignaturesListVM vm = _handler.GetSignaturesListVM(_orchardServices.WorkContext.CurrentUser.Id, take, skip, startDate, endDate);

                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalSignaturesUploaded);

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


        public ActionResult EnableAdminSignature(int adminSignatureId)
        {
            string errorMessage = string.Empty;
            try
            {
                _handler.CheckForPermission(Permissions.CanViewUploadedSignatures);
                if (adminSignatureId == 0) { errorMessage = "signature does not exist"; throw new DirtyFormDataException(); }
                _handler.EnableAdminSignature(_orchardServices.WorkContext.CurrentUser.Id, adminSignatureId, ref errorMessage);
                if (!string.IsNullOrEmpty(errorMessage)) { throw new DirtyFormDataException(); }
                _notifier.Add(NotifyType.Information, PoliceLang.ToLocalizeString("Signature enabled successfully"));
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString(errorMessage));
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
            return RedirectToAction(nameof(RouteName.PSSAdminSignatureUpload.SignaturesList));
        }
    }
}