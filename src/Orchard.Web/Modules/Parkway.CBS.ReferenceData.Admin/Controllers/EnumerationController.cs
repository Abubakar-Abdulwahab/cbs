using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.ReferenceData.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.ReferenceData.Admin.Controllers
{
    [Admin]
    public class EnumerationController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public IDataEnumerationHandler _handler;
        dynamic Shape { get; set; }
        private Localizer T { get; }

        public EnumerationController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IDataEnumerationHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        [HttpGet]
        public ActionResult UploadData()
        {
            try
            {
                _handler.CheckForPermission(Permissions.EnumerationData);
                return View(_handler.GetLGAsAndAdapters());
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.tenant404());
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return Redirect("/Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadData(ValidateFileModel model)
        {
            try
            {
                _handler.CheckForPermission(Permissions.EnumerationData);
                var uploadedFile = HttpContext.Request.Files.Get("EnumerationFile");

                if (uploadedFile == null || uploadedFile.ContentLength <= 0)
                {
                    Logger.Error("File content is empty for file upload");
                    _notifier.Add(NotifyType.Information, ErrorLang.valuerequired("File content"));
                    return View(_handler.GetLGAsAndAdapters());
                }

                ValidateFileResponseVM response = _handler.ProcessEnumerationDataFile(uploadedFile, new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id }, model);
                if (response.ErrorOccurred)
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.unsupportedfiletype($"{response.ErrorMessage}"));
                    return View(_handler.GetLGAsAndAdapters());
                }

                TempData.Add("batchId", response.BatchId);

                return RedirectToAction(response.RedirectToAction);
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.givemelocalizedmessage(exception.Message));
                return View(_handler.GetLGAsAndAdapters());
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return Redirect("/Admin");
        }

        public ActionResult CheckBatchRecords(ReferenceDataBatchVM model, PagerParameters pagerParameters, string endRange)
        {
            try
            {
                _handler.CheckForPermission(Permissions.EnumerationData);

                try
                {
                    if (TempData.ContainsKey("batchId"))
                    {
                        int batchId = 0;
                        if (int.TryParse(TempData["batchId"].ToString(), out batchId)) { }

                        if (batchId > 0)
                        {
                            var batchRef = _handler.GetReferenceDataBatchRef(batchId);
                            if (!string.IsNullOrEmpty(batchRef)) { _notifier.Add(NotifyType.Information, Lang.referencedatasavedsuccessfully(batchRef)); }
                        }
                    }
                }
                catch (Exception) { }

                TempData = null;
                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(model.FromRange) && !string.IsNullOrEmpty(model.EndRange))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(model.FromRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(model.EndRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        startDate = new DateTime(2017, 04, 10);
                        endDate = DateTime.Now;
                    }
                }
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;
                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                ReferenceDataBatchSearchParams searchData = new ReferenceDataBatchSearchParams
                {
                    FromRange = startDate,
                    EndRange = endDate,
                    BatchRef = model.BatchRef
                };


                ReferenceDataBatchVM VMmodel = _handler.GetCollectionReport(skip, take, searchData);

                var pageShape = Shape.Pager(pager).TotalItemCount(VMmodel.TotalNumberOfRecords);

                VMmodel.FromRange = startDate.ToString("dd'/'MM'/'yyyy");
                VMmodel.EndRange = endDate.ToString("dd'/'MM'/'yyyy");

                #region routedata
                RouteData routeData = new RouteData();
                routeData.Values.Add("FromRange", model.FromRange);
                routeData.Values.Add("EndRange", model.EndRange);
                if (!string.IsNullOrEmpty(model.BatchRef)) { routeData.Values.Add("BatchRef", model.BatchRef); }
                #endregion


                VMmodel.Pager = pageShape;
                return View(VMmodel);
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return Redirect("/Admin");
        }

        public ActionResult CheckNAGISBatchRecords()
        {
            try
            {
                _handler.CheckForPermission(Permissions.EnumerationData);

                try
                {
                    if (TempData.ContainsKey("batchId"))
                    {
                        int batchId = 0;
                        if (int.TryParse(TempData["batchId"].ToString(), out batchId)) { }

                        if (batchId > 0)
                        {
                            var batchRef = _handler.GetNAGISDataBatchRef(batchId);
                            if (!string.IsNullOrEmpty(batchRef)) { _notifier.Add(NotifyType.Information, Lang.referencedatasavedsuccessfully(batchRef)); }
                        }
                    }
                }
                catch (Exception) { throw; }

                TempData = null;

               NAGISDataBatchVM VMmodel = _handler.GetCollectionReport();
                return View(VMmodel);
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return Redirect("/Admin");
        }

        public ActionResult DownloadBatchPdfInvoice(string batchRef)
        {
            try
            {
                _handler.CheckForPermission(Permissions.EnumerationData);

                var objRefDataBatch = _handler.GetReferenceDataBatch(batchRef);
                string extension = Path.GetExtension(objRefDataBatch.BatchInvoiceFileName);
                Response.Clear();
                Response.ClearHeaders();
                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + objRefDataBatch.BatchRef+extension);
                Response.TransmitFile(objRefDataBatch.BatchInvoiceFileName);
                Response.End();
                Response.Flush();
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.givemelocalizedmessage(exception.Message));
                return Redirect("/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, Lang.genericexception_text);
                return Redirect("/Admin");
            }
            #endregion

            return null;
        }

        public async Task<ActionResult> ExportNAGISRecordRecords(long batchId)
        {
            try
            {
                _handler.CheckForPermission(Permissions.EnumerationData);

                string site = _orchardServices.WorkContext.CurrentSite.SiteName;
                string tenantIdentifier = site.Replace(" ", "");
                string fileName = "NAGISInvoiceData"+".xlsx";
                var headers = TemplateUtil.KeyValueTemplateFor("NAGISResponse", tenantIdentifier);


                NAGISInvoiceSummaryVM VMmodel = _handler.GetNAGISInvoiceSummaryCollection(batchId);
                IExcelExportEngine _excel = new ExcelEngine(headers);
                var excelData = await _excel.DownloadAsExcelAsync(headers, VMmodel.ReportRecords, fileName, "", "");
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                System.Web.HttpContext.Current.Response.OutputStream.Write(excelData, 0, excelData.Length);
                System.Web.HttpContext.Current.Response.End();
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(string.Format("Error occurred {0}", exception));
                _notifier.Add(NotifyType.Error, ErrorLang.givemelocalizedmessage(exception.Message));
                return Redirect("/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, Lang.genericexception_text);
                return Redirect("/Admin");
            }
            #endregion

            return null;

        }

    }
}