using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PoliceOfficerReport.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.POSSAP.Scheduler.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers.Handlers
{
    public class SchedulerPoliceOfficerReportHandler : IPoliceOfficerReportSchedulerHandler
    {

        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly IHandlerComposition _handlerComposition;
        private readonly Lazy<IPoliceOfficerFilter> _policeOfficerFilter;
        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;
        private readonly Lazy<IPoliceOfficerManager<PoliceOfficer>> _policeOfficerManager;
        private readonly ICoreOfficersDataFromExternalSource _corePoliceOfficerDataFromExternalSource;

        public SchedulerPoliceOfficerReportHandler(Lazy<ICoreCommand> coreCommand, ICoreStateAndLGA coreStateLGAService, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, Lazy<IPoliceOfficerFilter> policeOfficerFilter, IPoliceRankingManager<PoliceRanking> policeRankingManager, IHandlerComposition handlerComposition, Lazy<IPoliceOfficerManager<PoliceOfficer>> policeOfficerManager, ICoreOfficersDataFromExternalSource corePoliceOfficerDataFromExternalSource)
        {
            _coreCommand = coreCommand;
            _policeOfficerFilter = policeOfficerFilter;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _coreStateLGAService = coreStateLGAService;
            _policeRankingManager = policeRankingManager;
            _handlerComposition = handlerComposition;
            _policeOfficerManager = policeOfficerManager;
            _corePoliceOfficerDataFromExternalSource = corePoliceOfficerDataFromExternalSource;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="CanViewOfficersSchedule"></param>
        public void CheckForPermission(Permission CanViewOfficersSchedule)
        {
            _handlerComposition.IsAuthorized(CanViewOfficersSchedule);
        }



        private void ValidateSearchParams(PoliceOfficerSearchParams searchParams)
        {
            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedCommand, out parsedId)) { searchParams.CommandId = parsedId; }
        }


        /// <summary>
        /// Check to see of the request needs to be processed by an
        /// external system.
        /// </summary>
        /// <returns></returns>
        public APIResponse GetReportFromExternalSystem(PoliceOfficerSearchParams searchParams)
        {
            return new APIResponse { ResponseObject = GenerateRequestIdentifierString(searchParams) };
        }


        /// <summary>
        /// When the request identifier has been generated 
        /// we need to check if the search param
        /// </summary>
        /// <returns>APIResponse</returns>
        public APIResponse CheckIfSearchConstraintsExists(PoliceOfficerSearchParams searchParams, string requestIdentifier)
        {
            if(CheckForRequestMismatch(searchParams, requestIdentifier)) { return new APIResponse { Error = true, ResponseObject = "Request mismatch" }; }
            //now that we have confirmed that the search params are the same
            //lets check if the page chunk exists
            return new APIResponse { ResponseObject = new { ChunkExists = _corePoliceOfficerDataFromExternalSource.CheckIfChunkExists(requestIdentifier, searchParams.Take, searchParams.Skip) } };
        }


        /// <summary>
        /// Call external source for data
        /// </summary>
        /// <returns></returns>
        public APIResponse CallExternalSourceForData(PoliceOfficerSearchParams searchParams, string requestIdentifier)
        {
            if(CheckForRequestMismatch(searchParams, requestIdentifier)) { return new APIResponse { Error = true, ResponseObject = "Request mismatch" }; }
            return new APIResponse { ResponseObject = _corePoliceOfficerDataFromExternalSource.GetOfficersDataFromExternalSource(searchParams, requestIdentifier) };
        }


        private bool CheckForRequestMismatch(PoliceOfficerSearchParams searchParams, string requestIdentifier)
        {
            return GenerateRequestIdentifierString(searchParams) != requestIdentifier;

        }


        /// <summary>
        /// Get the string value that identifies the search
        /// parameters
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>string</returns>
        private string GenerateRequestIdentifierString(PoliceOfficerSearchParams searchParams)
        {
            return Util.OneWaySHA512Hash($"{searchParams.SelectedCommand}-{searchParams.State}-{searchParams.LGA}-{searchParams.IPPISNo}-{searchParams.IdNumber}-{searchParams.Rank}-{searchParams.OfficerName}", AppSettingsConfigurations.EncryptionSecret);
        }


        public PoliceOfficerReportVM GetVMForReports(PoliceOfficerSearchParams searchParams)
        {
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
                IdNumber = searchParams.IdNumber,
                searchParametersToken = Util.LetsEncrypt(JsonConvert.SerializeObject(searchParams), AppSettingsConfigurations.EncryptionSecret)
            };
        }
    }
}