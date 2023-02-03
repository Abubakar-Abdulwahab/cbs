using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Mvc.AntiForgery;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        //private dynamic Shape { get; set; }
        private readonly IPaymentHandler _handler;
        public ILogger Logger { get; set; }


        public PaymentController(IOrchardServices orchardServices, IPaymentHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// View receipt
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public ActionResult ViewReceipt(string receiptNumber)
        {
            try
            {
                CreateReceiptDocumentVM result = _handler.CreateReceiptByteFile(receiptNumber);
                return File(result.DocByte, "application/pdf");
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to ViewReceipt without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Information, ErrorLang.usernotauthorized());
            }
            catch(NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Information, ErrorLang.norecord404());
            }
            catch (Exception ex)
            {
                _notifier.Error(ErrorLang.genericexception());
                Logger.Error(ex, ex.Message);
            }
            return Redirect("~/Admin");
        }


        [Admin]
        public ActionResult SearchInvoicePayment()
        {
            try
            {
                bool hasErrors = false;
                string errorMessage = string.Empty;

                try
                {
                    if (TempData.ContainsKey("SearchInvoicePaymentErrorMessage"))
                    {
                        hasErrors = true;
                        errorMessage = TempData["SearchInvoicePaymentErrorMessage"].ToString();
                        TempData.Remove("SearchInvoicePaymentErrorMessage");
                    }
                }
                catch (Exception)
                { Logger.Error("Error getting SearchInvoicePaymentErrorMessage from temp data SearchInvoicePayment OPS"); }
                _handler.CheckForPermission(Permissions.AddPayments);
                return View(new SearchInvoicePaymentVM { HasError = hasErrors, ErrorMessage = errorMessage });
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception " + exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            } 
            #endregion
            return Redirect("~/Admin");
        }


        [Admin]
        [HttpPost, ActionName("SearchInvoicePayment")]
        public ActionResult SearchInvoicePayment(string InvoiceNumber)
        {
            try
            {
                _handler.CheckForPermission(Permissions.AddPayments);
                return RedirectToAction("AddInvoicePayment", new { InvoiceNumber = InvoiceNumber.Trim() });
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception " + exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }


        [Admin]
        public ActionResult AddInvoicePayment(string InvoiceNumber)
        {
            try
            {
                return View(_handler.GetAddPaymentVM(InvoiceNumber));
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to AddInvoicePayment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, message + exception.Message);
                _notifier.Add(NotifyType.Information, ErrorLang.usernotauthorized());
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Information, Lang.invoicealreadypaid(InvoiceNumber));
                return RedirectToRoute("AssessmentReport");
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Information, ErrorLang.invoice404());
                //add error message here
                TempData = null;
                TempData.Add("SearchInvoicePaymentErrorMessage", ErrorLang.invoice404(InvoiceNumber).ToString());
                return RedirectToRoute("SearchInvoicePayment");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception in AddInvoicePayment inv num : " + InvoiceNumber + exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }


        [Admin]
        [HttpPost, ActionName("AddInvoicePayment")]
        public ActionResult AddInvoicePayment(AddPaymentVM model, string invoiceNumber)
        {
            try
            {
                _handler.UpdateInvoicePayment(this, model);
                _notifier.Add(NotifyType.Information, Lang.invoicepaymentadded(invoiceNumber, model.AmountPaid));
                return RedirectToRoute("AssessmentReport");
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to add payment without permission", _orchardServices.WorkContext.CurrentUser.UserName);
                Logger.Error(exception, string.Format("Excpetion {0}. Msg {1}", exception.Message, message));
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                //_notifier.Add(NotifyType.Information, Lang.invoicealreadypaid(InvoiceNumber));
                return RedirectToRoute("AssessmentReport");
            }
            catch(DirtyFormDataException)
            {
                var invoiceDetails = _handler.GetAddPaymentVM(model.InvoiceNumber);
                model.InvoiceNumber = invoiceDetails.InvoiceNumber;
                model.DueDate = invoiceDetails.DueDate;
                model.AmountDue = invoiceDetails.AmountDue;
                model.Recipient = invoiceDetails.Recipient;
                model.TIN = invoiceDetails.TIN;
                model.PhoneNumber = invoiceDetails.PhoneNumber;
                model.Email = invoiceDetails.Email;
                return View(model);
            }
            catch (NoInvoicesMatchingTheParametersFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Information, ErrorLang.invoice404());
                return RedirectToRoute("AssessmentReport");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception in AddInvoicePayment : " + exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }

        public ActionResult PostMockNotif()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryTokenOrchard(false)]
        public string ClientCallBackMock(PaymentNotification model)
        { return JsonConvert.SerializeObject(model); }
    }
}