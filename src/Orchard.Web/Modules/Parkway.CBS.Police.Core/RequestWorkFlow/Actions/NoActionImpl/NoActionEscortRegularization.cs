using System;
using Orchard;
using System.Linq;
using Orchard.Logging;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.NoActionImpl.Contracts;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.StateConfig;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.NoActionImpl
{
    public class NoActionEscortRegularization : IServiceNoActionImpl
    {
        public PSSServiceTypeDefinition GetServiceType => PSSServiceTypeDefinition.EscortRegularization;


        private readonly ITypeImplComposer _compositionHandler;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsManager<PSSRegularizationUnknownOfficerRecurringInvoiceSettings> _regularizationInvoiceSettingsManager;
        private readonly IPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> _regularizationDeploymentContributionLogManager;
        private readonly IPSSRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentLog> _regularizationDeploymentLogManager;
        private readonly IPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> _proposedRegularizationUnknownPoliceOfficerDeploymentLogManager;
        private readonly IEscortAmountChartSheetManager<EscortAmountChartSheet> _escortAmountChartSheetManager;


        public NoActionEscortRegularization(IPSSRegularizationUnknownOfficerRecurringInvoiceSettingsManager<PSSRegularizationUnknownOfficerRecurringInvoiceSettings> regularizationInvoiceSettingsManager, IOrchardServices orchardServices, IPSSRegularizationUnknownPoliceOfficerDeploymentContributionLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> regularizationDeploymentContributionLogManager, IPSSRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSRegularizationUnknownPoliceOfficerDeploymentLog> regularizationDeploymentLogManager, ITypeImplComposer compositionHandler, IPSSProposedRegularizationUnknownPoliceOfficerDeploymentLogManager<PSSProposedRegularizationUnknownPoliceOfficerDeploymentLog> proposedRegularizationUnknownPoliceOfficerDeploymentLogManager, IEscortAmountChartSheetManager<EscortAmountChartSheet> escortAmountChartSheetManager)
        {
            _compositionHandler = compositionHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _regularizationInvoiceSettingsManager = regularizationInvoiceSettingsManager;
            _regularizationDeploymentContributionLogManager = regularizationDeploymentContributionLogManager;
            _regularizationDeploymentLogManager = regularizationDeploymentLogManager;
            _proposedRegularizationUnknownPoliceOfficerDeploymentLogManager = proposedRegularizationUnknownPoliceOfficerDeploymentLogManager;
            _escortAmountChartSheetManager = escortAmountChartSheetManager;
        }


        public RequestFlowVM DoServiceImplementationWorkForNoAction(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, string approvalNumber)
        {
            return DoDeployment(requestDeets, approvalNumber);
        }


        /// <summary>
        /// Here we do the deployment of the police officers 
        /// </summary>
        /// <param name="requestDeets"></param>
        public RequestFlowVM DoDeployment(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, string approvalNumber)
        {
            try
            {
                StateConfig stateConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                string deductionPercentage = stateConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PSSOfficerAllowanceDeduction.ToString()).FirstOrDefault().Value;

                if (string.IsNullOrEmpty(deductionPercentage))
                {
                    throw new Exception("Deployment allowance payment configuration was not found. Request Id " + requestDeets.ElementAt(0).Request.Id);
                }

                bool parsed = decimal.TryParse(deductionPercentage, out decimal convertedDeductionPercentage);
                if (!parsed)
                {
                    throw new Exception("Unable to convert configured deployment allowance deduction percentage. Request Id " + requestDeets.ElementAt(0).Request.Id);
                }

                EscortRegularizationSettingsDTO escortSettingDeets = _regularizationInvoiceSettingsManager.GetEscortRegularizationSettingsDetails(requestDeets.ElementAt(0).Request.Id);

                if (escortSettingDeets == null)
                {
                    throw new Exception("Escort regularization settings not found for request id " + requestDeets.ElementAt(0).Request.Id);
                }

                //get the list of officers assigned
                IEnumerable<RegularizationRequestWithoutOfficersUploadBatchItemsStagingDTO> deploymentLogs = _proposedRegularizationUnknownPoliceOfficerDeploymentLogManager.GetEscortRegularizationOfficerDeployment(requestDeets.ElementAt(0).Request.Id, requestDeets.ElementAt(0).InvoiceId);

                //save the officers
                List<PSSRegularizationUnknownPoliceOfficerDeploymentLog> deployments = new List<PSSRegularizationUnknownPoliceOfficerDeploymentLog> { };
                List<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> deploymentContributionLogs = new List<PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog> { };
                List<RequestCommand> requestCommands = new List<RequestCommand> { };

                foreach (var commandDeploymentLog in deploymentLogs)
                {
                    PSSRegularizationUnknownPoliceOfficerDeploymentLog deploymentLog = new PSSRegularizationUnknownPoliceOfficerDeploymentLog
                    {
                        GenerateRequestWithoutOfficersUploadBatchItemsStaging = new GenerateRequestWithoutOfficersUploadBatchItemsStaging { Id = commandDeploymentLog.BatchItemStagingId },
                        StartDate = commandDeploymentLog.StartDate,
                        EndDate = commandDeploymentLog.EndDate,
                        DeploymentRate = commandDeploymentLog.DeploymentRate,
                        LGA = new CBS.Core.Models.LGA { Id = escortSettingDeets.LGAId },
                        State = new CBS.Core.Models.StateModel { Id = escortSettingDeets.StateId },
                        Request = requestDeets.ElementAt(0).Request,
                        Invoice = new CBS.Core.Models.Invoice { Id = requestDeets.First().InvoiceId },
                        IsActive = true
                    };
                    deployments.Add(deploymentLog);

                    decimal deploymentAllowance = Math.Round((convertedDeductionPercentage / 100) * (commandDeploymentLog.DeploymentRate * commandDeploymentLog.NumberOfOfficers * escortSettingDeets.WeekDayNumber), MidpointRounding.ToEven);
                    PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog deploymentContributionLog = new PSSRegularizationUnknownPoliceOfficerDeploymentContributionLog
                    {
                        GenerateRequestWithoutOfficersUploadBatchItemsStaging = new GenerateRequestWithoutOfficersUploadBatchItemsStaging { Id = commandDeploymentLog.BatchItemStagingId },
                        NumberOfDays = escortSettingDeets.WeekDayNumber,
                        DeploymentAllowancePercentage = convertedDeductionPercentage,
                        DeploymentAllowanceAmount = deploymentAllowance,
                        DeploymentRate = commandDeploymentLog.DeploymentRate,
                        Request = requestDeets.ElementAt(0).Request,
                        Invoice = new CBS.Core.Models.Invoice { Id = requestDeets.First().InvoiceId },
                        IsActive = true
                    };
                    deploymentContributionLogs.Add(deploymentContributionLog);

                    for (int i = 0; i < commandDeploymentLog.NumberOfOfficers; i++)
                    {
                        requestCommands.Add(new RequestCommand { Command = new Command { Id = commandDeploymentLog.CommandId }, Request = requestDeets.ElementAt(0).Request });
                    }
                }

                //save deployment logs for this request
                if (!_regularizationDeploymentLogManager.SaveBundleUnCommit(deployments))
                {
                    throw new CouldNotSaveRecord("Could not save deployment records for request Id " + requestDeets.ElementAt(0).Request.Id);
                }

                //save deployment logs for this request
                if (!_regularizationDeploymentContributionLogManager.SaveBundleUnCommit(deploymentContributionLogs))
                {
                    throw new CouldNotSaveRecord("Could not save deployment records for request Id " + requestDeets.ElementAt(0).Request.Id);
                }

                //save associated commands for this request
                _compositionHandler.SaveCommandDetails(requestCommands);

                return new RequestFlowVM
                {
                    Message = string.Format("{0} have been logged for deployment to address: {1}. The customer {2} has been notified of this action. This request has reached it's final level of approval.", deploymentLogs.Sum(x => x.NumberOfOfficers), escortSettingDeets.Address, escortSettingDeets.CustomerName)
                };
            }
            catch (Exception)
            {
                _regularizationDeploymentLogManager.RollBackAllTransactions();
                throw;
            }
        }

    }
}