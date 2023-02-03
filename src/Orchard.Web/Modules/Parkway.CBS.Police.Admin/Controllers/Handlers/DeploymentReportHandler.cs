using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Admin.VM;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Core.DataFilters.OfficerDeployment.Contracts;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using System.Linq;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class DeploymentReportHandler : IDeploymentReportHandler
    {
        private readonly IHandlerComposition _handlerComposition;

        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly Lazy<IPSServiceManager<PSService>> _serviceManager;
        private readonly IOfficerDeploymentFilter _officerDeploymentFilter;
        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;
        private readonly Lazy<IRevenueHeadManager<RevenueHead>> _revenueHeadRepository;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;


        public DeploymentReportHandler(ICoreStateAndLGA coreStateLGAService, IHandlerComposition handlerComposition, Lazy<ICoreCommand> coreCommand, Lazy<IPSServiceManager<PSService>> serviceManager, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, Lazy<IRevenueHeadManager<RevenueHead>> revenueHeadRepository, IPoliceRankingManager<PoliceRanking> policeRankingManager, IOfficerDeploymentFilter officerDeploymentFilter)
        {
            _coreCommand = coreCommand;
            _serviceManager = serviceManager;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _revenueHeadRepository = revenueHeadRepository;
            _handlerComposition = handlerComposition;
            _coreStateLGAService = coreStateLGAService;
            _policeRankingManager = policeRankingManager;
            _officerDeploymentFilter = officerDeploymentFilter;
        }



        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        public void CheckForPermission(Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }



        public DeploymentReportVM GetVMForReports(OfficerDeploymentSearchParams searchParams)
        {
            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedCommand, out parsedId)) { searchParams.CommandId = parsedId; }

            searchParams.ApprovalAccessRoleUserId = _approvalAccesRoleManager.Value.GetAccessRoleUserId(searchParams.AdminUserId);
            bool applyAccessRestrictions = searchParams.ApprovalAccessRoleUserId > 0 ? true : false; ;

            dynamic recordsAndAggregate = _officerDeploymentFilter.GetRequestReportViewModel(searchParams, applyAccessRestrictions);
            IEnumerable<PoliceOfficerDeploymentVM> reports = ((IEnumerable<PoliceOfficerDeploymentVM>)recordsAndAggregate.ReportRecords);

            return new DeploymentReportVM
            {
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                StateLGAs = _coreStateLGAService.GetStates(),
                ListLGAs = (searchParams.State != 0) ? _coreStateLGAService.GetStates().Where(x => x.Id == searchParams.State).First().LGAs.ToList() : null,
                State = searchParams.State,
                LGA = searchParams.LGA,
                Commands = (searchParams.LGA != 0) ? _coreCommand.Value.GetCommandsByLGAId(searchParams.LGA) : null,
                SelectedCommand = searchParams.SelectedCommand,
                CommandId = searchParams.CommandId,
                CustomerName = searchParams.CustomerName,
                RequestRef = searchParams.RequestRef,
                IPPISNo = searchParams.IPPISNo,
                APNumber = searchParams.APNumber,
                Rank = searchParams.Rank,
                OfficerName = searchParams.OfficerName,
                Ranks = _policeRankingManager.GetPoliceRanks().Reverse<PoliceRankingVM>().ToList(),
                ReportRecords = (reports == null || !reports.Any()) ? new List<PoliceOfficerDeploymentVM> { } : reports.ToList(),
                TotalNumberOfDeployments = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfDeployments).First().TotalRecordCount),
                TotalNumberOfActiveDeployments = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfActiveDeployments).First().TotalRecordCount),
                TotalNumberOfOfficersInActiveDeployments = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfOfficersInActiveDeployments).First().TotalRecordCount),
                InvoiceNumber = searchParams.InvoiceNumber,
            };
        }


    }
}