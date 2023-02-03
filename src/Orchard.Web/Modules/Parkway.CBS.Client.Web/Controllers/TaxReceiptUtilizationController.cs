using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Services;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Parkway.CBS.Module.Web.Middleware.Filters;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class TaxReceiptUtilizationController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICommonBaseHandler _baseHandler;
        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipValidationService _membershipValidationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly ITaxReceiptUtilizationHandler _handler;

        public TaxReceiptUtilizationController(ICommonBaseHandler baseHandler, IOrchardServices orchardServices, IUserService userService, IMembershipService membershipService, ICBSUserManager<CBSUser> cbsUserService, IAuthenticationService authenticationService, IMembershipValidationService membershipValidationService, IUserEventHandler userEventHandler, ITaxReceiptUtilizationHandler handler) : base(orchardServices, userService, membershipService, cbsUserService, authenticationService, membershipValidationService, userEventHandler, baseHandler)
        {
            _baseHandler = baseHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _userService = userService;
            _membershipService = membershipService;
            _cbsUserService = cbsUserService;
            _authenticationService = authenticationService;
            _membershipValidationService = membershipValidationService;
            _userEventHandler = userEventHandler;
            _handler = handler;
        }


        /// <summary>
        /// route C.Tax.Receipt.Utilizations
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            try
            {
                return View();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }


        /// <summary>
        /// route C.Tax.Receipt.Utilization
        /// </summary>
        /// <returns></returns>
        [BrowserHeaderFilter]
        [HttpGet]
        public ActionResult ReceiptUtilization(string scheduleBatchRef)
        {
            string errorMessage = null;
            try
            {
                if (string.IsNullOrEmpty(scheduleBatchRef)) { throw new NoRecordFoundException("Schedule batch ref not specified"); }
                ReceiptUtilizationVM vm = _handler.GetVM(scheduleBatchRef);
                if (vm != null)
                {
                    vm.HeaderObj = HeaderFiller(GetLoggedInUserDetails());
                    return View(vm);
                }
                else { errorMessage = $"Schedule with specified batch ref {scheduleBatchRef} was not found."; }
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.norecord404(exception.Message).ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            if (errorMessage != null) { TempData = null; TempData.Add("Error", errorMessage); }
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }

        /// <summary>
        /// route C.Tax.Receipt.Utilization
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReceiptUtilization()
        {
            try
            {
                return RedirectToRoute(RouteName.PAYEBatchItemReceipt.PAYEBatchItemReceipts);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }

        /// <summary>
        /// route C.Tax.Receipt.Utilization.GenerateInvoice
        /// </summary>
        /// <param name="batchRef"></param>
        /// <param name="__RequestVerificationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateInvoice(string batchRef)
        {
            string errorMessage = null;
            int categoryId = 0;
            UserDetailsModel user = null;
            try
            {
                if (!string.IsNullOrEmpty(batchRef))
                {
                    if (Session["InvoiceGenerationStage"] != null) { Session.Remove("InvoiceGenerationStage"); }

                    user = GetLoggedInUserDetails();
                    TempData = null;

                    int revenueHeadId = _handler.GetRevenueHeadIdForPAYE();

                    if (user != null && user.Entity != null)
                    {
                        var category = user.Entity.TaxEntityCategory;
                        if (category == null)
                        {
                            Logger.Error(string.Format("No category found exception {0}", user.Entity.Id));
                            throw new NoCategoryFoundException(string.Format("No category found exception {0}", user.Entity.Id));
                        }
                        categoryId = category.Id;
                    }
                    else { throw new CBSUserNotFoundException(); }

                    GenerateInvoiceStepsModel processStage = new GenerateInvoiceStepsModel { CategoryId = categoryId, RevenueHeadId = revenueHeadId, InvoiceGenerationStage = InvoiceGenerationStage.InvoiceProceed };
                    ReceiptUtilizationVM vm = _handler.GetVM(batchRef);
                    if (vm.OutstandingAmount == 0.00m) { return RedirectToRoute(RouteName.TaxReceiptUtilization.ReceiptUtilization,new { scheduleBatchRef = vm.BatchRef }); }
                    ProceedWithInvoiceGenerationVM result = _handler.GetModelWhenUserIsSignedIn(user, revenueHeadId, categoryId);
                    processStage.BillingType = result.BillingType;
                    processStage.ProceedWithInvoiceGenerationVM = result;
                    processStage.InvoiceConfirmedModel = new InvoiceConfirmedModel
                    {
                        Amount = vm.OutstandingAmount,
                        Token = _handler.EncryptBatchToken(JsonConvert.SerializeObject(new FileProcessModel { BatchRecordId = vm.BatchRecordId })),
                        IsNotStaging = true
                    };
                    processStage.InvoiceGenerationStage = InvoiceGenerationStage.GenerateInvoice;
                    Session.Add("InvoiceGenerationStage", JsonConvert.SerializeObject(processStage));
                    return RedirectToRouteX(Module.Web.RouteName.InvoiceConfirmation.ConfirmInvoice);
                }
                else { throw new Exception("Batch ref not specified."); }
            }
            catch (CBSUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.usernotfound().ToString());
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.norecord404(exception.Message).ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            if (errorMessage != null) { TempData = null; TempData.Add("Error", errorMessage); }
            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }

    }
}