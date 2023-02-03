using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class RevenueHeadPermissionsHandler : BaseHandler, IRevenueHeadPermissionsHandler
    {
        private readonly IOrchardServices _orchardServices;
        private IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        public ILogger Logger { get; set; }
        private readonly IMDAManager<MDA> _mdaRepository;
        private readonly IRevenueHeadManager<RevenueHead> _revHeadRepo;
        private readonly IRevenueHeadPermissionManager<RevenueHeadPermission> _revHeadPermissionRepo;
        private readonly ICoreRevenueHeadPermissionService _revHeadPermissionService;

        public RevenueHeadPermissionsHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository,  IMDAManager<MDA> mdaRepository, IRevenueHeadManager<RevenueHead> revHeadRepo, IRevenueHeadPermissionManager<RevenueHeadPermission> revHeadPermissionRepo, ICoreRevenueHeadPermissionService revHeadPermissionService) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _mdaRepository = mdaRepository;
            _revHeadRepo = revHeadRepo;
            _revHeadPermissionRepo = revHeadPermissionRepo;
            _revHeadPermissionService = revHeadPermissionService;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get revenue head permission constraints VM
        /// </summary>
        /// <param name="expertSystemsId"></param>
        /// <param name="permissionId"></param>
        /// <returns>AssignRevenueHeadPermissionConstraintsVM</returns>
        public AssignRevenueHeadPermissionConstraintsVM GetRevenueHeadPermissionConstraintsVM(int expertSystemsId, int permissionId)
        {
            try
            {
                AssignRevenueHeadPermissionConstraintsVM vm = new AssignRevenueHeadPermissionConstraintsVM { MDAs = new List<MDAVM> { }, Permissions = new List<RevenueHeadPermissionVM> { } };
                IEnumerable<RevenueHeadPermissionVM> permissions = _revHeadPermissionRepo.GetRevenueHeadPermissions();
                IEnumerable<MDAVM> mdas = _revHeadRepo.GetMDAsForBillableRevenueHeads();
                IEnumerable<ExpertSystemVM> expertSystems = _settingsRepository.GetExpertSystemById(expertSystemsId);
                if(expertSystems == null) { throw new Exception("Invalid Expert System Id"); }
                IEnumerable<IGrouping<int, RevenueHeadPermissionsConstraintsVM>> constraints = null;
                if (permissionId != 0)
                {
                    constraints = _revHeadPermissionService.GetExistingConstraints(expertSystemsId, permissionId).GroupBy(x => x.MDAId);
                }
                //Check if the expert system already has constraints.
                if (constraints != null)
                {
                    List<int> constrainedMdas = new List<int>();
                    Dictionary<int, IEnumerable<int>> constrainedMdasAndRevenueHeads = new Dictionary<int, IEnumerable<int>>();
                    foreach (var constraint in constraints)
                    {
                        constrainedMdas.Add(constraint.Key);
                        constrainedMdasAndRevenueHeads.Add(constraint.Key, constraint.Select(x => x.RevenueHeadId));
                    }
                    vm.SelectedMdas = constrainedMdas;
                    vm.SelectedPermissionIdParsed = permissionId;
                    vm.SelectedRhAndMdas = JsonConvert.SerializeObject(constrainedMdasAndRevenueHeads);
                }
                else
                {
                    vm.SelectedMdas = new List<int>();
                    vm.SelectedRhAndMdas = JsonConvert.SerializeObject(new Dictionary<int, IEnumerable<int>>());
                    vm.SelectedPermissionIdParsed = permissionId;
                }
                vm.ExpertSystem = expertSystems.Single();
                vm.Permissions = permissions?.ToList();
                vm.MDAs = mdas?.ToList();

                return vm;
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Assign selected revenue heads constraints to expert system
        /// </summary>
        /// <param name="userInput"></param>
        public void AssignExpertSystemToSelectedRevenueHeads(AssignRevenueHeadPermissionConstraintsVM userInput)
        {
            try
            {
                IsAuthorized<RevenueHeadPermissionsHandler>(Permissions.AssignRevenueHeadPermissions);
                int permissionId;
                if (int.TryParse(userInput.SelectedPermissionId, out permissionId))
                {
                    if (permissionId < 1 || _revHeadPermissionRepo.Count(x => x.Id == permissionId) < 1) { throw new Exception("Revenue head permission specified is not valid."); }
                    if(_settingsRepository.Count(x => x.Id == userInput.ExpertSystem.Id) < 1) { throw new Exception("Selected expert systems doesn't exist"); }
                    if (userInput.SelectedMdas == null || userInput.SelectedRevenueHeads == null || string.IsNullOrEmpty(userInput.SelectedRhAndMdas))
                    {
                        _revHeadPermissionService.DeleteExistingExpertSystemRecords(userInput.ExpertSystem.Id); //Remove restrictions if none were selected.
                        return;
                    }
                    userInput.SelectedPermissionIdParsed = permissionId;
                    foreach (var rh in userInput.SelectedMdas)
                    {
                        if (rh < 1) { throw new Exception("MDA selected is not valid."); }
                    }
                    _revHeadPermissionService.TryAssignRevenueHeadPermissionsToExpertSystem(userInput, GetUser(_orchardServices.WorkContext.CurrentUser.Id));
                }
                else { throw new Exception("Revenue head permission not specified."); }
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Get a list of revenue heads for the mdas with the specified Id.
        /// </summary>
        /// <param name="mdaId">If the request takes too long to complete before the user initiates another this will contain multiple MDA ids otherwise it would have just one</param>
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
                    if (MDAIds != null)
                    {
                        foreach (var mdaId in MDAIds)
                        {
                            revenueHeads.AddRange(_revHeadRepo.GetRevenueHeadsPerMdaOnAccessList(mdaId, userId, false));
                        }
                    }
                    else { throw new Exception("No MDA selected."); }
                }
                return revenueHeads.GroupBy(rh => rh.MDAId).ToList();
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Gets MDAs restricted to the specified Access Type
        /// </summary>
        /// <param name="accessType"></param>
        /// <returns></returns>
        public List<MDAVM> GetMDAsForAccessType(string accessType)
        {
            try
            {
                if (!string.IsNullOrEmpty(accessType))
                {
                    var parsed = 0;
                    if(int.TryParse(accessType, out parsed))
                    {
                        return _mdaRepository.GetAccessList(_orchardServices.WorkContext.CurrentUser.Id, (CBS.Core.Models.Enums.AccessType)parsed, true).ToList();
                    }
                    else { throw new Exception("Unable to parse access type."); }
                }
                else { throw new Exception("Access type not specified."); }
            }
            catch (Exception) { throw; }
        }
    }
}