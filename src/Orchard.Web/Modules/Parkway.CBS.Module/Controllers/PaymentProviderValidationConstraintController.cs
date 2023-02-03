using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class PaymentProviderValidationConstraintController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        private readonly IPaymentProviderValidationConstraintHandler _handler;
        

        public PaymentProviderValidationConstraintController(IOrchardServices orchardServices, IPaymentProviderValidationConstraintHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _handler = handler;
        }
        

        [HttpGet]
        public ActionResult Assign(string providerId)
        {
            try
            {
                int providerID = 0;
                if (!string.IsNullOrEmpty(providerId))
                {
                    int.TryParse(providerId, out providerID);
                    if(providerID < 1) { _notifier.Add(NotifyType.Error, ErrorLang.genericexception()); return RedirectToRoute("ExternalPaymentProvider.List"); }
                }
                else { _notifier.Add(NotifyType.Error, ErrorLang.genericexception()); return RedirectToRoute("ExternalPaymentProvider.List"); }

                AssignExternalPaymentProviderVM model = _handler.GetAssignExternalPaymentProviderVM(providerID);
                model.SelectedPaymentProviderParsed = providerID;

                return View(model);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = $"User ID {_orchardServices.WorkContext.CurrentUser.Id} tried to assign an external payment provider without permission";
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        [HttpPost]
        public ActionResult Assign(AssignExternalPaymentProviderVM userInput)
        {
            try
            {
                if (!string.IsNullOrEmpty(userInput.SelectedPaymentProvider))
                {
                    _handler.AssignPaymentProviderToSelectedRevenueHeads(userInput);
                    _notifier.Add(NotifyType.Information, Lang.savesuccessfully);
                    return RedirectToAction("Assign");
                }
                else { throw new Exception($"No valid mda or revenue head selected for assignment to payment provider."); }
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = $"User ID {_orchardServices.WorkContext.CurrentUser.Id} tried to assign an external payment provider without permission";
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }

            var vm = _handler.GetAssignExternalPaymentProviderVM((int)userInput.SelectedPaymentProviderParsed);
            userInput.MDAs = vm.MDAs;
            userInput.PaymentProviders = vm.PaymentProviders;

            return View(userInput);
        }

    }
}