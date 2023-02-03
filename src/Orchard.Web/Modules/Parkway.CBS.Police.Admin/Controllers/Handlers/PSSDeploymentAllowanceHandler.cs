using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.OfficerDeploymentAllowance.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSDeploymentAllowanceHandler : IPSSDeploymentAllowanceHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOfficerDeploymentAllowanceFilter _officerDeploymentAllowanceFilter;
        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;
        private readonly Lazy<IPSSDeploymentAllowanceImpl> _deploymentAllowanceApprovalImpl;
        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly ICoreStateAndLGA _coreStateLGAService;

        public PSSDeploymentAllowanceHandler(IHandlerComposition handlerComposition, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, IOfficerDeploymentAllowanceFilter officerDeploymentAllowanceFilter, IPoliceRankingManager<PoliceRanking> policeRankingManager, Lazy<IPSSDeploymentAllowanceImpl> deploymentAllowanceApprovalImpl, Lazy<ICoreCommand> coreCommand, ICoreStateAndLGA coreStateLGAService)
        {
            _handlerComposition = handlerComposition;
            _officerDeploymentAllowanceFilter = officerDeploymentAllowanceFilter;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _policeRankingManager = policeRankingManager;
            _deploymentAllowanceApprovalImpl = deploymentAllowanceApprovalImpl;
            _coreCommand = coreCommand;
            _coreStateLGAService = coreStateLGAService;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        public void CheckForPermission(Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }

        /// <summary>
        /// Get the deployment allowance request details using deployment allowance request id
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <returns>EscortDeploymentRequestDetailsVM</returns>
        public EscortDeploymentRequestDetailsVM GetDeploymentAllowanceRequestDetails(long deploymentAllowanceRequestId)
        {
            return _deploymentAllowanceApprovalImpl.Value.GetRequestViewDetails(deploymentAllowanceRequestId);
        }

        /// <summary>
        /// Get view model for deployment allowance requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>DeploymentAllowanceReportVM</returns>
        public DeploymentAllowanceReportVM GetVMForReports(OfficerDeploymentAllowanceSearchParams searchParams)
        {
            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedCommand, out parsedId)) { searchParams.CommandId = parsedId; }

            searchParams.ApprovalAccessRoleUserId = _approvalAccesRoleManager.Value.GetAccessRoleUserId(searchParams.AdminUserId);
            bool applyAccessRestrictions = searchParams.ApprovalAccessRoleUserId > 0 ? true : false; ;

            dynamic recordsAndAggregate = _officerDeploymentAllowanceFilter.GetRequestReportViewModel(searchParams, applyAccessRestrictions);
            IEnumerable<PoliceOfficerDeploymentAllowanceVM> reports = ((IEnumerable<PoliceOfficerDeploymentAllowanceVM>)recordsAndAggregate.ReportRecords);
            List<CBS.Core.Models.StateModel> states = _coreStateLGAService.GetStates();
            return new DeploymentAllowanceReportVM
            {
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                AccountNumber = searchParams.AccountNumber,
                FileNumber = searchParams.FileNumber,
                IPPISNo = searchParams.IPPISNumber,
                APNumber = searchParams.APNumber,
                Rank = searchParams.RankId,
                InvoiceNumber = searchParams.InvoiceNumber,
                Ranks = _policeRankingManager.GetPoliceRanks().Reverse<PoliceRankingVM>().ToList(),
                ReportRecords = (reports == null || !reports.Any()) ? new List<PoliceOfficerDeploymentAllowanceVM> { } : reports.ToList(),
                TotalNumberOfDeploymentAllowances = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalRecordCount).First().TotalRecordCount),
                TotalAmountOfDeploymentAllowances = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalAllowanceAmount).First().TotalAmount,
                Status = searchParams.RequestStatus,
                StateLGAs = states,
                ListLGAs = (searchParams.State != 0) ? states.Where(x => x.Id == searchParams.State).First().LGAs.ToList() : null,
                State = searchParams.State,
                LGA = searchParams.LGA,
                Commands = (searchParams.LGA != 0) ? _coreCommand.Value.GetCommandsByLGAId(searchParams.LGA) : null,
                SelectedCommand = searchParams.SelectedCommand,
                CommandId = searchParams.CommandId
            };
        }

        /// <summary>
        /// Save approval details
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <param name="sRequestFormDump"></param>
        /// <returns>bool</returns>
        public bool SaveRequestApprovalDetails(long deploymentAllowanceRequestId, dynamic sRequestFormDump)
        {
            return _deploymentAllowanceApprovalImpl.Value.SaveRequestApprovalDetails(deploymentAllowanceRequestId, sRequestFormDump);
        }

    }
}