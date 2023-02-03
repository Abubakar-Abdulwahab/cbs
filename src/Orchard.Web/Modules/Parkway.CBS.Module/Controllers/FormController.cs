using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class FormController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        private IFormHandler _handler;

        public FormController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IFormHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
        }
        
        // GET: Form
        public ActionResult Create(string revenueHeadSlug, int revenueHeadId)
        {
            try
            {
                var model = _handler.GetFormSetupView(revenueHeadId, revenueHeadSlug);
                return View(model);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create SRH without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
            }
            catch (NoBillingDetailAddedYetException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create form when no billing info found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.addbillingddetailsfirst);
                return RedirectToAction("Create", "Billing", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create SRH but the given MDA slug could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.recordcouldnotbefound_text);
                return new HttpNotFoundResult();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
            } 
            #endregion
            return Redirect("~/Admin");
        }

        [HttpPost]
        public ActionResult Create(ICollection<FormControlViewModel> controlCollection, string revenueHeadSlug, int revenueHeadId)
        {
            try
            {
                _handler.TrySaveFormDetails(this, controlCollection, revenueHeadId, revenueHeadSlug);
                _notifier.Add(NotifyType.Information, Lang.formdatahasbeensaved);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
            }
            catch (NoBillingDetailAddedYetException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create form when no billing info found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.addbillingddetailsfirst);
                return RedirectToAction("Create", "Billing", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} revenue head 404. {1} revenue head slug {2} rev head id ", _orchardServices.WorkContext.CurrentUser.Id, revenueHeadSlug, revenueHeadId);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.recordcouldnotbefound_text);
            }
            catch (CouldNotSaveFormRevenueHeadControls exception)
            {
                var message = String.Format("\nUser ID {0} could not save form controls. {1} revenue head slug {2} rev head id ", _orchardServices.WorkContext.CurrentUser.Id, revenueHeadSlug, revenueHeadId);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.couldnotsaveformdetails);
            }            
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
            } 
            #endregion
            return RedirectToAction("RevenueHeadDashBoard", "RevenueHead", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
        }

        public ActionResult Edit(string revenueHeadSlug, int revenueHeadId)
        {
            try
            {

                List<FormControlRevenueHeadMetaDataExtended> alreadyExistingControls;
                var model = _handler.GetEditFormSetupView(revenueHeadId, revenueHeadSlug,out alreadyExistingControls);
                ViewBag.ExistingControls = alreadyExistingControls;
                
                return View(model);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
            }
            catch (NoBillingDetailAddedYetException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create form when no billing info found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.addbillingddetailsfirst);
                return RedirectToAction("Create", "Billing", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
            }
            catch (CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view edit form when no form info found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                //_notifier.Add(NotifyType.Warning, Lang.addbillingddetailsfirst);
                return RedirectToAction("Create", "Form", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} revenue head 404. {1} revenue head slug {2} rev head id ", _orchardServices.WorkContext.CurrentUser.Id, revenueHeadSlug, revenueHeadId);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.recordcouldnotbefound_text);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
            } 
            #endregion
            return RedirectToAction("RevenueHeadDashBoard", "RevenueHead", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
        }

        [HttpPost]
        public ActionResult Edit(ICollection<FormControlViewModel> controlCollection, string revenueHeadSlug, int revenueHeadId)
        {
            try
            {
                _handler.TryUpdateFormDetails(controlCollection, revenueHeadId, revenueHeadSlug);
                _notifier.Add(NotifyType.Information,Lang.formdatahasbeensaved);
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
            }
            catch (NoBillingDetailAddedYetException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create form when no billing info found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.addbillingddetailsfirst);
                return RedirectToAction("Create", "Billing", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} revenue head 404. {1} revenue head slug {2} rev head id ", _orchardServices.WorkContext.CurrentUser.Id, revenueHeadSlug, revenueHeadId);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, Lang.recordcouldnotbefound_text);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
            } 
            #endregion
            return RedirectToAction("RevenueHeadDashBoard", "RevenueHead", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
        }

        
    }
}