using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Services.PSSDeploymentAllowance;
using Parkway.CBS.Services.PSSDeploymentAllowance.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class EscortDeploymentAllowanceApproval : IPSSDeploymentAllowanceImpl
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Escort;
        private readonly Lazy<IPoliceofficerDeploymentAllowanceManager<PoliceofficerDeploymentAllowance>> _deploymentAllowanceManager;
        private readonly Lazy<IPSSDeploymentAllowanceSettlementEngineDetailsManager<PSSDeploymentAllowanceSettlementEngineDetails>> _settlementEngineDetailsManager;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ITypeImplComposer _compositionHandler;

        public EscortDeploymentAllowanceApproval(IOrchardServices orchardServices, Lazy<IPoliceofficerDeploymentAllowanceManager<PoliceofficerDeploymentAllowance>> deploymentAllowanceManager, ITypeImplComposer compositionHandler, Lazy<IPSSDeploymentAllowanceSettlementEngineDetailsManager<PSSDeploymentAllowanceSettlementEngineDetails>> settlementEngineDetailsManager)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _deploymentAllowanceManager = deploymentAllowanceManager;
            _compositionHandler = compositionHandler;
            _settlementEngineDetailsManager = settlementEngineDetailsManager;
        }

        /// <summary>
        /// Get the deployment allowance view details using deployment allowance request id
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <returns>EscortDeploymentRequestDetailsVM</returns>
        public EscortDeploymentRequestDetailsVM GetRequestViewDetails(long deploymentAllowanceRequestId)
        {
            EscortDeploymentRequestDetailsVM requestDeet = _deploymentAllowanceManager.Value.GetRequestViewDetails(deploymentAllowanceRequestId);
            requestDeet.ViewName = "EscortDeploymentDetails";
            return requestDeet;
        }

        /// <summary>
        /// Update the deployment allowance request status and queue the job for settlement
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <param name="requestVM"></param>
        /// <returns>RequestApprovalResponse</returns>
        public bool SaveRequestApprovalDetails(long deploymentAllowanceRequestId, dynamic requestVM)
        {
            try
            {
                PoliceofficerDeploymentAllowance depAllowance = _deploymentAllowanceManager.Value.Get(x => x.Id == deploymentAllowanceRequestId);
                depAllowance.InitiatedBy = new UserPartRecord { Id = requestVM.ApproverId };
                depAllowance.Comment = requestVM.Comment;
                depAllowance.Status = requestVM.ApprovalStatus;
                if (requestVM.ApprovalStatus == (int)PSSRequestStatus.Rejected)
                {
                    return true;
                }

                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node nodeRuleCode = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PSSAllowanceSettlementRuleCode.ToString()).FirstOrDefault();
                Node nodeCallbackURL = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PSSAllowanceCallBackURL.ToString()).FirstOrDefault();

                if (nodeRuleCode == null || nodeCallbackURL == null)
                {
                   throw new Exception("Deployment allowance settlement configuration details was not found");
                }

                List<DeploymentAllowanceSettlementItemsVM> settlementItems = new List<DeploymentAllowanceSettlementItemsVM>();
                settlementItems.Add(new DeploymentAllowanceSettlementItemsVM
                {
                    AccountName = depAllowance.PoliceOfficerLog.Name,
                    AccountNumber = depAllowance.PoliceOfficerLog.AccountNumber,
                    BankCode = depAllowance.PoliceOfficerLog.BankCode,
                    Amount = depAllowance.Amount,
                    Narration = $"{depAllowance.PoliceOfficerLog.Name} - {depAllowance.Narration}"
                });

                DeploymentAllowanceSettlementVM settlementRequestModel = new DeploymentAllowanceSettlementVM
                {
                    CallbackURL = nodeCallbackURL.Value,
                    Narration = depAllowance.Narration,
                    RuleCode = nodeRuleCode.Value,
                    PaymentType = "OTHER",
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}", nodeRuleCode.Value, DateTime.Now.ToString("yyyy-MM-ddTHH"), deploymentAllowanceRequestId),
                    Items = settlementItems
                };
                depAllowance.Status = (int)DeploymentAllowanceStatus.Waiting;
                depAllowance.SettlementReferenceNumber = settlementRequestModel.ReferenceNumber;

                string ssttlmtmodel = JsonConvert.SerializeObject(settlementRequestModel);

                var deploymentAllowanceSettlement = new PSSDeploymentAllowanceSettlementEngineDetails
                {
                    RequestReference = settlementRequestModel.ReferenceNumber,
                    PoliceofficerDeploymentAllowance = new PoliceofficerDeploymentAllowance { Id = deploymentAllowanceRequestId },
                    Amount = depAllowance.Amount,
                    SettlementEngineRequestJSON = ssttlmtmodel,
                    TimeFired = DateTime.Now
                };
                if (!_settlementEngineDetailsManager.Value.Save(deploymentAllowanceSettlement))
                {
                    throw new CouldNotSaveRecord("Could not save deployment allowance settlement details");
                };

                IPSSDeploymentAllowanceJob deploymentAllowanceJobs = new PSSDeploymentAllowanceJob();
                deploymentAllowanceJobs.QueueDeploymentAllowanceRequest(deploymentAllowanceSettlement.Id);

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("{0}", exception.Message));
                _deploymentAllowanceManager.Value.RollBackAllTransactions();
                throw;
            }
        }
    }
}