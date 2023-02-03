using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PoliceOfficerReport.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PoliceOfficerReportHandler : IPoliceOfficerReportHandler
    {
        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly IHandlerComposition _handlerComposition;
        private readonly Lazy<IPoliceOfficerFilter> _policeOfficerFilter;
        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;
        private readonly Lazy<IPoliceOfficerManager<PoliceOfficer>> _policeOfficerManager;

        public PoliceOfficerReportHandler(Lazy<ICoreCommand> coreCommand, ICoreStateAndLGA coreStateLGAService, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, Lazy<IPoliceOfficerFilter> policeOfficerFilter, IPoliceRankingManager<PoliceRanking> policeRankingManager, IHandlerComposition handlerComposition, Lazy<IPoliceOfficerManager<PoliceOfficer>> policeOfficerManager)
        {
            _coreCommand = coreCommand;
            _policeOfficerFilter = policeOfficerFilter;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _coreStateLGAService = coreStateLGAService;
            _policeRankingManager = policeRankingManager;
            _handlerComposition = handlerComposition;
            _policeOfficerManager = policeOfficerManager;
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
        /// Get Reports VM for Police Officer Report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PoliceOfficerReportVM GetVMForReports(PoliceOfficerSearchParams searchParams)
        {
            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedCommand, out parsedId)) { searchParams.CommandId = parsedId; }

            bool applyAccessRestrictions = _approvalAccesRoleManager.Value.UserHasAcessTypeRole(searchParams.AdminUserId);

            dynamic recordsAndAggregate = _policeOfficerFilter.Value.GetRequestReportViewModel(searchParams, applyAccessRestrictions);
            IEnumerable<PoliceOfficerVM> reports = ((IEnumerable<PoliceOfficerVM>)recordsAndAggregate.ReportRecords);

            return new PoliceOfficerReportVM
            {
                StateLGAs = _coreStateLGAService.GetStates(),
                ListLGAs = (searchParams.State != 0) ? _coreStateLGAService.GetStates().Where(x => x.Id == searchParams.State).First().LGAs.ToList() : null,
                SelectedState = searchParams.State,
                SelectedLGA = searchParams.LGA,
                Reports = (reports == null || !reports.Any()) ? new List<PoliceOfficerVM> { } : reports.ToList(),
                TotalActiveOfficersRecord = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfActiveOfficers).First().TotalRecordCount),
                IppisNumber = searchParams.IPPISNo,
                Ranks = _policeRankingManager.GetPoliceRanks().Reverse<PoliceRankingVM>().ToList(),
                SelectedRank = searchParams.Rank,
                SelectedCommand = searchParams.SelectedCommand,
                CommandId = searchParams.CommandId,
                Commands = (searchParams.LGA > 0) ? _coreCommand.Value.GetCommandsByLGAId(searchParams.LGA) : new List<CommandVM>(),
                OfficerName = searchParams.OfficerName,
                IdNumber = searchParams.IdNumber
            };
        }


        /// <summary>
        /// Get police officers of the rank with specified rankId that belong to the command with the specified commandId
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="rankId"></param>
        /// <returns></returns>
        public APIResponse GetPoliceOfficersByCommandAndRankId(int commandId, long rankId)
        {
            List<PoliceOfficerVM> officers = _policeOfficerManager.Value.GetPoliceOfficersByCommandAndRankId(commandId,rankId);
            if (officers == null || !officers.Any())
            {
                return new APIResponse { Error = true, ResponseObject = "No police officer of the specified rank was found for the selected command." };
            }

            return new APIResponse { ResponseObject = officers };
        }
    }
}