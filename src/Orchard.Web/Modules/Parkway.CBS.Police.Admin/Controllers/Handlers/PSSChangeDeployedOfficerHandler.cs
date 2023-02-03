using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSChangeDeployedOfficerHandler : IPSSChangeDeployedOfficerHandler
    {
        private readonly IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> _policeOfficerDeploymentLogManger;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly IHandlerComposition _handlerComposition;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;
        private readonly IPolicerOfficerLogManager<PolicerOfficerLog> _policeOfficerLogManager;

        public PSSChangeDeployedOfficerHandler(IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> policeOfficerDeploymentLogManger, ICoreStateAndLGA coreStateLGAService, IHandlerComposition handlerComposition, IPoliceRankingManager<PoliceRanking> policeRankingManager, IPolicerOfficerLogManager<PolicerOfficerLog> policeOfficerLogManager)
        {
            _policeOfficerDeploymentLogManger = policeOfficerDeploymentLogManger;
            _coreStateLGAService = coreStateLGAService;
            _handlerComposition = handlerComposition;
            _policeRankingManager = policeRankingManager;
            _policeOfficerLogManager = policeOfficerLogManager;
        }



        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }


        /// <summary>
        /// Get ChangeDeployedOfficerVM using specified deployment Id
        /// </summary>
        /// <param name="deploymentId"></param>
        /// <returns></returns>
        public ChangeDeployedOfficerVM GetChangeDeployedOfficerVM(int deploymentId)
        {
            try
            {
                ChangeDeployedOfficerVM vm = new ChangeDeployedOfficerVM { DeploymentInfo = new PoliceOfficerDeploymentLogVM { } };
                vm.DeploymentInfo = _policeOfficerDeploymentLogManger.GetPoliceOfficerDeploymentLogVM(deploymentId);
                if (vm.DeploymentInfo == null) { throw new Exception($"Deployment Info not found for Id {deploymentId}"); }
                //Check if deployment is still running i.e if the end date of deployment has not past
                if(vm.DeploymentInfo.EndDate < DateTime.Now) { vm.CanNotBeChanged = true; return vm; }
                vm.StateLGAs = _coreStateLGAService.GetStates();
                vm.PoliceRanks = _policeRankingManager.GetPoliceRanks().ToList();
                vm.deploymentLogId = deploymentId;
                return vm;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Change deployed police officer
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errorMessage"></param>
        public void ChangeDeployedOfficer(ChangeDeployedOfficerVM userInput, ref string errorMessage)
        {
            try
            {
                PoliceOfficerDeploymentLogVM existingOfficerLog = _policeOfficerDeploymentLogManger.GetPoliceOfficerDeploymentLogVM(userInput.deploymentLogId);
                if (existingOfficerLog != null)
                {
                    PoliceOfficerLogVM relievingOfficer = _policeOfficerLogManager.GetPoliceOfficerDetails(userInput.selectedOfficer);
                    if (relievingOfficer == null) { throw new Exception($"Officer with Id {userInput.selectedOfficer} does not exist"); }
                    if (relievingOfficer.RankId != existingOfficerLog.SelectedOfficerRank) 
                    {
                        errorMessage = $"Relieving Officer rank {relievingOfficer.RankName} does not match current officer rank {existingOfficerLog.OfficerRankName}";
                        throw new CBS.Core.Exceptions.DirtyFormDataException($"Relieving Officer rank {relievingOfficer.RankId} does not match current officer rank {existingOfficerLog.SelectedOfficerRank}");
                    }

                    PoliceOfficerDeploymentLog newOfficerLog = new PoliceOfficerDeploymentLog
                    {
                        Address = existingOfficerLog.Address,
                        CustomerName = existingOfficerLog.CustomerName,
                        Invoice = new CBS.Core.Models.Invoice { Id = existingOfficerLog.InvoiceId },
                        State = new CBS.Core.Models.StateModel { Id = existingOfficerLog.SelectedState },
                        LGA = new CBS.Core.Models.LGA { Id = existingOfficerLog.SelectedLGA },
                        Request = new PSSRequest { Id = existingOfficerLog.RequestId },
                        StartDate = DateTime.Now,
                        EndDate = existingOfficerLog.EndDate,
                        IsActive = true,
                        Status = existingOfficerLog.Status,
                        OfficerName = relievingOfficer.Name,
                        PoliceOfficerLog = new PolicerOfficerLog { Id = relievingOfficer.Id },
                        Command = new Command { Id = relievingOfficer.CommandId },
                        OfficerRank = new PoliceRanking { Id = relievingOfficer.RankId }
                    };

                    if (_policeOfficerDeploymentLogManger.Save(newOfficerLog))
                    {
                        existingOfficerLog.PoliceOfficerDeploymentLog.IsActive = false;
                        existingOfficerLog.PoliceOfficerDeploymentLog.RelievingOfficerLog = new PolicerOfficerLog { Id = relievingOfficer.Id };
                    }
                    else { throw new Exception($"Unable to change officer for deployment with Id {existingOfficerLog.Id}"); }
                }
                else { throw new Exception($"Deployment log with Id {userInput.deploymentLogId} was not found"); }
            }
            catch
            {
                _policeOfficerDeploymentLogManger.RollBackAllTransactions();
                throw;
            }
        }
    }
}