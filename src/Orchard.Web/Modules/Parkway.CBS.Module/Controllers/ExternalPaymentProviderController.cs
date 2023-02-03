using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class ExternalPaymentProviderController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        private readonly IExternalPaymentProviderHandler _handler;
        int oneMonth = 1;
        int maximumListMonths = 6;

        public ExternalPaymentProviderController(IOrchardServices orchardServices, IExternalPaymentProviderHandler handler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _handler = handler;
            Shape = shapeFactory;
        }


        // GET: ExternalPaymentProvider
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                _handler.CheckForPermission(Permissions.CreateExternalPaymentProvider);
                ExternalPaymentProviderVM vm = new ExternalPaymentProviderVM { };
                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = $"User ID {_orchardServices.WorkContext.CurrentUser.Id} tried to create an external payment provider without permission";
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error ,ErrorLang.genericexception());
            }
            return Redirect("~/Admin");
        }


        [HttpPost]
        public ActionResult Create(ExternalPaymentProviderVM userInput)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CreateExternalPaymentProvider);
                _handler.TryCreateExtPaymentProvider(this, userInput);
                _notifier.Add(NotifyType.Information, Lang.extpaymentprovidersaved);
                return RedirectToRoute("ExternalPaymentProvider.List");
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = $"User ID {_orchardServices.WorkContext.CurrentUser.Id} tried to create an external payment provider without permission";
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error ,ErrorLang.genericexception());
            }

            return View(userInput);
        }


        public ActionResult List(PagerParameters pagerParameters, string from, string end)
        {
            try
            {
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(end))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth; //Added a month to balance the substraction e.g Jannuary to April, 4-1 will be 3 months instead of 4
                        if (selectedMonth > maximumListMonths)
                        {
                            Logger.Debug("Max number of requests exceeded");
                            _notifier.Add(NotifyType.Error, ErrorLang.maximumreportmonthsrexceeded());
                            return RedirectToRoute("ExternalPaymentProvider.List");
                        }
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

                var searchData = new PaymentProviderSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Skip = skip,
                    Take = take
                };

                PaymentProviderListVM model = _handler.GetPaymentProviderListVM(searchData);

                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalProviders);

                model.Pager = pageShape;

                return View(model);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = $"User ID {_orchardServices.WorkContext.CurrentUser.Id} tried to view external payment providers without permission";
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

    }
}