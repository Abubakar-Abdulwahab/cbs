using Orchard;
using Orchard.Logging;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DataFilters.DeploymentAllowancePaymentReport.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class DeploymentAllowancePaymentReportHandler : IDeploymentAllowancePaymentReportHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover> _serviceRequestFlowApproverManager;
        private readonly IDeploymentAllowancePaymentReportFilter _deploymentAllowancePaymentReportFilter;
        private readonly ICommandTypeManager<CommandType> _commandTypeManager;
        ILogger Logger { get; set; }

        public DeploymentAllowancePaymentReportHandler(IHandlerComposition handlerComposition, IOrchardServices orchardServices, 
                                                        IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover> serviceRequestFlowApproverManager, 
                                                        IDeploymentAllowancePaymentReportFilter deploymentAllowancePaymentReportFilter, ICommandTypeManager<CommandType> commandTypeManager)
        {
            _handlerComposition = handlerComposition;
            _orchardServices = orchardServices;
            _serviceRequestFlowApproverManager = serviceRequestFlowApproverManager;
            _deploymentAllowancePaymentReportFilter = deploymentAllowancePaymentReportFilter;
            _commandTypeManager = commandTypeManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewDeploymentAllowancePaymentReport"></param>
        public void CheckForPermission(Permission canViewDeploymentAllowancePaymentReport)
        {
            _handlerComposition.IsAuthorized(canViewDeploymentAllowancePaymentReport);
        }


        /// <summary>
        /// Gets the Deployment Allowance Payment Report VM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public DeploymentAllowancePaymentReportVM GetDeploymentAllowancePaymentReportVM(DeploymentAllowancePaymentSearchParams searchParams)
        {
            try
            {
                searchParams.ApplyRestriction = _serviceRequestFlowApproverManager.CheckIfUserIsWalletRolePlayer(searchParams.UserPartRecordId);

                dynamic recordsAndAggregate = _deploymentAllowancePaymentReportFilter.GetDeploymentAllowancePaymentReportViewModel(searchParams);
                IEnumerable<DeploymentAllowancePaymentReportItemVM> reports = (IEnumerable<DeploymentAllowancePaymentReportItemVM>)recordsAndAggregate.ReportRecords;

                return new DeploymentAllowancePaymentReportVM
                {
                    DeploymentAllowancePaymentReportItems = reports.ToList(),
                    SourceAccount = searchParams.SourceAccountName,
                    PaymentRef = searchParams.PaymentRef,
                    Status = searchParams.Status,
                    From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                    End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                    CommandTypes = _commandTypeManager.GetCommandTypes(),
                    CommandTypeId = searchParams.CommandTypeId,
                    TotalDeploymentAllowancePaymentReportRecord = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalDeploymentAllowancePaymentReportRecord).First().TotalRecordCount,
                    TotalDeploymentAllowancePaymentReportAmount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalDeploymentAllowancePaymentReportRecord).First().TotalAmount,
                };
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}