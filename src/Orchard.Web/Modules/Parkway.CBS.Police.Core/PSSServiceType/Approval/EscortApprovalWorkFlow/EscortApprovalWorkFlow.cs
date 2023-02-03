using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalWorkFlow.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalWorkFlow
{
    public class EscortApprovalWorkFlow : IEscortApprovalWorkFlow
    {
        private readonly Lazy<IEscortProcessFlowManager<EscortProcessFlow>> _escortProcessFlowManager;
        private readonly Lazy<IEscortProcessStageDefinitionManager<EscortProcessStageDefinition>> _escortProcessDefManager;
        private readonly Lazy<IEscortSquadAllocationGroupManager<EscortSquadAllocationGroup>> _escortAllocGrpManager;
        private readonly Lazy<IEscortViewRubricManager<EscortViewRubric>> _escortRubricManager;
        public ILogger Logger { get; set; }

        public EscortApprovalWorkFlow(Lazy<IEscortProcessFlowManager<EscortProcessFlow>> escortProcessFlowManager, Lazy<IEscortProcessStageDefinitionManager<EscortProcessStageDefinition>> escortProcessDefManager, Lazy<IEscortSquadAllocationGroupManager<EscortSquadAllocationGroup>> escortAllocGrpManager, Lazy<IEscortViewRubricManager<EscortViewRubric>> escortRubricManager)
        {
            _escortProcessFlowManager = escortProcessFlowManager;
            _escortProcessDefManager = escortProcessDefManager;
            _escortAllocGrpManager = escortAllocGrpManager;
            _escortRubricManager = escortRubricManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Here we need to know what role and level this admin user is on
        /// </summary>
        public List<EscortViewRubricDTO> DoProcessLevelValidation(int adminUserId, int commandTypeId, Int64 requestId)
        {
            //here we need to find the user level in the assigned  process stage definition for the user
            //what this means is that at this is that this is the level this user is allowed to work with
            List<EscortProcessFlowDTO> escortFlowList = _escortProcessFlowManager.Value.GetProcessFlowObject(adminUserId, commandTypeId);
            if (escortFlowList.Count !=  1) { throw new UserNotAuthorizedForThisActionException(); }
            //for this usecase the admin user must have only one role

            //now we have found the process level for this admin user
            //what we need to do determine if the user's views are only readonly
            //check the rubric to see if the present status of the flow 
            //we need to check the status of the request

            //but before then we need to get the status or the EscortProcessStageDefinition level the application is in
            //after getting the level the user is allowed to work with, we get the level of the escort request
            //so if the user is ahead of the level or below the level of the request we will know at this point
            EscortSquadAllocationGroupDTO grp = _escortAllocGrpManager.Value.GetProcessStage(requestId);
            if(grp == null)
            {
                Logger.Error("No group found for this escort allocation with request Id " + requestId);
                //throw new UserNotAuthorizedForThisActionException();
            }
            //now that we know the stage this request is in we need to look up the rubric to see what the views
            //can do check the rubric for this level
            //from the current level we need to check that the level of the user is allowed to view and edit

            //so here what we are doing is that we are getting a map of the level and other levels so that
            //by knowing a particular level we can get the levels that have certain permission based on the current level

            if(grp == null) { return _escortRubricManager.Value.GetPermissionRubric(escortFlowList[0].LevelGrpId); }
            return _escortRubricManager.Value.GetPermissionRubric(grp.RequestLevel.LevelGrpId, escortFlowList[0].LevelGrpId);
        }

        /// <summary>
        /// Get view permissions
        /// </summary>
        /// <param name="rubricPermissions"></param>
        /// <returns>List{EscortApprovalViewPermissions}</returns>
        public List<EscortApprovalViewPermissions> GetPermissions(List<EscortViewRubricDTO> rubricPermissions)
        {
            return rubricPermissions.Select(x => (EscortApprovalViewPermissions)x.PermissionType).ToList();
        }

    }
}