using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.OSGOF.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.OSGOF.Admin.Controllers
{
    [Admin]
    public class CellSitesController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ICellSitesHandler _handler;
        public IOperatorHandler _operatorHandler;
        dynamic Shape { get; set; }
        private Localizer T { get; }


        public CellSitesController(IOrchardServices orchardServices, ICellSitesHandler handler, IShapeFactory shapeFactory, IOperatorHandler operatorHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
            _operatorHandler = operatorHandler;
        }


        /// <summary>
        /// ROuteName: CellSitesFileUploadReport
        /// Show the result of the file upload
        /// </summary>
        [HttpGet]
        public ActionResult CellSitesFileUploadReport(string scheduleRef, PagerParameters pagerParameters)
        {
            try
            {
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;
                var viewModel = _handler.GetScheduleStagingData(scheduleRef, take, skip);
                if (viewModel.Error)
                {
                    _notifier.Add(NotifyType.Error, T(viewModel.ErrorMessage));
                }
                else
                {
                    var pageShape = Shape.Pager(pager).TotalItemCount(viewModel.TotalNumberOfRecords);
                    viewModel.Pager = pageShape;
                }
                return View(viewModel);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute("MDA.Dashboard");
            }
        }

        [HttpPost]
        public ActionResult CellSitesFileUploadReport(string scheduleRef)
        {
            try
            {
                CellSitesStagingReportVM viewModel = _handler.CompleteCellSiteProcessing(scheduleRef);

                if (viewModel.Error) { return View(viewModel); }
                TempData = null;
                TempData.Add("Message", viewModel.Message);
                return RedirectToRoute("List");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute("MDA.Dashboard");
            }
        }

        [HttpGet]
        public ActionResult List(CellSitesVM model, PagerParameters pagerParameters)
        {
            try
            {
                try
                {
                    if (TempData.ContainsKey("Message"))
                    { _notifier.Add(NotifyType.Information, T(TempData["Message"].ToString())); }
                }
                catch (Exception exception)
                { Logger.Error(exception, exception.Message); }
                TempData = null;

                //_handler.CheckForPermission(Permissions.List);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;
                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                //Zero is passed so that it can fetch all the cell sites without filtering
                int operatorId = 0;
                CellSitesVM viewModel = _handler.GetCellSites(operatorId, skip, take);

                var pageShape = Shape.Pager(pager).TotalItemCount(viewModel.TotalNumberOfCellSites);


                viewModel.Pager = pageShape;
                return View(viewModel);
            }
            #region Catch clauses 
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return RedirectToRoute("MDA.Dashboard");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute("MDA.Dashboard");
            }
            #endregion
        }



        [HttpGet]
        public ActionResult AddCellSites(string payerId)
        {
            try
            {
                //_cellSitesHandler.CheckForPermission(Permissions.AddCellSites);
                return View(_operatorHandler.GetOperator(payerId));
            }
            catch (NoRecordFoundException exception)
            {
                //do work here
                Logger.Error(exception, string.Format("Tax payer not found {0}. Exception {1}", payerId, exception.ToString()));
                _notifier.Add(NotifyType.Error, ErrorLang.notaxpayerrecord404(payerId));
                return RedirectToAction("AddOperator", "Operator", new { });
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute("MDA.Dashboard");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCellSites(TaxPayerWithDetails model)
        {
            try
            {
                // _cellSitesHandler.CheckForPermission(Permissions.AddCellSites);

                var uploadedFile = HttpContext.Request.Files.Get("CellSitesFile");

                if (uploadedFile == null || uploadedFile.ContentLength <= 0)
                {
                    Logger.Error("File content is empty for file upload");
                }
                CellSitesFileValidationObject response = _handler.CreateCellSites(model.PayerId, uploadedFile, new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id }, null);
                if (!response.HeaderHasErrors)
                {
                    return RedirectToRoute("CellSitesFileUploadReport", new { scheduleRef = response.ScheduleStagingBatchNumber });
                }
                return View(model);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception.Message, exception);
                _notifier.Add(NotifyType.Error, ErrorLang.notaxpayerrecord404(model.PayerId));
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return RedirectToRoute("MDA.Dashboard");
        }


    }
}