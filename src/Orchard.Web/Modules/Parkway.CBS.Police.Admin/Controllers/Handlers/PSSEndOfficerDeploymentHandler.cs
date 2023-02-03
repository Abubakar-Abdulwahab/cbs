using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSEndOfficerDeploymentHandler : IPSSEndOfficerDeploymentHandler
    {
        private readonly IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> _policeOfficerDeploymentLogManger;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly IHandlerComposition _handlerComposition;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;
        private readonly IPoliceOfficerManager<PoliceOfficer> _policeOfficerManager;
        public ILogger Logger { get; set; }

        public PSSEndOfficerDeploymentHandler(IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> policeOfficerDeploymentLogManger, ICoreStateAndLGA coreStateLGAService, IHandlerComposition handlerComposition, IPoliceRankingManager<PoliceRanking> policeRankingManager, IPoliceOfficerManager<PoliceOfficer> policeOfficerManager)
        {
            _policeOfficerDeploymentLogManger = policeOfficerDeploymentLogManger;
            _coreStateLGAService = coreStateLGAService;
            _handlerComposition = handlerComposition;
            _policeRankingManager = policeRankingManager;
            _policeOfficerManager = policeOfficerManager;
            Logger = NullLogger.Instance;
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
        public EndOfficerDeploymentVM GetDeployedOfficerDetails(int deploymentId)
        {
            try
            {
                EndOfficerDeploymentVM vm = new EndOfficerDeploymentVM { DeploymentInfo = new PoliceOfficerDeploymentLogVM { } };
                vm.DeploymentInfo = _policeOfficerDeploymentLogManger.GetPoliceOfficerDeploymentLogVM(deploymentId);
                if (vm.DeploymentInfo == null) { throw new NoRecordFoundException($"Deployment Info not found for Id {deploymentId}"); }
                //Check if deployment is still running i.e if the end date of deployment has not past
                if(vm.DeploymentInfo.EndDate < DateTime.Now) { vm.CanNotEndDeployment = true; return vm; }
                vm.DeploymentLogId = deploymentId;
                return vm;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Change deployed police officer
        /// </summary>
        /// <param name="userInput"></param>
        public void EndOfficerDeployment(EndOfficerDeploymentVM userInput, int adminUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(userInput.EndReason))
                {
                    throw new DirtyFormDataException("Comment field is empty for end reason");
                }

                if (userInput.EndReason.Length < 10)
                {
                    throw new DirtyFormDataException("End reason comment requires atleast 10 characters");
                }

                PoliceOfficerDeploymentLogVM depLog = _policeOfficerDeploymentLogManger.GetPoliceOfficerDeploymentLogVM(userInput.DeploymentLogId);
                if (depLog != null)
                {
                    depLog.PoliceOfficerDeploymentLog.IsActive = false;
                    depLog.PoliceOfficerDeploymentLog.Status = (int)DeploymentStatus.Terminated;
                    depLog.PoliceOfficerDeploymentLog.DeploymentEndReason = userInput.EndReason;
                    depLog.PoliceOfficerDeploymentLog.DeploymentEndBy = new Orchard.Users.Models.UserPartRecord { Id = adminUserId };
                }
                else { throw new NoRecordFoundException($"Deployment log with Id {userInput.DeploymentLogId} was not found"); }
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ex.Message);
                _policeOfficerDeploymentLogManger.RollBackAllTransactions();
                throw;
            }
        }
    }
}