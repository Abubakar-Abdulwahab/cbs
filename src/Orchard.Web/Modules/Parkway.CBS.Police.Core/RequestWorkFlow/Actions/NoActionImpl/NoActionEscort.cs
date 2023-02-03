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
using System.Dynamic;
using Newtonsoft.Json;
using Parkway.DataExporter.Implementations.Util;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.NoActionImpl
{
    public class NoActionEscort : IServiceNoActionImpl
    {
        public PSSServiceTypeDefinition GetServiceType => PSSServiceTypeDefinition.Escort;


        private readonly IPSSEscortDetailsManager<PSSEscortDetails> _escortManager;
        private readonly ITypeImplComposer _compositionHandler;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> _poDeploymentLog;
        private readonly IProposedEscortOfficerManager<ProposedEscortOfficer> _proposedEscortRepo;
        private readonly IPoliceofficerDeploymentAllowanceManager<PoliceofficerDeploymentAllowance> _deploymentAllowanceManager;
        private readonly IEscortFormationOfficerManager<EscortFormationOfficer> _escortFormationOfficerManager;
        private readonly IPoliceofficerDeploymentAllowanceTrackerManager<PoliceofficerDeploymentAllowanceTracker> _deploymentAllowanceTrackerManager;
        private readonly IPSSDispatchNoteManager<PSSDispatchNote> _pssDispatchNoteManager;
        private readonly IRequestCommandManager<RequestCommand> _requestCommandManager;

        public NoActionEscort(IPSSEscortDetailsManager<PSSEscortDetails> escortManager, ITypeImplComposer compositionHandler, IOrchardServices orchardServices, IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> poDeploymentLog, IProposedEscortOfficerManager<ProposedEscortOfficer> proposedEscortRepo, IPoliceofficerDeploymentAllowanceManager<PoliceofficerDeploymentAllowance> deploymentAllowanceManager, IEscortFormationOfficerManager<EscortFormationOfficer> escortFormationOfficerManager, IPoliceofficerDeploymentAllowanceTrackerManager<PoliceofficerDeploymentAllowanceTracker> deploymentAllowanceTrackerManager, IPSSDispatchNoteManager<PSSDispatchNote> pssDispatchNoteManager, IRequestCommandManager<RequestCommand> requestCommandManager)
        {
            _escortManager = escortManager;
            _compositionHandler = compositionHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _poDeploymentLog = poDeploymentLog;
            _proposedEscortRepo = proposedEscortRepo;
            _deploymentAllowanceManager = deploymentAllowanceManager;
            _escortFormationOfficerManager = escortFormationOfficerManager;
            _deploymentAllowanceTrackerManager = deploymentAllowanceTrackerManager;
            _pssDispatchNoteManager = pssDispatchNoteManager;
            _requestCommandManager = requestCommandManager;
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
                //get escort deets
                EscortDetailsDTO escortDeets = _escortManager.GetEscortDetails(requestDeets.ElementAt(0).Request.Id);

                if (!escortDeets.OfficersHaveBeenAssigned)
                {
                    throw new DirtyFormDataException("Escort has not been confirmed " + requestDeets.ElementAt(0).Request.Id);
                }
                //get the list of officers assigned
                IEnumerable<ProposedEscortOffficerVM> listOfProposedOfficers = _escortFormationOfficerManager.GetEscortOfficers(requestDeets.ElementAt(0).Request.Id);
                //save the officers
                List<PoliceOfficerDeploymentLog> deployments = new List<PoliceOfficerDeploymentLog> { };

                foreach (var proposedOfficer in listOfProposedOfficers)
                {
                    PoliceOfficerDeploymentLog deploymentLog = new PoliceOfficerDeploymentLog
                    {
                        PoliceOfficerLog = new PolicerOfficerLog { Id = proposedOfficer.PoliceOfficerLogId },
                        OfficerRank = new PoliceRanking { Id = proposedOfficer.OfficerRankId },
                        Address = escortDeets.Address,
                        StartDate = escortDeets.StartDate,
                        EndDate = escortDeets.EndDate,
                        Command = new Command { Id = proposedOfficer.OfficerCommandId },
                        CustomerName = escortDeets.CustomerName,
                        LGA = new CBS.Core.Models.LGA { Id = escortDeets.LGAId },
                        State = new CBS.Core.Models.StateModel { Id = escortDeets.StateId },
                        Request = requestDeets.ElementAt(0).Request,
                        OfficerName = proposedOfficer.OfficerName,
                        DeploymentRate = proposedOfficer.DeploymentRate,
                        Invoice = new CBS.Core.Models.Invoice { Id = requestDeets.First().InvoiceId },
                        Status = (int)DeploymentStatus.Pending
                    };
                    deployments.Add(deploymentLog);
                }

                if (!_poDeploymentLog.SaveBundleUnCommit(deployments))
                {
                    throw new CouldNotSaveRecord("Could not save deployment records for request Id " + requestDeets.ElementAt(0).Request.Id);
                }
                //save associated commands to this request
                _compositionHandler.SaveCommandDetails(deployments.Select(x => new RequestCommand { Command = x.Command, Request = x.Request }).ToList());

                //create and save dispatch note
                var template = TemplateUtil.RazorTemplateFor("DispatchNote", _orchardServices.WorkContext.CurrentSite.SiteName);

                _pssDispatchNoteManager.Save(new PSSDispatchNote
                {
                    ApplicantName = escortDeets.CustomerName,
                    ApprovalNumber = approvalNumber,
                    FileRefNumber = escortDeets.FileRefNumber,
                    OriginStateName = escortDeets.OriginStateName,
                    OriginLGAName = escortDeets.OriginLGAName,
                    OriginAddress = escortDeets.OriginAddress,
                    ServiceDeliveryLGAName = escortDeets.LGAName,
                    ServiceDeliveryStateName = escortDeets.StateName,
                    ServiceDeliveryAddress = escortDeets.Address,
                    StartDate = escortDeets.StartDate,
                    EndDate = escortDeets.EndDate,
                    ServicingCommands = JsonConvert.SerializeObject(listOfProposedOfficers.GroupBy(x => x.OfficerCommandId).Select( x => new CommandVM { Name = x.FirstOrDefault().OfficerCommandName, Address = x.FirstOrDefault().OfficerCommandAddress, StateName = x.FirstOrDefault().OfficerCommandStateName, LGAName = x.FirstOrDefault().OfficerCommandLGAName } )),
                    PoliceOfficers = JsonConvert.SerializeObject(listOfProposedOfficers.Select(x => new PoliceOfficerLogVM { Name = x.OfficerName, CommandName = x.OfficerCommandName, IdentificationNumber = x.OfficerIdentificationNumber, RankName = x.OfficerRankName })),
                    DispatchNoteTemplate = JsonConvert.SerializeObject(new CertificateTemplateVM { Template = template, TemplateName = "DispatchNote" }),
                });

                //Do deployment allowance processing
                DoDeploymentAllowance(requestDeets, listOfProposedOfficers, escortDeets);

                return new RequestFlowVM
                {
                    Message = string.Format("{0} have been logged for deployment to address: {1}. The customer {2} has been notified of this action. This request has reached it's final level of approval.", listOfProposedOfficers.Count(), escortDeets.Address, escortDeets.CustomerName)
                };
            }
            catch (Exception)
            {
                _poDeploymentLog.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Here we do the deployment allowance for the assigned police officers 
        /// </summary>
        /// <param name="requestDeets"></param>
        public void DoDeploymentAllowance(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, IEnumerable<ProposedEscortOffficerVM> listOfProposedOfficers, EscortDetailsDTO escortDeets)
        {
            try
            {
                StateConfig stateConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                string deductionPercentage = stateConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PSSOfficerAllowanceDeduction.ToString()).FirstOrDefault().Value;
                string initialPaymentPercentage = stateConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PSSOfficerAllowanceMobilizationFee.ToString()).FirstOrDefault().Value;

                if (string.IsNullOrEmpty(deductionPercentage) || string.IsNullOrEmpty(initialPaymentPercentage))
                {
                    throw new Exception("Allowance payment configuration was not found. Request Id " + requestDeets.ElementAt(0).Request.Id);
                }

                bool parsed = decimal.TryParse(deductionPercentage, out decimal convertedDeductionPercentage);
                if (!parsed)
                {
                    throw new Exception("Unable to convert configured allowance deduction percentage. Request Id " + requestDeets.ElementAt(0).Request.Id);
                }

                //If the duration is less than a month, set the settlement completion to true
                //In this case the whole fund will be settled at the starting of the request. So the next settlement date is starting date.
                //escortDeets.StartDate.AddMonths(1).AddDays(-1) is a month from starting date, then remove a day to arrive at the end date
                //e.g 01-01-2022, adding a month will give us 01-02-2022, then removing a day will give us 31-01-2022
                DateTime monthFromStartDate = escortDeets.StartDate.AddMonths(1).AddDays(-1);
                PoliceofficerDeploymentAllowanceTracker deploymentAllowanceTracker = new PoliceofficerDeploymentAllowanceTracker
                {
                    Request = requestDeets.ElementAt(0).Request,
                    Invoice = new CBS.Core.Models.Invoice { Id = requestDeets.First().InvoiceId },
                    IsSettlementCompleted = escortDeets.EndDate < monthFromStartDate ? true : false,
                    NumberOfSettlementDone = 1, //This is the first settlement to the officer(s)
                    NextSettlementDate = escortDeets.EndDate < monthFromStartDate ? escortDeets.StartDate : monthFromStartDate,
                    SettlementCycleStartDate = escortDeets.StartDate,
                    SettlementCycleEndDate = escortDeets.EndDate < monthFromStartDate ? escortDeets.EndDate : monthFromStartDate,
                    EscortDetails = new PSSEscortDetails { Id = escortDeets.Id }
                };

                if (!_deploymentAllowanceTrackerManager.Save(deploymentAllowanceTracker))
                {
                    throw new CouldNotSaveRecord("Could not save deployment allowance tracker for request Id " + requestDeets.ElementAt(0).Request.Id);
                }

                List<PoliceofficerDeploymentAllowance> deploymentAllowances = null;
                if (escortDeets.EndDate < monthFromStartDate)
                {
                    deploymentAllowances = ProcessLessThanAMonthDeploymentAllowance(requestDeets, listOfProposedOfficers, escortDeets, convertedDeductionPercentage);
                }
                else
                {
                    parsed = decimal.TryParse(initialPaymentPercentage, out decimal convertedInitialPaymentPercentage);
                    if (!parsed)
                    {
                        throw new Exception("Unable to convert configured allowance initial payment percentage. Request Id " + requestDeets.ElementAt(0).Request.Id);
                    }
                    deploymentAllowances = ProcessMoreThanAMonthDeploymentAllowance(requestDeets, listOfProposedOfficers, escortDeets, convertedDeductionPercentage, convertedInitialPaymentPercentage);
                }

                if(deploymentAllowances == null)
                {
                    throw new Exception("Deployment allowance to be settled not found. Request Id " + requestDeets.ElementAt(0).Request.Id);
                }

                if (!_deploymentAllowanceManager.SaveBundleUnCommit(deploymentAllowances))
                {
                    throw new CouldNotSaveRecord("Could not save deployment allowance records for request Id " + requestDeets.ElementAt(0).Request.Id);
                }
            }
            catch (Exception)
            {
                _deploymentAllowanceManager.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Process less than a month deployment allowance to the deployed officer(s)
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="listOfProposedOfficers"></param>
        /// <param name="escortDeets"></param>
        /// <param name="operativeDirectCostPercentage"></param>
        /// <returns>List<PoliceofficerDeploymentAllowance></returns>
        private List<PoliceofficerDeploymentAllowance> ProcessLessThanAMonthDeploymentAllowance(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, IEnumerable<ProposedEscortOffficerVM> listOfProposedOfficers, EscortDetailsDTO escortDeets, decimal operativeDirectCostPercentage)
        {
            List<PoliceofficerDeploymentAllowance> deploymentAllowances = new List<PoliceofficerDeploymentAllowance> { };
            decimal initialPaymentPercentage = 100; //100% of the money will be paid to the officer(s) since the request is less than a month

            foreach (var proposedOfficer in listOfProposedOfficers)
            {
                int numberOfDays = (escortDeets.EndDate - escortDeets.StartDate).Days + 1;
                decimal invoiceContributedAmount = Math.Round(proposedOfficer.DeploymentRate * numberOfDays, 2);

                string narration = $"{initialPaymentPercentage}% fee for {proposedOfficer.OfficerName} for duration {escortDeets.StartDate.ToString("dd/MM/yyyy")} to {escortDeets.EndDate.ToString("dd/MM/yyyy")}";
                Logger.Information($"Deployment allowance processing ::: {narration}. Number of days::{numberOfDays}");

                PoliceofficerDeploymentAllowance deploymentLog = new PoliceofficerDeploymentAllowance
                {
                    Status = (int)DeploymentAllowanceStatus.PendingApproval,
                    PoliceOfficerLog = new PolicerOfficerLog { Id = proposedOfficer.PoliceOfficerLogId },
                    Amount = _compositionHandler.ComputeAllowanceFee(invoiceContributedAmount, operativeDirectCostPercentage, initialPaymentPercentage),
                    ContributedAmount = invoiceContributedAmount,
                    Narration = narration,
                    Request = requestDeets.ElementAt(0).Request,
                    Invoice = new CBS.Core.Models.Invoice { Id = requestDeets.First().InvoiceId },
                    PaymentStage = (int)PSSAllowancePaymentStage.OneOffFee,
                    Command = new Command { Id = proposedOfficer.OfficerCommandId },
                    EscortDetails = new PSSEscortDetails { Id = escortDeets.Id }
                };
                deploymentAllowances.Add(deploymentLog);
            }
            return deploymentAllowances;
        }

        /// <summary>
        /// Process more than a month deployment allowance to the deployed officer(s)
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="listOfProposedOfficers"></param>
        /// <param name="escortDeets"></param>
        /// <param name="operativeDirectCostPercentage"></param>
        /// <param name="mobilizationPaymentPercentage"></param>
        /// <returns>List<PoliceofficerDeploymentAllowance></returns>
        private List<PoliceofficerDeploymentAllowance> ProcessMoreThanAMonthDeploymentAllowance(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, IEnumerable<ProposedEscortOffficerVM> listOfProposedOfficers, EscortDetailsDTO escortDeets, decimal operativeDirectCostPercentage, decimal mobilizationPaymentPercentage)
        {
            List<PoliceofficerDeploymentAllowance> deploymentAllowances = new List<PoliceofficerDeploymentAllowance> { };

            foreach (var proposedOfficer in listOfProposedOfficers)
            {
                //escortDeets.StartDate.AddMonths(1).AddDays(-1) is a month from starting date, then remove a day to arrive at the end date
                //e.g 01-01-2022, adding a month will give us 01-02-2022, then removing a day will give us 31-01-2022
                int numberOfDays = (escortDeets.StartDate.AddMonths(1).AddDays(-1) - escortDeets.StartDate).Days + 1;
                decimal invoiceContributedAmount = Math.Round(proposedOfficer.DeploymentRate * numberOfDays, 2);

                string narration = $"{mobilizationPaymentPercentage}% mobilization fee for {proposedOfficer.OfficerName} for duration {escortDeets.StartDate.ToString("dd/MM/yyyy")} to {escortDeets.StartDate.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy")}";

                Logger.Information($"Deployment allowance processing ::: {narration}. Number of days::{numberOfDays}");

                PoliceofficerDeploymentAllowance deploymentLog = new PoliceofficerDeploymentAllowance
                {
                    Status = (int)DeploymentAllowanceStatus.PendingApproval,
                    PoliceOfficerLog = new PolicerOfficerLog { Id = proposedOfficer.PoliceOfficerLogId },
                    Amount = _compositionHandler.ComputeAllowanceFee(invoiceContributedAmount, operativeDirectCostPercentage, mobilizationPaymentPercentage),
                    ContributedAmount = invoiceContributedAmount,
                    Narration = narration,
                    Request = requestDeets.ElementAt(0).Request,
                    Invoice = new CBS.Core.Models.Invoice { Id = requestDeets.First().InvoiceId },
                    PaymentStage = (int)PSSAllowancePaymentStage.MobilizationFee,
                    Command = new Command { Id = proposedOfficer.OfficerCommandId },
                    EscortDetails = new PSSEscortDetails { Id = escortDeets.Id }
                };
                deploymentAllowances.Add(deploymentLog);
            }
            return deploymentAllowances;
        }
    }
}