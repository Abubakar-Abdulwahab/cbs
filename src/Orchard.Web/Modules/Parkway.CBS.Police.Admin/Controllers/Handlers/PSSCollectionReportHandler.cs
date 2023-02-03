using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.CollectionReport.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSCollectionReportHandler : IPSSCollectionReportHandler
    {
        private readonly Lazy<ICoreCommand> _coreCommand;
        private readonly Lazy<IPSServiceManager<PSService>> _serviceManager;
        private readonly Lazy<IPoliceCollectionFilter> _collectionFilter;
        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;
        private readonly Lazy<IRevenueHeadManager<RevenueHead>> _revenueHeadRepository;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        public PSSCollectionReportHandler(Lazy<ICoreCommand> coreCommand, Lazy<IPSServiceManager<PSService>> serviceManager, Lazy<IPoliceCollectionFilter> collectionFilter, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, Lazy<IRevenueHeadManager<RevenueHead>> revenueHeadRepository, ICoreStateAndLGA coreStateLGAService)
        {
            _coreCommand = coreCommand;
            _serviceManager = serviceManager;
            _collectionFilter = collectionFilter;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _revenueHeadRepository = revenueHeadRepository;
            _coreStateLGAService = coreStateLGAService;
        }

        /// <summary>
        /// Get vm for requests view
        /// <para>VM that contains the list of requests</para>
        /// </summary>
        /// <returns>RequestReportVM</returns>
        public CollectionReportVM GetVMForRequestReport(PSSCollectionSearchParams searchParams)
        {
            CollectionReportVM vm = new CollectionReportVM { };

            int parsedId = 0;
            if (Int32.TryParse(searchParams.SelectedRevenueHead, out parsedId)) { searchParams.RevenueHeadId = parsedId; }
            if (Int32.TryParse(searchParams.SelectedCommand, out parsedId)) { searchParams.CommandId = parsedId; }

            bool applyAccessRestrictions = _approvalAccesRoleManager.Value.UserHasAcessTypeRole(searchParams.AdminUserId);

            dynamic recordsAndAggregate = _collectionFilter.Value.GetRequestReportViewModel(searchParams, applyAccessRestrictions);
            IEnumerable<PSSTransactionLogVM> reports = ((IEnumerable<PSSTransactionLogVM>)recordsAndAggregate.ReportRecords);
            IEnumerable<PSSRequestTypeVM> services = _serviceManager.Value.GetAllServices();
            List<StateModel> states = _coreStateLGAService.GetStates();

            return new CollectionReportVM
            {
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                InvoiceNumber = searchParams.InvoiceNumber,
                FileNumber = searchParams.FileNumber,
                PaymentRef = searchParams.PaymentRef,
                ReceiptNumber = searchParams.ReceiptNumber,
                Reports = (reports == null || !reports.Any()) ? new List<PSSTransactionLogVM> { } : reports.ToList(),
                RevenueHeads = _revenueHeadRepository.Value.GetAllRevenueHeads(),
                StateLGAs = states,
                ListLGAs = (searchParams.State != 0) ? states.Where(x => x.Id == searchParams.State).First().LGAs.ToList() : null,
                State = searchParams.State,
                LGA = searchParams.LGA,
                Commands = (searchParams.LGA != 0) ? _coreCommand.Value.GetCommandsByLGAId(searchParams.LGA) : new List<Core.HelperModels.CommandVM> { },
                SelectedRevenueHead = searchParams.SelectedRevenueHead,
                SelectedCommand = searchParams.SelectedCommand,
                TotalNumberOfIndividualRequestsReceived = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().DistinctRecordCount),
                TotalRequestRecord = (int)(((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount),
                TotalRequestAmount = ((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalAmount
            };
        }
    }
}