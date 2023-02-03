using Orchard;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Orchard.Logging;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using System.Globalization;
using Parkway.CBS.Core.DataFilters.SettlementRules;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class SettlementHandler : BaseHandler, ISettlementHandler
    {
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }
        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        public IInvoicingService _invoicingService;
        private readonly ICoreSettingsService _coreSettingsService;
        private readonly ICoreSettlementRuleService _coreSettlementRuleService;
        private readonly ISettlementRulesFilter _settlementRulesFilter;

        public SettlementHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IInvoicingService invoicingService, ICoreSettingsService coreSettingsService, ICoreSettlementRuleService coreSettlementRuleService, ISettlementRulesFilter settlementRulesFilter) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _settingsRepository = settingsRepository;
            _invoicingService = invoicingService;
            _coreSettingsService = coreSettingsService;
            _coreSettlementRuleService = coreSettlementRuleService;
            _settlementRulesFilter = settlementRulesFilter;
        }


        /// <summary>
        /// Get view for creation of settlement rule
        /// </summary>
        /// <returns>SettlementRuleVM</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public SettlementRuleVM GetCreateSettlementRule(List<string> sMdaIds)
        {
            IsAuthorized<SettlementHandler>(Permissions.CreateSettlementRule);
            return _coreSettlementRuleService.GetModelForCreateSettlememtView(sMdaIds);
        }


        /// <summary>
        /// Get billable revenue heads that belong to this MDA
        /// </summary>
        /// <param name="sId">string value of the MDAId</param>
        /// <returns>APIResponse</returns>
        public APIResponse GetRevenueHeads(string sId)
        {
            try
            {
                int id = 0;
                if (!(int.TryParse(sId, out id)))
                { return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().ToString() }; }

                List<RevenueHeadVM> vms = _coreSettlementRuleService.GetRevenueHeads(id).ToList();
                return new APIResponse { ResponseObject = vms };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " Get billable revenue heads that belong to this MDA Id " + sId);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }

        /// <summary>
        /// Gets all the revenue heads for the mdas selected
        /// </summary>
        /// <param name="mdaIds"></param>
        /// <returns></returns>
        public APIResponse GetRevenueHeadsPerMda(string mdaIds)
        {
            try
            {
                return new APIResponse { ResponseObject = _coreSettlementRuleService.GetRevenueHeadsPerMda(mdaIds, GetUser(_orchardServices.WorkContext.CurrentUser.Id).Id) };
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message + " Getting billable revenue heads on access list");
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }


        /// <summary>
        /// try create settlement rule
        /// </summary>
        /// <param name="model"></param>
        public void TryCreateSettlementRule(SettlementController callback, SettlementRuleVM model)
        {
            IsAuthorized<SettlementHandler>(Permissions.CreateSettlementRule);
            List<ErrorModel> errors = new List<ErrorModel> { };
            //do validation here

            if (!model.SettlementFrequencyModel.EoDSettlement)
            {
                if (model.SettlementFrequencyModel.FrequencyType == Core.Models.Enums.FrequencyType.None)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "", FieldName = "SettlementFrequencyModel.FrequencyType" });
                }
                //check date format
                if (!string.IsNullOrEmpty(model.SettlementFrequencyModel.FixedBill.StartFrom) && !string.IsNullOrEmpty(model.SettlementFrequencyModel.FixedBill.StartTime))
                {
                    DateTime startDateTimeForSettlement;
                    bool parsed = DateTime.TryParseExact(model.SettlementFrequencyModel.FixedBill.StartFrom.Trim() + " " + model.SettlementFrequencyModel.FixedBill.StartTime.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDateTimeForSettlement);
                    if (!parsed)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "This field is required. Format day/month/year example: 09/09/2019", FieldName = "SettlementFrequencyModel.FixedBill.StartFrom" });
                        errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartTime", ErrorMessage = "A valid time is required" });
                    }
                    else
                    {
                        var dateNow = DateTime.Now.ToLocalTime();
                        Logger.Information(string.Format("Comparing start date {0} to date now {1}", startDateTimeForSettlement, dateNow.ToString()));
                        if (dateNow >= startDateTimeForSettlement)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Date time is in the past. Pick a date and time in the future", FieldName = "SettlementFrequencyModel.FixedBill.StartFrom" });
                            errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartTime", ErrorMessage = "Date time is in the past. Pick a date and time in the future" });
                        }
                        model.SettlementFrequencyModel.FixedBill.StartDateAndTime = startDateTimeForSettlement;
                    }
                }
                else
                {
                    errors.Add(new ErrorModel { ErrorMessage = "This field is required. Format day/month/year example: 09/09/2019", FieldName = "SettlementFrequencyModel.FixedBill.StartFrom" });
                    errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartTime", ErrorMessage = "A valid time is required" });
                }
            }

            //check for  selected mda and revenue head
            int mdaId, paymentProviderId, paymentChannelId = 0;
            //if (!int.TryParse(model.SMDAId, out mdaId))
            //{
            //    errors.Add(new ErrorModel { ErrorMessage = "Select a valid MDA", FieldName = "SMDAId" });
            //}

            if (model.RevenueHeadsSelected == null || model.RevenueHeadsSelected.Count <= 0)
            {
                if (model.SMDAIds.Count == 1 && model.SMDAIds.Single() == "0" || model.SMDAIds.Contains("0"))
                {
                    model.RevenueHeadsSelected = new List<int> { { 0 } };
                }
                else
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Please select any or all revenue heads", FieldName = "SRevenueHeadId" });
                }
            }

            if (!int.TryParse(model.SPaymentProviderId, out paymentProviderId))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Select a valid Payment Provider", FieldName = "SPaymentProviderId" });
            }

            if (!int.TryParse(model.SPaymentChannelId, out paymentChannelId))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Select a valid Payment Channel", FieldName = "SPaymentChannelId" });
            }

            //model.MDAId = mdaId;
            model.PaymentProviderId = paymentProviderId;
            model.PaymentChannelId = paymentChannelId;
            try
            {
                if (errors.Any()) { throw new DirtyFormDataException(); }

                model.ApplyToAllRevenueHeads = model.RevenueHeadsSelected.Contains(0);
                _coreSettlementRuleService.TrySaveSettlementRule(model, _settingsRepository.User(_orchardServices.WorkContext.CurrentUser.Id), ref errors);
            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<SettlementHandler, SettlementController>(callback, errors);
            }
            catch (Exception exception)
            {
                AddValidationErrorsToCallback<SettlementHandler, SettlementController>(callback, errors, false);
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// try create settlement rule staging
        /// </summary>
        /// <param name="model"></param>
        public void TryCreateSettlementRuleForStaging(SettlementController callback, SettlementRuleVM model)
        {
            IsAuthorized<SettlementHandler>(Permissions.CreateSettlementRule);
            List<ErrorModel> errors = new List<ErrorModel> { };
            //do validation here
            if (!model.SettlementFrequencyModel.EoDSettlement)
            {
                throw new NotImplementedException();
                if (model.SettlementFrequencyModel.FrequencyType == Core.Models.Enums.FrequencyType.None)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.fieldrequired("Frequency Type").ToString(), FieldName = "SettlementFrequencyModel.FrequencyType" });
                }
                //check date format
                if (!string.IsNullOrEmpty(model.SettlementFrequencyModel.FixedBill.StartFrom) && !string.IsNullOrEmpty(model.SettlementFrequencyModel.FixedBill.StartTime))
                {
                    DateTime startDateTimeForSettlement;
                    bool parsed = DateTime.TryParseExact(model.SettlementFrequencyModel.FixedBill.StartFrom.Trim() + " " + model.SettlementFrequencyModel.FixedBill.StartTime.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDateTimeForSettlement);
                    if (!parsed)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "This field is required. Format day/month/year example: 09/09/2019", FieldName = "SettlementFrequencyModel.FixedBill.StartFrom" });
                        errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartTime", ErrorMessage = "A valid time is required" });
                    }
                    else
                    {
                        var dateNow = DateTime.Now.ToLocalTime();
                        Logger.Information(string.Format("Comparing start date {0} to date now {1}", startDateTimeForSettlement, dateNow.ToString()));
                        if (dateNow >= startDateTimeForSettlement)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Date time is in the past. Pick a date and time in the future", FieldName = "SettlementFrequencyModel.FixedBill.StartFrom" });
                            errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartTime", ErrorMessage = "Date time is in the past. Pick a date and time in the future" });
                        }
                        model.SettlementFrequencyModel.FixedBill.StartDateAndTime = startDateTimeForSettlement;
                    }
                }
                else
                {
                    errors.Add(new ErrorModel { ErrorMessage = "This field is required. Format day/month/year example: 09/09/2019", FieldName = "SettlementFrequencyModel.FixedBill.StartFrom" });
                    errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartTime", ErrorMessage = "A valid time is required" });
                }
            }

            try
            {
                if (errors.Any()) { throw new DirtyFormDataException(); }

                _coreSettlementRuleService.TrySaveSettlementRuleToStaging(model, _settingsRepository.User(_orchardServices.WorkContext.CurrentUser.Id), ref errors);

                if (errors.Any()) { throw new DirtyFormDataException(); }
            }
            catch (Exception exception)
            {
                AddValidationErrorsToCallback<SettlementHandler, SettlementController>(callback, errors, false);
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public SettlementsViewModel GetSettlementRulesReport(SettlementController callBack, SettlementRulesSearchParams searchParams)
        {
            IsAuthorized<SettlementHandler>(Permissions.CreateSettlementRule);
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                searchParams.Admin = GetUser(_orchardServices.WorkContext.CurrentUser.Id);
                SettlementsViewModel vm = new SettlementsViewModel { RuleRecords = new List<SettlementRuleLite> { } };
                dynamic recordsAndAggregate =  _settlementRulesFilter.GetSettlementRulesListViewModel(searchParams);

                vm.RuleRecords = ((IEnumerable<SettlementRuleLite>) recordsAndAggregate.RulesRecords)?.ToList();
                vm.TotalRules = ((IEnumerable<ReportStatsVM>) recordsAndAggregate.Aggregate).First().TotalRecordCount;

                return vm;
            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<SettlementHandler, SettlementController>(callBack, errors, false);
                throw;
            }
            catch (Exception exception)
            {
                AddValidationErrorsToCallback<SettlementHandler, SettlementController>(callBack, errors, false);
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
        
    }
}