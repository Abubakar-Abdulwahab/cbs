using Newtonsoft.Json;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Controllers
{
    //[Admin]
    public class InvoiceController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        //private dynamic Shape { get; set; }
        private readonly IInvoiceHandler _handler;
        public ILogger Logger { get; set; }

        private static Localizer _t = NullLocalizer.Instance;

        private static Localizer T { get { return _t; } }

        public InvoiceController(IOrchardServices orchardServices, IInvoiceHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
        }

        public ActionResult PreviewInvoice(string invoiceNumber)
        {
            string url = _handler.GetInvoiceURL(invoiceNumber);
            return View(new InvoicePreviewVM { InvoiceURL = url });
        }


        [Admin]
        public ActionResult GenerateInvoice()
        {
            try
            { return View(_handler.GetViewForSearchForTaxEntity()); }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to CollectionReport without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in GenerateInvoice MSG: {0}", exception.Message));
            } 
            #endregion
            return Redirect("~/Admin");
        }


        [Admin]
        [HttpPost, ActionName("GenerateInvoice")]
        public ActionResult GenerateInvoice(SearchForTaxEntityVM model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.TIN) && string.IsNullOrEmpty(model.PhoneNumber))
                {
                    _notifier.Error(ErrorLang.nosearchparamforsearchfortaxpayer());
                    return View(model);
                }
                List<TaxPayerWithDetails> taxPayers = _handler.SearchForTaxPayer(model);
                return View("SelectTaxPayer", taxPayers);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to GenerateInvoice without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        [Admin]
        [HttpPost, ActionName("ConfirmTaxPayer")]
        public ActionResult ConfirmTaxPayer(string taxPayerId)
        {
            try
            {
                GenerateInvoiceConfirmTaxPayer vm = _handler.GetTaxPayerAndRevenueHeads(taxPayerId);
                if (vm.TaxPayerWithDetails == null)
                {
                    _notifier.Error(ErrorLang.taxpayer404());
                    vm = new GenerateInvoiceConfirmTaxPayer { TaxPayerWithDetails = new TaxPayerWithDetails { }, RevenueHeads = new List<RevenueHeadLite> { } };
                }
                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to ConfirmTaxPayer without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }

        /// <summary>
        /// 
        /// URL Name: ConfirmedTaxPayerAndRevenueHead
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns></returns>
        [Admin]
        [HttpPost, ActionName("ConfirmedTaxPayerAndRevenueHead")]
        public ActionResult ConfirmedTaxPayerAndRevenueHead(string taxPayerId, string revenueHeadId)
        {
            try
            {
                DateTime date = DateTime.Now.ToLocalTime();
                string sessionKey = string.Format("{0}{1}{2}", taxPayerId, revenueHeadId, date.Ticks);
                Session.Add(sessionKey, string.Empty);

                return RedirectToRoute(RouteName.GenerateInvoice.ShowFormFields,
                   new { pageToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(Util.LetsEncrypt(JsonConvert.SerializeObject(new PageToken { SessionKey = sessionKey, sTaxPayerId = taxPayerId, sRevenueHeadId = revenueHeadId, DateTimeCreated = date })))) });
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to ConfirmedTaxPayerAndRevenueHead without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        [Admin]
        public ActionResult ShowFormFields(string pageToken = null)
        {
            try
            {
                PageToken tokenModel = null;
                if (!string.IsNullOrEmpty(pageToken))
                {
                    tokenModel = JsonConvert.DeserializeObject<PageToken>(Util.LetsDecrypt(UTF8Encoding.UTF8.GetString(Convert.FromBase64String(pageToken))));
                    //does deserialization
                    //then check that the sessionkey is valid
                    if (System.Web.HttpContext.Current.Session[tokenModel.SessionKey] == null)
                    {
                        throw new TimeoutException { };
                    }
                }
                var vm = _handler.GetDetailsGenerateInvoiceDetailsWithFormFields(tokenModel.sTaxPayerId, tokenModel.sRevenueHeadId);
                if (TempData["AdminGenerateInvoiceVMPostBack"] != null && TempData.Any()) {
                    var postbackModel = JsonConvert.DeserializeObject<AdminGenerateInvoiceVM>(TempData["AdminGenerateInvoiceVMPostBack"].ToString());
                    vm.Amount = postbackModel.Amount;
                    foreach(var form in vm.Forms)
                    {
                        form.FormValue = postbackModel.Forms.Where(x => x.ControlIdentifier == form.ControlIdentifier).SingleOrDefault()?.FormValue;
                    }
                    if (!string.IsNullOrEmpty(postbackModel.errors))
                    {
                        var errors = JsonConvert.DeserializeObject<ICollection<ErrorModel>>(postbackModel.errors).ToList();
                        foreach(var error in errors)
                        {
                            ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                        }
                    }
                }
                TempData = null;
                if (!vm.Forms.Any())
                {
                    Session.Remove(tokenModel.SessionKey);
                    TempData = null;
                    TempData["TaxPayerId"] = tokenModel.sTaxPayerId;
                    TempData["RevenueHeadId"] = tokenModel.sRevenueHeadId;
                    return RedirectToRoute("GenerateInvoiceCreateBill");
                }
                return View(vm);
            }
            catch (TimeoutException)
            {
                _notifier.Error(ErrorLang.sessionended());
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to CreateBill without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }



        [Admin]
        public ActionResult CreateBill()
        {
            try
            {
                string taxPayerId = null;
                string revenueHeadId = null;
                try
                {
                    if (TempData.ContainsKey("TaxPayerId"))
                    { taxPayerId = TempData["TaxPayerId"].ToString(); }

                    if (TempData.ContainsKey("RevenueHeadId"))
                    { revenueHeadId = TempData["RevenueHeadId"].ToString(); }

                    TempData = null;
                }
                catch (Exception exception) { Logger.Error(exception, "Error getting error value from temp data " + exception.Message); }

                if(string.IsNullOrEmpty(taxPayerId))
                {
                    _notifier.Error(ErrorLang.taxpayer404());
                    return RedirectToRoute("GenerateInvoiceSearchForTaxEntity");
                }

                if (string.IsNullOrEmpty(revenueHeadId))
                {
                    _notifier.Error(ErrorLang.revenuehead404());
                    return RedirectToRoute("GenerateInvoiceSearchForTaxEntity");
                }
                TaxPayerWithDetails taxPayer = _handler.GetTaxPayer(taxPayerId);
                RevenueHeadDetails revenueHeadDetails = _handler.GetRevenueHeadDetails(revenueHeadId);

                var vm = _handler.GetViewForCreateBill(taxPayer, revenueHeadDetails);

                if (vm.ViewModel.DoesNotHaveInput)
                {
                    AdminConfirmingInvoiceVM confirmInvoiceModel = _handler.GetConfirmingInvoiceVM(vm.ViewModel, revenueHeadDetails, taxPayer);
                    string jsonValue = JsonConvert.SerializeObject(confirmInvoiceModel);
                    TempData = null;
                    TempData.Add("ConfirmInvoiceVM", jsonValue);
                    return RedirectToRoute("GenerateInvoiceConfirmInvoice");
                }

                return View(vm.ViewModel);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to CreateBill without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }



        [Admin]
        [HttpPost, ActionName("CreateBill")]
        public ActionResult CreateBill(AdminGenerateInvoiceVM model, string taxPayerId, string revenueHeadId, ICollection<FormControlViewModel> controlCollectionFromUserInput)
        {
            ViewToShowVM vm = null;
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                TaxPayerWithDetails taxPayer = _handler.GetTaxPayer(taxPayerId);
                model.Forms = controlCollectionFromUserInput?.ToList();
                RevenueHeadDetails revenueHeadDetails = _handler.ValidateInvoiceDataInput(model, revenueHeadId, taxPayer.CategoryId, ref errors);
                if(errors.Count > 0)
                {
                    //lets get the view model
                    vm = _handler.GetCallBackViewModelForInvoiceDataInput(model, taxPayerId, revenueHeadDetails);
                    //add errors to model state
                    _handler.AddErrorsToModelState(this, errors);

                }

                AdminConfirmingInvoiceVM confirmInvoiceModel = _handler.GetConfirmingInvoiceVM(model, revenueHeadDetails, taxPayer);
                string jsonValue = JsonConvert.SerializeObject(confirmInvoiceModel);
                TempData = null;
                TempData.Add("ConfirmInvoiceVM", jsonValue);
                return RedirectToRoute("GenerateInvoiceConfirmInvoice");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to CreateBill without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (DirtyFormDataException)
            {
                if(model.Forms != null && model.Forms.Any())
                {
                    DateTime date = DateTime.Now.ToLocalTime();
                    string sessionKey = string.Format("{0}{1}{2}", taxPayerId, revenueHeadId, date.Ticks);
                    Session.Add(sessionKey, string.Empty);
                    TempData = null;
                    model.errors = JsonConvert.SerializeObject(errors);
                    TempData.Add("AdminGenerateInvoiceVMPostBack", JsonConvert.SerializeObject(model));
                    return RedirectToRoute(RouteName.GenerateInvoice.ShowFormFields,
                       new { pageToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(Util.LetsEncrypt(JsonConvert.SerializeObject(new PageToken { SessionKey = sessionKey, sTaxPayerId = taxPayerId, sRevenueHeadId = revenueHeadId, DateTimeCreated = date })))) });
                }
                else { return View(vm.ViewModel); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        /// <summary>
        /// URL Name: GenerateInvoiceConfirmInvoice
        /// </summary>
        [Admin]
        public ActionResult ConfirmInvoice()
        {
            try
            {
                string displayVM = null;
                AdminConfirmingInvoiceVM model = null;
                try
                {
                    if (TempData.ContainsKey("ConfirmInvoiceVM"))
                    {
                        displayVM = TempData["ConfirmInvoiceVM"].ToString();
                        model = JsonConvert.DeserializeObject<AdminConfirmingInvoiceVM>(displayVM);
                    }
                    else
                    {
                        _notifier.Error(ErrorLang.taxpayer404());
                        return RedirectToRoute("GenerateInvoiceSearchForTaxEntity");
                    }
                    TempData = null;
                }
                catch (Exception exception) { Logger.Error(exception, "Error getting error value from temp data " + exception.Message); }

                if (model == null)
                {
                    _notifier.Error(ErrorLang.taxpayer404());
                    return RedirectToRoute("GenerateInvoiceSearchForTaxEntity");
                }
                string jsonValue = JsonConvert.SerializeObject(model);
                TempData.Add("ConfirmInvoiceVM", jsonValue);
                //get tokens
                var tokens = _handler.GetTamperProofTokens(jsonValue);
                model.SubToken = tokens.SubToken;
                model.Token = tokens.Token;
                return View(model);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to ConfirmInvoice without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        /// <summary>
        /// URL Name: GenerateInvoiceConfirmInvoice
        /// </summary>
        [Admin]
        [HttpPost, ActionName("ConfirmInvoice")]
        public ActionResult ConfirmInvoice(string token, string subToken)
        {
            TempData = null;

            try
            {
                AdminConfirmingInvoiceVM model = _handler.GetAdminConfirmingInvoiceModel(token, subToken);
                if (model == null)
                {
                    _notifier.Error(ErrorLang.genericexception());
                    return RedirectToRoute("GenerateInvoiceSearchForTaxEntity");
                }
                CreateInvoiceModel createInvoiceModel = _handler.GetCreateInvoiceModel(model);

                InvoiceGeneratedResponseExtn invoiceDetails = _handler.GenerateInvoice(createInvoiceModel);
                TempData.Add("InvoiceDetails", JsonConvert.SerializeObject(invoiceDetails));
                if (!string.IsNullOrEmpty(invoiceDetails.Message)) { _notifier.Add(NotifyType.Information, T(invoiceDetails.Message)); }
                return RedirectToRoute("GenerateInvoiceDetails", new { invoiceNumber = invoiceDetails.InvoiceNumber });
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to ConfirmInvoice without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        /// <summary>
        /// URL Name: GenerateInvoiceDetails
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        [Admin]
        public ActionResult InvoiceDetails(string invoiceNumber)
        {
            try
            {
                InvoiceGeneratedResponseExtn invoiceDetails = null;
                if (TempData.ContainsKey("InvoiceDetails"))
                {
                    invoiceDetails = JsonConvert.DeserializeObject<InvoiceGeneratedResponseExtn>(TempData["InvoiceDetails"].ToString());
                    TempData.Remove("InvoiceDetails");
                    TempData = null;
                }
                else
                {
                    invoiceDetails = _handler.GetInvoiceDetails(invoiceNumber);
                    if (invoiceDetails == null)
                    {
                        _notifier.Error(ErrorLang.invoice404());
                        return RedirectToRoute("GenerateInvoiceSearchForTaxEntity");
                    }
                }
                return View(invoiceDetails);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to InvoiceDetails without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error getting error value from temp data " + exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return RedirectToRoute("GenerateInvoiceSearchForTaxEntity");
        }



        /// <summary>
        /// Search invoice number for payment refs
        /// </summary>
        /// <returns></returns>
        [Admin]
        public ActionResult SearchInvoiceNumberForPaymentRef()
        {
            try
            {
                return View();
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to InvoiceDetails without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error getting error value from temp data " + exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        [Admin]
        [HttpPost]
        public ActionResult SearchInvoiceNumberForPaymentRef(string invoiceNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(invoiceNumber)) { throw new NoRecordFoundException(); }
                InvoiceDetails vm = _handler.GetInvoicePaymentRefs(invoiceNumber);
                if (vm == null) { throw new NoRecordFoundException(); }
                return View("InvoiceRefsResult", vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to InvoiceDetails without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (NoRecordFoundException)
            {
                var message = String.Format("Invoice number not found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.invoice404());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error getting error value from temp data " + exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }
    }
}