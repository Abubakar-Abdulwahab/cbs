using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HTTP.Handlers.Billing;
using Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts;
using Newtonsoft.Json;
using Parkway.CBS.Core.Models.Enums;
using System.Globalization;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreSettlementRuleService : ICoreSettlementRuleService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMDAManager<MDA> _mdaRepository;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepo;
        private readonly Lazy<ISettlementRuleManager<SettlementRule>> _settlementRepo;
        private readonly Lazy<ISettlementRuleStagingManager<SettlementRuleStaging>> _settlementStagingRepo;
        private readonly Lazy<ISettlementRuleDetailsManager<SettlementRuleDetails>> _settlementDetailsRepo;
        private readonly Lazy<ISettlementRuleDetailsStagingManager<SettlementRuleDetailsStaging>> _settlementDetailsStagingRepo;
        private readonly Fixed _validationForSchedule;
        private readonly IExternalPaymentProviderManager<ExternalPaymentProvider> _externalPaymentProvidersRepo;

        public ILogger Logger { get; set; }


        public CoreSettlementRuleService(IOrchardServices orchardServices, IMDAManager<MDA> mdaRepository, IRevenueHeadManager<RevenueHead> revenueHeadRepo, Lazy<ISettlementRuleManager<SettlementRule>> settlementRepo, IEnumerable<IBillingFrequencyFilter> billingFrequencyFilter, Lazy<ISettlementRuleDetailsManager<SettlementRuleDetails>> settlementDetailsRepo, IExternalPaymentProviderManager<ExternalPaymentProvider> externalPaymentProvidersRepo, Lazy<ISettlementRuleDetailsStagingManager<SettlementRuleDetailsStaging>> settlementDetailsStagingRepo, Lazy<ISettlementRuleStagingManager<SettlementRuleStaging>> settlementStagingRepo)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _mdaRepository = mdaRepository;
            _revenueHeadRepo = revenueHeadRepo;
            _settlementRepo = settlementRepo;
            _validationForSchedule = new Fixed(billingFrequencyFilter);
            _settlementDetailsRepo = settlementDetailsRepo;
            _externalPaymentProvidersRepo = externalPaymentProvidersRepo;
            _settlementDetailsStagingRepo = settlementDetailsStagingRepo;
            _settlementStagingRepo = settlementStagingRepo;
        }


        /// <summary>
        /// Get list of third party payment providers
        /// </summary>
        /// <returns>SettlementRuleVM</returns>
        public SettlementRuleVM GetModelForCreateSettlememtView(List<string> MDAIds)
        {
            IEnumerable<PaymentProviderVM> providers = _externalPaymentProvidersRepo.GetProviders();
            IEnumerable<PaymentChannelVM> channels = GetPaymentChannels();

            IEnumerable<MDAVM> mdas = _revenueHeadRepo.GetMDAsForBillableRevenueHeads();

            List<RevenueHeadVM> revenueHeadVMs = new List<RevenueHeadVM> { };
            //if (mdaId > 0)
            //{
            //    revenueHeadVMs = _revenueHeadRepo.GetBillableRevenueHeadsForMDA(mdaId);
            //}
            int mdaId = 0;
            List<int> selectedMdas = new List<int> { };
            Dictionary<int, IEnumerable<int>> selectedMdasAndRevenueHeads = new Dictionary<int, IEnumerable<int>>();
            foreach (var sMdaId in MDAIds)
            {
                if (int.TryParse(sMdaId, out mdaId))
                {
                    selectedMdas.Add(mdaId);
                    if (mdaId < 1) { continue; }
                    selectedMdasAndRevenueHeads.Add(mdaId, _revenueHeadRepo.GetBillableRevenueHeadsForMDA(mdaId).Select(x => x.Id));
                }
            }



            return new SettlementRuleVM
            {
                MDAs = mdas.ToList(),
                SelectedMdas = selectedMdas,
                PaymentProviders = providers.ToList(),
                PaymentChannels = channels.ToList(),
                SettlementFrequencyModel = new SettlementFrequencyModel
                {
                    Duration = new DurationModel { },
                    FixedBill = new FixedBillingModel
                    {
                        DailyBill = new DailyBillingModel { },
                        WeeklyBill = new WeeklyBillingModel
                        {
                            Days = new List<Days> { }
                        },
                        MonthlyBill = new MonthlyBillingModel { },
                        YearlyBill = new YearlyBillingModel { MonthlyBill = new MonthlyBillingModel { }, },
                        StartFrom = DateTime.Now.ToLocalTime().AddDays(1).ToString("dd/MM/yyyy"),
                        StartTime = "00:00"
                    }
                },
                RevenueHeads = revenueHeadVMs != null ? revenueHeadVMs.ToList() : new List<RevenueHeadVM> { },
                SelectedRhAndMdas = JsonConvert.SerializeObject(selectedMdasAndRevenueHeads)
            };
        }


        /// <summary>
        /// Get channels for payments
        /// </summary>
        /// <returns>IEnumerable{PaymentChannelVM}</returns>
        private IEnumerable<PaymentChannelVM> GetPaymentChannels()
        {
            return Enum.GetValues(typeof(PaymentChannel)).Cast<PaymentChannel>().Select(e => new PaymentChannelVM { Id = (int)e, Name = e.ToDescription() });
        }



        /// <summary>
        /// Get billable revenue heads that belong to this mda
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadVM}</returns>
        public IEnumerable<RevenueHeadVM> GetRevenueHeads(int id)
        {
            return _revenueHeadRepo.GetBillableRevenueHeadsForMDA(id);
        }


        /// <summary>
        /// Get a list of revenue heads for the mdas with the specified Id
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns></returns>
        public dynamic GetRevenueHeadsPerMda(string mdaIds, int userId)
        {
            try
            {
                //int userId = GetUser(_orchardServices.WorkContext.CurrentUser.Id).Id;
                List<RevenueHeadLite> revenueHeads = new List<RevenueHeadLite> { };
                if (!string.IsNullOrEmpty(mdaIds))
                {
                    IEnumerable<int> MDAIds = JsonConvert.DeserializeObject<IEnumerable<int>>(mdaIds);
                    if (MDAIds != null)
                    {
                        foreach (var mdaId in MDAIds)
                        {
                            revenueHeads.AddRange(_revenueHeadRepo.GetRevenueHeadsPerMdaOnAccessList(mdaId, userId, false));
                        }
                    }
                    else { throw new Exception("No MDA selected."); }
                }
                return revenueHeads.GroupBy(rh => rh.MDAId).ToList();
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Try save settlement rule
        /// </summary>
        /// <param name="model"><see cref="SettlementRuleVM"/></param>
        /// <param name="adminUser"></param>
        /// <param name="errors"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <exception cref="RecordAlreadyExistsException"></exception>
        /// <exception cref="Exception"></exception>
        public void TrySaveSettlementRule(SettlementRuleVM model, UserPartRecord adminUser, ref List<ErrorModel> errors)
        {
            try
            {
                //check for name
                SettlementRule exists = _settlementRepo.Value.Get(x => x.Name == model.Name);
                if (exists != null)
                {
                    errors.Add(new ErrorModel { FieldName = "Name", ErrorMessage = string.Format("A rule with the name {0} already exists", model.Name) });
                    throw new DirtyFormDataException { };
                }
                DateTime? nextScheduledDateTimeToRun = null;
                string cronExpression = string.Empty;

                if (!model.SettlementFrequencyModel.EoDSettlement)
                {
                    model.SettlementFrequencyModel.FixedBill.YearlyBill.MonthlyBill = model.SettlementFrequencyModel.FixedBill.MonthlyBill;
                    cronExpression = _validationForSchedule.ValidateAndGetCronExpression(model.SettlementFrequencyModel.FrequencyType, model.SettlementFrequencyModel.FixedBill, model.SettlementFrequencyModel.FixedBill.StartDateAndTime);
                    //get next run date
                    nextScheduledDateTimeToRun = Utilities.Util.GetNextDate(cronExpression, model.SettlementFrequencyModel.FixedBill.StartDateAndTime);
                    if (nextScheduledDateTimeToRun == null)
                    {
                        errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartTime", ErrorMessage = ErrorLang.couldnotgetnextscheduledate().ToString() });
                        throw new DirtyFormDataException { };
                    }
                }
                else
                {
                    DateTime userInputStartDate = DateTime.Now;
                    if (!DateTime.TryParseExact(model.SettlementFrequencyModel.FixedBill.StartFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out userInputStartDate))
                    {
                        errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartFrom", ErrorMessage = ErrorLang.dateandtimecouldnotbeparsed().ToString() });
                        throw new DirtyFormDataException { };
                    }
                    model.SettlementFrequencyModel.FixedBill.StartDateAndTime = userInputStartDate;
                    cronExpression = "0 0 0 * * ?";//12 midnight
                    nextScheduledDateTimeToRun = Utilities.Util.GetNextDate(cronExpression, model.SettlementFrequencyModel.FixedBill.StartDateAndTime).Value.AddMilliseconds(-1);
                }


                SettlementRule persistModel = new SettlementRule
                {
                    Name = model.Name.Trim(),
                    AddedBy = adminUser,
                    SettlementEngineRuleIdentifier = model.SettlementEngineRuleIdentifier,
                    CronExpression = cronExpression,
                    NextScheduleDate = nextScheduledDateTimeToRun.Value,
                    JSONScheduleModel = JsonConvert.SerializeObject(model.SettlementFrequencyModel),
                    SettlementPeriodStartDate = model.SettlementFrequencyModel.FixedBill.StartDateAndTime,
                    SettlementPeriodEndDate = nextScheduledDateTimeToRun.Value
                };

                //do save for details
                List<SettlementRuleDetails> details = DoWork(model, persistModel, errors);
                //give the revenue head Id the first value, so the method computing the 
                //heirarchy will know the rule has revenue head(s) specified
                if (!model.ApplyToAllRevenueHeads)
                {
                    model.RevenueHeadId = details.First().RevenueHead.Id;
                }

                //persistModel.SettlementHierarchyLevel = GetHierarchyLevel(model);

                if (!_settlementRepo.Value.Save(persistModel)) { throw new CannotSaveRecordException(); }
                int failedInsertIndex = _settlementDetailsRepo.Value.SaveBundleUnCommitStatelessWithErrors(details);
                if (failedInsertIndex >= 0)
                {
                    //get the revenue head name
                    if (!model.ApplyToAllRevenueHeads)
                    {
                        RevenueHeadVM revenueHead = _revenueHeadRepo.GetRevenueHeadVM(model.RevenueHeadsSelected.ElementAt(failedInsertIndex));
                        errors.Add(new ErrorModel { FieldName = "SRevenueHeadId", ErrorMessage = ErrorLang.mdarevenueprovidercombinationalreadyexists(revenueHead.Name).ToString() });
                    }
                    else
                    {
                        errors.Add(new ErrorModel { FieldName = "SRevenueHeadId", ErrorMessage = ErrorLang.mdarevenueprovidercombinationalreadyexists().ToString() });
                    }
                    model.RevenueHeadIdWithError = model.RevenueHeadsSelected.ElementAt(failedInsertIndex);
                    throw new RecordAlreadyExistsException { };
                }
            }
            catch (Exception exception)
            {
                _revenueHeadRepo.RollBackAllTransactions();
                Logger.Error(exception, string.Format("Error saving settlement details {0}", JsonConvert.SerializeObject(model)));
                throw;
            }
        }


        /// <summary>
        /// Try save settlement rule to staging
        /// </summary>
        /// <param name="model"><see cref="SettlementRuleVM"/></param>
        /// <param name="adminUser"></param>
        /// <param name="errors"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <exception cref="RecordAlreadyExistsException"></exception>
        /// <exception cref="Exception"></exception>
        public void TrySaveSettlementRuleToStaging(SettlementRuleVM model, UserPartRecord adminUser, ref List<ErrorModel> errors)
        {
            try
            {
                //check for name
                if (_settlementStagingRepo.Value.Count(x => x.Name == model.Name) > 0 || _settlementRepo.Value.Count(x => x.Name == model.Name) > 0)
                {
                    Logger.Error(string.Format("A rule with the name {0} already exists", model.Name));
                    errors.Add(new ErrorModel { FieldName = "Name", ErrorMessage = string.Format("A rule with the name {0} already exists", model.Name) });
                    return;
                }

                DateTime? nextScheduledDateTimeToRun = null;
                string cronExpression = string.Empty;

                if (!model.SettlementFrequencyModel.EoDSettlement)
                {
                    model.SettlementFrequencyModel.FixedBill.YearlyBill.MonthlyBill = model.SettlementFrequencyModel.FixedBill.MonthlyBill;
                    cronExpression = _validationForSchedule.ValidateAndGetCronExpression(model.SettlementFrequencyModel.FrequencyType, model.SettlementFrequencyModel.FixedBill, model.SettlementFrequencyModel.FixedBill.StartDateAndTime);
                    //get next run date
                    nextScheduledDateTimeToRun = Utilities.Util.GetNextDate(cronExpression, model.SettlementFrequencyModel.FixedBill.StartDateAndTime);
                    if (nextScheduledDateTimeToRun == null)
                    {
                        errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartTime", ErrorMessage = ErrorLang.couldnotgetnextscheduledate().ToString() });
                        return;
                    }
                }
                else
                {
                    DateTime userInputStartDate = DateTime.Now;
                    if (!DateTime.TryParseExact(model.SettlementFrequencyModel.FixedBill.StartFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out userInputStartDate))
                    {
                        Logger.Error(ErrorLang.dateandtimecouldnotbeparsed().ToString());
                        errors.Add(new ErrorModel { FieldName = "SettlementFrequencyModel.FixedBill.StartFrom", ErrorMessage = ErrorLang.dateandtimecouldnotbeparsed().ToString() });
                        return;
                    }
                    model.SettlementFrequencyModel.FixedBill.StartDateAndTime = userInputStartDate;
                    cronExpression = "0 0 0 * * ?";//12 midnight
                    nextScheduledDateTimeToRun = Utilities.Util.GetNextDate(cronExpression, model.SettlementFrequencyModel.FixedBill.StartDateAndTime).Value.AddMilliseconds(-1);
                }

                //create SettlementRuleStaging model
                SettlementRuleStaging persistedStagingModel = new SettlementRuleStaging
                {
                    Name = model.Name.Trim(),
                    SettlementEngineRuleIdentifier = model.SettlementEngineRuleIdentifier.Trim(),
                    AddedBy = adminUser,
                    CronExpression = cronExpression,
                    SettlementPeriodStartDate = model.SettlementFrequencyModel.FixedBill.StartDateAndTime,
                    SettlementPeriodEndDate = nextScheduledDateTimeToRun.Value,
                    JSONScheduleModel = JsonConvert.SerializeObject(model.SettlementFrequencyModel),
                };

                //do work for staging details
                List<SettlementRuleDetailsStaging> detailsStaging = DoWorkForStaging(model, persistedStagingModel, errors);

                if (!_settlementStagingRepo.Value.Save(persistedStagingModel))
                {
                    Logger.Error(ErrorLang.couldnotsaverecord().ToString() + " for settlement staging");
                    errors.Add(new ErrorModel { FieldName = "Model", ErrorMessage = ErrorLang.couldnotsaverecord().ToString() });
                    return;
                }

                int failedInsertIndex = _settlementDetailsStagingRepo.Value.SaveBundleUnCommitStatelessWithErrors(detailsStaging);
                if (failedInsertIndex >= 0)
                {
                    //get the revenue head name
                    Logger.Error($"Error saving record for settlement staging at index {failedInsertIndex} value {detailsStaging.ElementAt(failedInsertIndex).RevenueHead.Id}");
                    errors.Add(new ErrorModel { FieldName = "SRevenueHeadId", ErrorMessage = ErrorLang.couldnotsaverecord().ToString() });
                    model.RevenueHeadIdWithError = detailsStaging.ElementAt(failedInsertIndex).RevenueHead.Id;
                    throw new CouldNotSaveRecord();
                }
            }
            catch (Exception exception)
            {
                _revenueHeadRepo.RollBackAllTransactions();
                Logger.Error(exception, string.Format("Error saving settlement details {0}", JsonConvert.SerializeObject(model)));
                errors.Add(new ErrorModel { FieldName = "Model", ErrorMessage = ErrorLang.genericexception().ToString() });
            }
        }


        /// <summary>
        /// do work
        /// </summary>
        /// <param name="model"></param>
        private List<SettlementRuleDetails> DoWork(SettlementRuleVM model, SettlementRule persistModel, List<ErrorModel> errors)
        {

            List<SettlementRuleDetails> settlementDetails = new List<SettlementRuleDetails> { };
            IEnumerable<MDARevenueHeadsVM> mdasAndRevenueHeads = null;
            var rhAndMdaCollection = JsonConvert.DeserializeObject<Dictionary<int, IEnumerable<int>>>(model.SelectedRhAndMdas);

            //all MDAs
            if (/*model.MDAId == 0 && */model.SMDAIds.Count == 1 && model.SMDAIds.Single() == "0")
            {
                //get all MDAs
                //here we get all MDAs and billable  revenue heads
                mdasAndRevenueHeads = _revenueHeadRepo.GetBillableRevenueHeadGroupByMDA();
                foreach (var item in mdasAndRevenueHeads)
                {
                    settlementDetails.Add(new SettlementRuleDetails
                    {
                        MDA = new MDA { Id = item.MDAId },
                        RevenueHead = new RevenueHead { Id = item.RevenueHeadID },
                        PaymentProvider = new ExternalPaymentProvider { Id = model.PaymentProviderId },
                        PaymentChannel_Id = model.PaymentChannelId,
                        SettlementRule = persistModel,
                    });
                }

                return settlementDetails;
            }
            else
            {
                //IEnumerable<int> revIds = null;

                //if (model.ApplyToAllRevenueHeads)
                //{
                //    revIds = _revenueHeadRepo.GetBillableRevenueHeadsIDsForMDA(model.MDAId);
                //}
                //else
                //{
                //    if (model.RevenueHeadsSelected.Where(sr => sr == 0).Count() > 0)
                //    {
                //        errors.Add(new ErrorModel { FieldName = "SRevenueHeadId", ErrorMessage = ErrorLang.norecord404().ToString() });
                //        errors.Add(new ErrorModel { FieldName = "SMDAId", ErrorMessage = ErrorLang.norecord404().ToString() });
                //        throw new DirtyFormDataException { };
                //    }

                //    IEnumerable<int> billablesForMDAId = _revenueHeadRepo.GetBillableRevenueHeadsIDsForMDA(model.MDAId);

                //    foreach (var item in model.RevenueHeadsSelected)
                //    {
                //        if (billablesForMDAId.FirstOrDefault(y => y == item) == 0)
                //        {
                //            errors.Add(new ErrorModel { FieldName = "SRevenueHeadId", ErrorMessage = ErrorLang.norecord404().ToString() });
                //            errors.Add(new ErrorModel { FieldName = "SMDAId", ErrorMessage = ErrorLang.norecord404().ToString() });
                //            throw new DirtyFormDataException { };
                //        }
                //    }

                //    revIds = model.RevenueHeadsSelected.Where(sr => sr > 0);
                //}

                //if (revIds == null || !revIds.Any())
                //{
                //    errors.Add(new ErrorModel { FieldName = "SRevenueHeadId", ErrorMessage = ErrorLang.norecord404().ToString() });
                //    errors.Add(new ErrorModel { FieldName = "SMDAId", ErrorMessage = ErrorLang.norecord404().ToString() });
                //    throw new DirtyFormDataException { };
                //}

                //foreach (var selectedRevenueHeadId in revIds)
                //{
                //    settlementDetails.Add(new SettlementRuleDetails
                //    {
                //        MDA = new MDA { Id = model.MDAId },
                //        RevenueHead = new RevenueHead { Id = selectedRevenueHeadId },
                //        PaymentProvider = new ExternalPaymentProvider { Id = model.PaymentProviderId },
                //        PaymentChannel_Id = model.PaymentChannelId,
                //        SettlementRule = persistModel,
                //    });
                //}
                foreach (var mda in rhAndMdaCollection)
                {
                    IEnumerable<int> billablesForMDAId = _revenueHeadRepo.GetBillableRevenueHeadsIDsForMDA(mda.Key);
                    foreach (var rh in mda.Value)
                    {
                        if (rh != 0)
                        {
                            if (billablesForMDAId.Count(rhId => rhId == rh) < 1) { throw new NoRecordFoundException(string.Format("No match for rev {0} and MDA {1} ", rh, mda.Key)); }
                            settlementDetails.Add(new SettlementRuleDetails
                            {
                                MDA = new MDA { Id = mda.Key },
                                RevenueHead = new RevenueHead { Id = rh },
                                PaymentProvider = new ExternalPaymentProvider { Id = model.PaymentProviderId },
                                PaymentChannel_Id = model.PaymentChannelId,
                                SettlementRule = persistModel,
                            });
                        }
                        else
                        {
                            errors.Add(new ErrorModel { FieldName = "SRevenueHeadId", ErrorMessage = ErrorLang.norecord404().ToString() });
                            throw new DirtyFormDataException { };
                            //settlementDetails.Add(new SettlementRuleDetails
                            //{
                            //    MDA = new MDA { Id = model.MDAId },
                            //    RevenueHead = new RevenueHead { Id = selectedRevenueHeadId },
                            //    PaymentProvider = new ExternalPaymentProvider { Id = model.PaymentProviderId },
                            //    PaymentChannel_Id = model.PaymentChannelId,
                            //    SettlementRule = persistModel,
                            //});
                        }
                    }
                }
            }
            return settlementDetails;
        }


        private List<SettlementRuleDetailsStaging> DoWorkForStaging(SettlementRuleVM model, SettlementRuleStaging persistedStagingModel, List<ErrorModel> errors)
        {
            return BuildCompleteSettlementRuleDetailsCombinations(BuildTemplateForPaymentProviderPaymentChannelCombinations(model, ref errors), model, persistedStagingModel, ref errors).ToList();
        }


        /// <summary>
        /// Do ranking of this settlement config
        /// </summary>
        /// <param name="model"></param>
        private int GetHierarchyLevel(SettlementRuleVM model)
        {
            //P-{0}-C-{1}-M-{2}-R-{0}
            if (model.MDAId > 0)
            {
                if (model.RevenueHeadId > 0)
                {
                    if (model.PaymentChannelId > 0)
                    {
                        //P-5-{>0}-{>0} eg P-5-1-5
                        return 6;
                    }
                    else
                    {
                        //P-0-{>0}-{>0}
                        return 5;
                    }
                }
                else
                {
                    if (model.PaymentChannelId > 0)
                    {
                        //P-{>0}-{>0}-0
                        return 4;
                    }
                    else
                    {
                        //P-0-{>0}-0
                        return 3;
                    }
                }
            }
            else
            {
                if (model.PaymentChannelId > 0)
                {
                    //P-{>0}-0-0
                    return 2;
                }
                else
                {
                    //P-0-0-0
                    return 1;
                }
            }
        }

        /// <summary>
        /// Builds structure that holds combinations of selected payment providers and payment channels
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private IEnumerable<SettlementRuleVerboseDetailsVM> BuildTemplateForPaymentProviderPaymentChannelCombinations(SettlementRuleVM model, ref List<ErrorModel> errors)
        {
            ICollection<SettlementRuleVerboseDetailsVM> combinationModel = new List<SettlementRuleVerboseDetailsVM>();
            foreach (var paymentProvider in model.SelectedPaymentProviders)
            {
                //validate provider
                int providerId = 0;
                if (!int.TryParse(paymentProvider, out providerId)) { errors.Add(new ErrorModel { FieldName = "SPaymentProviderId", ErrorMessage = "Selected payment provider is not valid" }); throw new DirtyFormDataException(); }
                foreach (var paymentChannel in model.SelectedPaymentChannels)
                {
                    //validate payment channel
                    if (paymentChannel == 0) { continue; }
                    combinationModel.Add(new SettlementRuleVerboseDetailsVM { PaymentProvider = providerId, PaymentChannel = paymentChannel });
                }
            }
            return combinationModel;
        }

        /// <summary>
        /// Builds structure that holds completed payment provider, payment channel, mda and revenue head line items combinations
        /// </summary>
        /// <param name="templateSettlementRuleDetailsCombinations">IEnumerable<SettlementRuleVerboseDetailsVM> template containing selected payment providers and payment channels combinations</param>
        /// <param name="model"></param>
        /// <param name="persistedStagingModel"></param>
        /// <param name="errors"></param>
        /// <returns>IEnumerable{SettlementRuleDetailsStaging}</returns>
        private IEnumerable<SettlementRuleDetailsStaging> BuildCompleteSettlementRuleDetailsCombinations(IEnumerable<SettlementRuleVerboseDetailsVM> templateSettlementRuleDetailsCombinations, SettlementRuleVM model, SettlementRuleStaging persistedStagingModel, ref List<ErrorModel> errors)
        {
            var rhAndMdaCollection = JsonConvert.DeserializeObject<Dictionary<int, IEnumerable<int>>>(model.SelectedRhAndMdas);
            ICollection<SettlementRuleDetailsStaging> completeSettlementRuleDetailsCombinations = new List<SettlementRuleDetailsStaging>();

            foreach (var ruleDetailsCombination in templateSettlementRuleDetailsCombinations)
            {
                foreach (var mda in rhAndMdaCollection)
                {
                    foreach (var rh in mda.Value)
                    {
                        if (rh == 0)
                        {
                            errors.Add(new ErrorModel { FieldName = "SRevenueHeadId", ErrorMessage = ErrorLang.norecord404().ToString() });
                            throw new DirtyFormDataException { };
                        }
                        completeSettlementRuleDetailsCombinations.Add(new SettlementRuleDetailsStaging
                        {
                            PaymentProvider = new ExternalPaymentProvider { Id = ruleDetailsCombination.PaymentProvider },
                            PaymentChannel_Id = ruleDetailsCombination.PaymentChannel,
                            MDA = new MDA { Id = mda.Key },
                            RevenueHead = new RevenueHead { Id = rh },
                            SettlementRuleStaging = persistedStagingModel,
                        });
                    }
                }
            }
            return completeSettlementRuleDetailsCombinations;
        }
    }
}