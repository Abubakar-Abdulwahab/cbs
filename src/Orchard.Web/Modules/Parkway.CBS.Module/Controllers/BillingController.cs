using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Payee;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class BillingController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        private IBillingHandler _handler;

        public BillingController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IBillingHandler handler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// create billing for collection
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns></returns>
        public ActionResult Create(int revenueHeadId, string revenueHeadSlug)
        {
            return DoBillingForGet(revenueHeadId, revenueHeadSlug);
        }


        /// <summary>
        /// Edit billing to add amount
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int revenueHeadId, string revenueHeadSlug)
        {
            return DoBillingForGet(revenueHeadId, revenueHeadSlug, true);
        }


        private ActionResult DoPostForBilling(BillingViewModel model, string revenueHeadSlug, int revenueHeadId, ICollection<DiscountModel> DiscountModel, ICollection<PenaltyModel> PenaltyModel, BillingFrequencyModel frequencyModel, bool isEdit = false)
        {
            CallBackObject callBackObject = new CallBackObject { };
            List<AssessmentInterface> assessmentInterfaces = new List<AssessmentInterface> { };
            try
            {
                assessmentInterfaces = _handler.GetAssessmentInterfaces();
                if (model.IsRecurring) { callBackObject.HasFrequencyValue = true; }
                _handler.TryPostBillingDetails(this, model, revenueHeadSlug, revenueHeadId, frequencyModel, DiscountModel, PenaltyModel, assessmentInterfaces, isEdit);
                if (isEdit)
                {
                    _notifier.Add(NotifyType.Information, Lang.savesuccessfully);
                    return RedirectToAction("RevenueHeadDashBoard", "RevenueHead", new { revenueHeadSlug, revenueHeadId });
                }
                return RedirectToAction("Create", "Form", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create SRH without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (CannotFindTenantIdentifierException exception)
            {
                Logger.Error(exception, "Could not find any tenant settings while getting assessment interface in create billing");
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create SRH but the given MDA slug could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenuehead404());
                return new HttpNotFoundResult();
            }
            catch (AlreadyHasBillingException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create billing when billing details already exist", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.alreadyhasbillinginfo());
                return RedirectToAction("Edit", "Billing", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
            }
            catch (CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create billing", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                return RedirectToAction("ViewSubRevenueHeads", "SubRevenueHead", new { parentrevenuedheadslug = revenueHeadSlug, parentrevenueheadid = revenueHeadId });
            }
            catch (DirtyFormDataException exception)
            {
                var message = String.Format("\nInvalid model state for billing creation  - LastUpdatedBy ID  {0}", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
            }
            catch (CouldNotSaveBillingException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create billing ", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.cannotsavebillinginfo());
            }
            catch (StartDateIsGreaterThanEndDateException exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.startdateisgreaterthanenddate(exception.Message));
            }
            catch (PayeRevenueHeadAlreadyExistsException)
            {
                Logger.Error(ErrorLang.payealreadyexists().ToString());
                _notifier.Add(NotifyType.Error, ErrorLang.payealreadyexists());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(Lang.genericexception_text);
                return Redirect("~/Admin");
            }
            #endregion
            model.CallBackObject = callBackObject;
            model.DiscountModelPostBackData = DiscountModel == null ? new List<DiscountModel>() : DiscountModel;
            model.Indexer = DiscountModel != null ? DiscountModel.Count : 0;
            model.RHName = model.RHName;
            model.PenaltyModelPostBackData = PenaltyModel == null ? new List<PenaltyModel>() : PenaltyModel;
            model.IndexerPenalty = PenaltyModel != null ? PenaltyModel.Count : 0;
            model.BillingDemandNotice = model.BillingDemandNotice ?? new BillingDemandNotice();
            model.FrequencyModel = frequencyModel ?? new BillingFrequencyModel { Duration = new DurationModel { }, FixedBill = new FixedBillingModel { }, VariableBill = new VariableBillingModel { } };
            model.FrequencyModel.Duration = frequencyModel.Duration ?? new DurationModel { };
            model.DirectAssessment.ListOfAssessmentInterface = assessmentInterfaces;
            if (model.FileUploadBillingModel == null) { model.FileUploadBillingModel = new FileUploadTemplatesVM { }; }
            model.FileUploadBillingModel.ListOfTemplates = _handler.GetFileUploadTemplatesVM().ListOfTemplates;
            model.IsEdit = isEdit;
            return View("Billing", model);
        }


        [HttpPost, ActionName("Create")]
        public ActionResult CreatePost(BillingViewModel model, string revenueHeadSlug, int revenueHeadId, ICollection<DiscountModel> DiscountModel, ICollection<PenaltyModel> PenaltyModel, BillingFrequencyModel frequencyModel)
        {
            return DoPostForBilling(model, revenueHeadSlug, revenueHeadId, DiscountModel, PenaltyModel, frequencyModel);
        }


      
        /// <summary>
        /// Do view for billing
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        private ActionResult DoBillingForGet(int revenueHeadId, string revenueHeadSlug, bool isEdit = false)
        {
            try
            {
                return View("Billing", _handler.GetBillingView(revenueHeadId, revenueHeadSlug, isEdit));
            }
            #region catch clauses
            catch (UserNotAuthorizedForThisActionException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create billing without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
            }
            catch (AlreadyHasBillingException exception)
            {
                var message = String.Format("\nUser ID {0} tried to view create billing when billing details already exist", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.alreadyhasbillinginfo());
                return RedirectToAction("Edit", "Billing", new { revenueHeadSlug = revenueHeadSlug, revenueHeadId = revenueHeadId });
            }
            catch (CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create billing", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                return RedirectToAction("ViewSubRevenueHeads", "SubRevenueHead", new { parentrevenuedheadslug = revenueHeadSlug, parentrevenueheadid = revenueHeadId });
            }
            catch (CannotFindRevenueHeadException exception)
            {
                var message = String.Format("\nUser ID {0} tried to create SRH but the given MDA slug could not be found", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(exception, exception.Message + message);
                _notifier.Add(NotifyType.Warning, ErrorLang.revenueheadnotfound());
                return new HttpNotFoundResult();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Error(ErrorLang.genericexception());
            }
            #endregion
            return Redirect("~/Admin");
        }



        /// <summary>
        /// Edit billing add amount
        /// </summary>
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(BillingViewModel model, int revenueHeadId, string revenueHeadSlug, BillingFrequencyModel frequencyModel, ICollection<DiscountModel> DiscountModel, ICollection<PenaltyModel> PenaltyModel)
        {
            return DoPostForBilling(model, revenueHeadSlug, revenueHeadId, DiscountModel, PenaltyModel, frequencyModel, true);           
        }

    }
}