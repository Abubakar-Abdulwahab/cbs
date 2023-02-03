using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class PaymentProviderValidationConstraintHandler : BaseHandler, IPaymentProviderValidationConstraintHandler
    {
        private readonly ICorePaymentProviderValidationConstraintService _paymentConstraintService;
        private readonly IOrchardServices _orchardServices;
        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly Lazy<ICoreMDARevenueAccessRestrictionsStagingService> _coreAccessRestrictionsStagingService;


        public PaymentProviderValidationConstraintHandler(ICorePaymentProviderValidationConstraintService paymentConstraintService, IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, Lazy<ICoreMDARevenueAccessRestrictionsStagingService> coreAccessRestrictionsStagingService) 
            : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _coreAccessRestrictionsStagingService = coreAccessRestrictionsStagingService;
            Logger = NullLogger.Instance;
            _paymentConstraintService = paymentConstraintService;
        }


        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="createMDA"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            IsAuthorized<PaymentProviderValidationConstraintHandler>(permission);
        }


        /// <summary>
        /// Get VM for Assign External Payment Provider
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public AssignExternalPaymentProviderVM GetAssignExternalPaymentProviderVM(int providerId)
        {
            try
            {
                IsAuthorized<PaymentProviderValidationConstraintHandler>(Permissions.AssignExternalPaymentProvider);
                AssignExternalPaymentProviderVM vm = new AssignExternalPaymentProviderVM { SelectedMdas = new List<int> { } };
                //vm.MDAs = _revHeadRepo.GetMDAsForBillableRevenueHeads().ToList();
                vm.MDAs = _paymentConstraintService.GetMDAsForBillableRevenueHeads();
                //Check if payment provider Id is valid.
                IEnumerable<PaymentProviderVM> provider = _paymentConstraintService.GetProvider(providerId);
                
                IEnumerable<IGrouping<int,PaymentProviderValidationConstraintsVM>> constraints = _paymentConstraintService.GetExistingRestrictions(providerId).GroupBy(x => x.MDAId);
                //Check if the payment provider already has constraints.
                if(constraints != null)
                {
                    List<int> constrainedMdas = new List<int>();
                    Dictionary<int, IEnumerable<int>> constrainedMdasAndRevenueHeads = new Dictionary<int, IEnumerable<int>>();
                    foreach (var constraint in constraints)
                    {
                        constrainedMdas.Add(constraint.Key);
                        constrainedMdasAndRevenueHeads.Add(constraint.Key, constraint.Select(x => x.RevenueHeadId));
                    }
                    vm.SelectedMdas = constrainedMdas;
                    vm.SelectedPaymentProviderName = provider.FirstOrDefault().Name;
                    vm.SelectedRhAndMdas = JsonConvert.SerializeObject(constrainedMdasAndRevenueHeads);
                    vm.IsEdit = (constraints.Any()) ? true : false;
                }
               
                return vm;
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Get a list of revenue heads for the mdas with the specified Id
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns></returns>
        public dynamic GetRevenueHeadsPerMda(string mdaIds)
        {
            try
            {
                int userId = GetUser(_orchardServices.WorkContext.CurrentUser.Id).Id;
                List<RevenueHeadLite> revenueHeads = new List<RevenueHeadLite> { };
                if (!string.IsNullOrEmpty(mdaIds))
                {
                    IEnumerable<int> MDAIds = JsonConvert.DeserializeObject<IEnumerable<int>>(mdaIds);
                    if(MDAIds != null)
                    {
                        foreach(var mdaId in MDAIds)
                        {
                            revenueHeads.AddRange(_paymentConstraintService.GetRevenueHeadsPerMdaOnAccessList(mdaId, userId, false));
                        }
                    }
                    else { throw new Exception("No MDA selected."); }
                }
                return revenueHeads.GroupBy(rh => rh.MDAId).ToList();
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Assign payment provider to selected revenue heads
        /// </summary>
        /// <param name="userInput"></param>
        public void AssignPaymentProviderToSelectedRevenueHeads(AssignExternalPaymentProviderVM userInput)
        {
            try
            {
                IsAuthorized<PaymentProviderValidationConstraintHandler>(Permissions.AssignExternalPaymentProvider);
                int providerId;
                if(int.TryParse(userInput.SelectedPaymentProvider, out providerId))
                {
                    if(providerId < 1) { throw new Exception("Payment provider specified is not valid."); }
                    if (!string.IsNullOrEmpty(userInput.MDARevenueHeadAccessRestrictionsReference))
                    {
                        //if there is a reference this means that this is an edit operation and that the edited records are in the MDARevenueAccessRestrictions table 
                        //so we do a merge operation that synchronizes the table with the PaymentProviderValidationConstraints table.
                        _paymentConstraintService.UpdatePaymentProviderConstraints(providerId, userInput.MDARevenueHeadAccessRestrictionsReference);
                        return;
                    }
                    userInput.SelectedPaymentProviderParsed = providerId;
                    foreach(var rh in userInput.SelectedMdas)
                    {
                        if(rh < 1) { throw new Exception("MDA selected is not valid."); }
                    }
                    _paymentConstraintService.TryAssignPaymentProviderToRevenueHeads(userInput, GetUser(_orchardServices.WorkContext.CurrentUser.Id));
                }
                else { throw new Exception("Payment provider not specified."); }
            }
            catch (Exception) { throw; }
        }

        
        /// <summary>
        /// Updates MDA & Revenue Heads access restrictions staging table with new records
        /// </summary>
        /// <param name="additions"></param>
        /// <param name="removals"></param>
        /// <param name="providerId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse UpdateStagingData(string additions, string removals, string providerId)
        {
                var additionsMap = JsonConvert.DeserializeObject<Dictionary<int, IEnumerable<int>>>(additions);
                var removalsMap = JsonConvert.DeserializeObject<Dictionary<int, IEnumerable<int>>>(removals);
                int userId = GetUser(_orchardServices.WorkContext.CurrentUser.Id).Id;
                int providerIdPased = 0;
                if (!int.TryParse(providerId, out providerIdPased))
                {
                    throw new Exception("Unable to parse specified payment provider id");
                }

                if (!_paymentConstraintService.CheckIfPaymentProviderExists(providerIdPased)) { throw new NoRecordFoundException("Specified payment provider does not exist."); }
                // validate MDAs & RevenueHeads
                // then save
                return new APIResponse
                {
                    ResponseObject = _coreAccessRestrictionsStagingService.Value
                                        .ValidateAndSaveStagingData(additionsMap, removalsMap, providerIdPased, userId, nameof(PaymentProviderValidationConstraint))
                };
        }

    }
}