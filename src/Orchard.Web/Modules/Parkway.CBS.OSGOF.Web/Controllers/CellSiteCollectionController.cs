using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Middleware.Filters;
using Parkway.CBS.Module.Web.ViewModels;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.OSGOF.Web.Controllers
{
    [CBSCollectionAuthorized]
    public class CellSiteCollectionController : BaseController
    {
        private readonly ICellSiteHandler _handler;
        private readonly IOrchardServices _orchardServices;

        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;

        public CellSiteCollectionController(ICellSiteHandler handler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
        }


        /// <summary>
        /// <para>URL Name: process-cell-site-file</para>
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        public virtual JsonResult StartCellSiteProcessing(string batchToken)
        {
            Logger.Information("starting process osgof cell sites file upload");
            return Json(_handler.ProcessCellSiteFile(batchToken), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// When the file contents have been read, we call this method to validate the records stored
        /// in the client payments table for cell sites against the stored cell sites table
        /// <para>URL Name: process-cell-site-file</para>
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        public virtual JsonResult CellSiteComparison(string batchToken)
        {
            Logger.Information("starting process osgof cell sites file upload");
            return Json(_handler.ValidateCellSitesAgainstStoredRecords(batchToken), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// RouteName: OSGOF.C.CellSiteDataDetails"
        /// When the file contents have been read, we call this method to validate the records stored
        /// in the client payments table for cell sites against the stored cell sites table
        /// <para>URL Name: process-cell-site-schedule-details</para>
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        public virtual JsonResult CellSitePaymentScheduleDetails(string batchToken)
        {
            Logger.Information("get osgof payment schedule details");
            return Json(_handler.GetPaymentScheduleDetails(batchToken), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// <para>URL Name: get-next-cellsites-page</para>
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual JsonResult CellSitesMoveRight(string batchToken, int page)
        {
            Logger.Information(string.Format("getting page data for batch token - {0} page - {1}", batchToken, page));
            return Json(_handler.GetPagedCellSiteData(batchToken, page), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// RouteName: OSGOF.C.File.Upload
        /// </summary>
        /// <returns></returns>
        [HttpPost, ActionName("OSGOFFileUpload")]
        public virtual ActionResult OSGOFFileUpload()
        {
            Logger.Information("File upload ops");
            TempData = null;
            //check if the session is still valid
            if (System.Web.HttpContext.Current.Session["InvoiceGenerationStage"] == null)
            {
                Logger.Error("Session not found");
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                //We could either use this or the below
                //return RedirectToRouteX("C.SelfAssessment");
                //return RedirectToRoute("OSGOF.C.SelfAssessment");
                return RedirectToRoute("C.SelfAssessment");
            }

            GenerateInvoiceStepsModel processStage = null;
            var sProcessStage = System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString();

            try { processStage = JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(sProcessStage); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Could not deserialize process stage {0} {1} ", sProcessStage, exception.Message));
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                //return RedirectToRoute("OSGOF.C.SelfAssessment");
                return RedirectToRoute("C.SelfAssessment");
            }

            if (processStage == null)
            {
                Logger.Error(string.Format("Could not deserialize process stage {0}", sProcessStage));
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                //return RedirectToRoute("OSGOF.C.SelfAssessment");
                return RedirectToRoute("C.SelfAssessment");
            }

            TempData.Add("RevenueHeadIdentifier", processStage.RevenueHeadId.ToString());
            TempData.Add("TaxCategory", processStage.CategoryId.ToString());

            int revId = processStage.RevenueHeadId;
            int catId = processStage.CategoryId;
            string externalRef = processStage.ExternalRef;

            //lets check if the user is logged in so we can get the authorizing user for this invoice
            var user = GetLoggedInUserDetails();
            TaxEntity entity = null;
            bool fromProfile = false;
            //if the user is logged in let's get the tax profile attached to it
            if (user != null && user.Entity != null) { entity = user.Entity; }

            if (entity == null)
            {
                if (processStage.ProceedWithInvoiceGenerationVM != null)
                {
                    if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                    { entity = processStage.ProceedWithInvoiceGenerationVM.Entity; fromProfile = true; }
                }
            }

            if (entity == null)
            {
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                //return RedirectToRoute("OSGOF.C.SelfAssessment");
                return RedirectToRoute("C.SelfAssessment");
            }

            string errorMessage = string.Empty;

            if (processStage.InvoiceGenerationStage != InvoiceGenerationStage.InvoiceGenerated)
            {
                Logger.Error(string.Format("invalid stage {0} expected {1}", processStage.InvoiceGenerationStage, InvoiceGenerationStage.InvoiceGenerated.ToString()));
                Session.Remove("InvoiceGenerationStage");
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                //return RedirectToRoute("OSGOF.C.SelfAssessment");
                return RedirectToRoute("C.SelfAssessment");
            }

            try
            {
                //getting file details
                var file = HttpContext.Request.Files.Get("cellsitesfile");
                if (file == null || file.ContentLength <= 0)
                {
                    Logger.Error("File content is empty for file upload");
                    TempData.Add("Error", "Upload file not found.");
                    //return RedirectToRoute("OSGOF.C.SelfAssessment");
                    return RedirectToRoute("C.SelfAssessment");
                }
                //saving file details
                processStage.ProcessingDirectAssessmentReportVM = _handler.ProcessCellSitesFileUpload(file, processStage, entity, user.CBSUser);

                //keep a copy of the model before this so we can go back if we want
                processStage.InvoiceGenerationStage = InvoiceGenerationStage.ShowInvoiceConfirm;
                processStage.ProcessingDirectAssessmentReportVM.Type = PayeAssessmentType.FileUpload;
                if (fromProfile)
                { processStage.ProcessingDirectAssessmentReportVM.FromTaxProfileSetup = true; }
                processStage.ExternalRef = externalRef;
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return RedirectToRouteX("Confirm.Bill");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                var msg = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
                TempData["Error"] = msg;
            }
            //return RedirectToRoute("OSGOF.C.SelfAssessment");
            return RedirectToRoute("C.SelfAssessment");
        }


        [HttpPost, ActionName("OSGOFOnScreenInput")]
        public virtual ActionResult OSGOFOnScreenInput(ICollection<FileUploadCellSites> CellSites)
        {
            Logger.Information("Processing for onscreen osgof post");
            TempData = null;
            //check if session is valid
            if (System.Web.HttpContext.Current.Session["InvoiceGenerationStage"] == null)
            {
                Logger.Error("Session not found");
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }

            GenerateInvoiceStepsModel processStage = null;
            var sProcessStage = System.Web.HttpContext.Current.Session["InvoiceGenerationStage"].ToString();

            try { processStage = JsonConvert.DeserializeObject<GenerateInvoiceStepsModel>(sProcessStage); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Could not deserialize process stage {0} {1} ", sProcessStage, exception.Message));
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }

            if (processStage == null)
            {
                Logger.Error(string.Format("Could not deserialize process stage {0}", sProcessStage));
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }

            TempData.Add("RevenueHeadIdentifier", processStage.RevenueHeadId);
            TempData.Add("TaxCategory", processStage.CategoryId);

            int revId = processStage.RevenueHeadId;
            int catId = processStage.CategoryId;
            string externalRef = processStage.ExternalRef;
            //lets check if the user is logged in so we can get the authorizing user for this invoice
            UserDetailsModel user = GetLoggedInUserDetails();
            TaxEntity entity = null;
            bool fromProfile = false;
            //if the user is logged in let's get the tax profile attached to it
            if (user != null && user.Entity != null) { entity = user.Entity; }

            if (entity == null)
            {
                if (processStage.ProceedWithInvoiceGenerationVM != null)
                {
                    if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                    { entity = processStage.ProceedWithInvoiceGenerationVM.Entity; fromProfile = true; }
                }
            }

            if (entity == null)
            {
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }

            string errorMessage = string.Empty;

            if (processStage.InvoiceGenerationStage != InvoiceGenerationStage.InvoiceGenerated)
            {
                Logger.Error(string.Format("invalid stage {0} expected {1}", processStage.InvoiceGenerationStage, InvoiceGenerationStage.InvoiceGenerated.ToString()));
                Session.Remove("InvoiceGenerationStage");
                TempData.Add("Error", ErrorLang.sessionended().ToString());
                return RedirectToRouteX("C.SelfAssessment");
            }

            try
            {
                processStage.ProcessingDirectAssessmentReportVM = _handler.ProcessOSGOFOnScreenAssessment(CellSites, processStage, entity, user.CBSUser);

                processStage.InvoiceGenerationStage = InvoiceGenerationStage.ShowInvoiceConfirm;
                processStage.ProcessingDirectAssessmentReportVM.Type = PayeAssessmentType.OnScreen;
                if (fromProfile)
                { processStage.ProcessingDirectAssessmentReportVM.FromTaxProfileSetup = true; }

                processStage.ExternalRef = externalRef;
                Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                return RedirectToRouteX("Confirm.Bill");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                var msg = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
                TempData["Error"] = msg;
            }
            return RedirectToRouteX("C.SelfAssessment");
        }


        /// <summary>
        /// Upload new cell site
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UploadCellSite()
        {
            var user = GetLoggedInUserDetails();
            HeaderObj headerObj = new HeaderObj { ShowSignin = true };
            if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

            UploadCellSiteVM obj = new UploadCellSiteVM { HeaderObj = headerObj, PayerId = user.Entity.PayerId, ErrorMessage = string.Empty };
            return View(obj);
        }


        /// <summary>
        /// Upload new cell site
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCellSite(UploadCellSiteVM model)
        {
            try
            {
                var user = GetLoggedInUserDetails();
                if (user == null || user.CBSUser == null)
                { throw new AuthorizedUserNotFoundException(); }

                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

                var uploadedFile = HttpContext.Request.Files.Get("cellsitesfile");
                if (uploadedFile == null || uploadedFile.ContentLength <= 0)
                {
                    Logger.Error("File content is empty for file upload");
                    return View(new UploadCellSiteVM { HeaderObj = headerObj, PayerId = user.Entity.PayerId, ErrorMessage = ErrorLang.fieldrequired("Schedule file").ToString() });
                }
                //TODO check for mime type to be excel format

                CellSitesFileValidationObject response = _handler.CreateCellSites(model.PayerId, uploadedFile, null, new CBSUser { Id = user.CBSUser.Id });
                if (!response.HeaderHasErrors)
                {
                    //return RedirectToRoute("OSGOF.C.AddCellSite.Report", new { scheduleRef = response.ScheduleStagingBatchNumber });
                    return RedirectToRoute("C.AddCellSite.Report", new { scheduleRef = response.ScheduleStagingBatchNumber });
                }

                return View(new UploadCellSiteVM { HeaderObj = headerObj, PayerId = user.Entity.PayerId, ErrorMessage = ErrorLang.errorreadingosgofschedulefile().ToString() });
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.norecord404().ToString());
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRouteX("C.SelfAssessment");
        }


        /// <summary>
        /// 
        /// Show the result of the file upload
        /// </summary>
        [HttpGet]
        public ActionResult CellSitesFileUploadReport(string scheduleRef)
        {
            try
            {
                var user = GetLoggedInUserDetails();
                if (user == null || user.CBSUser == null)
                { throw new AuthorizedUserNotFoundException(); }

                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

                CellSitesStagingReportVM viewModel = _handler.GetStagingDataForSchdedule(scheduleRef, 10, 0);
                viewModel.HeaderObj = headerObj;
                viewModel.DoWork = false;
                //let get page size
                int chunkSize = 10;
                var dataSize = viewModel.TotalNumberOfRecords;

                double pageSize = ((double)dataSize / (double)chunkSize);
                int pages = 0;

                if (pageSize < 1 && dataSize >= 1) { pages = 1; }
                else { pages = (int)Math.Ceiling(pageSize); }

                viewModel.PageSize = pages;
                viewModel.ScheduleRef = scheduleRef;
                return View("CellSiteReport", viewModel);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.norecord404().ToString());
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            //return RedirectToRoute("OSGOF.C.SelfAssessment");
            return RedirectToRoute("C.SelfAssessment");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CellSitesFileUploadReport(string scheduleRef, string PayerId)
        {
            try
            {
                var user = GetLoggedInUserDetails();
                if (user == null || user.CBSUser == null)
                { throw new AuthorizedUserNotFoundException(); }

                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

                CellSitesStagingReportVM viewModel = _handler.CompleteCellSitesProcessingForClientUpload(scheduleRef);
                

                if (viewModel.Error) { return View(viewModel); }
                TempData = null;
                TempData.Add("Message", viewModel.Message);
                //return RedirectToRoute("OSGOF.C.CellSites.Report");
                return RedirectToRoute("C.CellSites.Report");
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.norecord404().ToString());
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            //return RedirectToRoute("OSGOF.C.SelfAssessment");
            return RedirectToRoute("C.SelfAssessment");
        }

       
        /// <summary>
        /// 
        /// Get list of cell sites for an operator
        /// </summary>
        [HttpGet]
        public ActionResult CellSiteList()
        {
            try
            {
                string message = string.Empty;
                if (TempData.ContainsKey("Message"))
                {
                    message = TempData["Message"].ToString();
                    TempData.Remove("Message");
                }


                var user = GetLoggedInUserDetails();
                if (user == null || user.CBSUser == null)
                { throw new AuthorizedUserNotFoundException(); }

                HeaderObj headerObj = new HeaderObj { ShowSignin = true };
                if (user != null && user.Entity != null) { headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

                CellSitesVM viewModel = _handler.GetCellSites(user.Entity.Id, 0, 10);
                viewModel.DoWork = false;
                viewModel.Message = message;
                viewModel.HeaderObj = headerObj;

                //let get page size
                int chunkSize = 10;
                var dataSize = viewModel.TotalNumberOfCellSites;

                double pageSize = ((double)dataSize / (double)chunkSize);
                int pages = 0;

                if (pageSize < 1 && dataSize >= 1) { pages = 1; }
                else { pages = (int)Math.Ceiling(pageSize); }

                viewModel.PageSize = pages;
                viewModel.OperatorId = user.Entity.Id;
                return View("CellSiteList", viewModel);
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.norecord404().ToString());
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            //return RedirectToRoute("OSGOF.C.SelfAssessment");
            return RedirectToRoute("C.SelfAssessment");
        }

    }
}