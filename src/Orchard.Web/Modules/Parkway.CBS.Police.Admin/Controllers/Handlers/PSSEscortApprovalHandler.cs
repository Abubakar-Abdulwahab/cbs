using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSEscortApprovalHandler : IPSSEscortApprovalHandler
    {
        private readonly IExternalDataOfficers _externalDataOfficers;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;
        private readonly ICommandManager<Command> _commandManager;
        private readonly IPolicerOfficerLogManager<PolicerOfficerLog> _policerOfficerLogManager;
        private readonly IEscortFormationAllocationManager<EscortFormationAllocation> _escortFormationAllocationManager;
        private readonly IProposedEscortOfficerManager<ProposedEscortOfficer> _proposedEscortOfficerManager;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly IPSSServiceTypeCustomApprovalImpl _serviceTypeCustomApprovalImpl;
        private readonly IEscortApprovalComposition _escortApprovalComposition;
        private readonly IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> _policeOfficerDeploymentLogManager;
        public ILogger Logger { get; set; }
        public PSSEscortApprovalHandler(IExternalDataOfficers externalDataOfficers, IPoliceRankingManager<PoliceRanking> policeRankingManager, ICommandManager<Command> commandManager, IPolicerOfficerLogManager<PolicerOfficerLog> policerOfficerLogManager, IEscortFormationAllocationManager<EscortFormationAllocation> escortFormationAllocationManager, IProposedEscortOfficerManager<ProposedEscortOfficer> proposedEscortOfficerManager, IPSSServiceTypeCustomApprovalImpl serviceTypeCustomApprovalImpl, Lazy<IPSSRequestManager<PSSRequest>> requestManager, IEscortApprovalComposition escortApprovalComposition, IPoliceOfficerDeploymentLogManager<PoliceOfficerDeploymentLog> policeOfficerDeploymentLogManager)
        {
            _externalDataOfficers = externalDataOfficers;
            _policeRankingManager = policeRankingManager;
            _commandManager = commandManager;
            _policerOfficerLogManager = policerOfficerLogManager;
            _escortFormationAllocationManager = escortFormationAllocationManager;
            _proposedEscortOfficerManager = proposedEscortOfficerManager;
            _serviceTypeCustomApprovalImpl = serviceTypeCustomApprovalImpl;
            _requestManager = requestManager;
            _escortApprovalComposition = escortApprovalComposition;
            _policeOfficerDeploymentLogManager = policeOfficerDeploymentLogManager;
        }


        /// <summary>
        /// Gets police officer with specified service number from HR system
        /// </summary>
        /// <param name="serviceNumber"></param>
        /// <returns></returns>
        public APIResponse GetPoliceOfficer(string serviceNumber)
        {
            try
            {
                if(_policeOfficerDeploymentLogManager.Count(x => x.PoliceOfficerLog.IdentificationNumber == serviceNumber && x.IsActive) > 0)
                {
                    return new APIResponse { Error = true, ResponseObject = PoliceErrorLang.police_officer_with_service_number_is_in_active_deployment(serviceNumber).Text };
                }


                var personnel = _externalDataOfficers.GetPoliceOfficer(new Core.ExternalSourceData.HRSystem.PersonnelRequestModel
                {
                    ServiceNumber = serviceNumber,
                    Page = "1",
                    PageSize = "1"
                });

                if (personnel.Error)
                {
                    List<PersonnelErrorResponseModel> response = JsonConvert.DeserializeObject<List<PersonnelErrorResponseModel>>(JsonConvert.SerializeObject(personnel.ResponseObject));
                    return new APIResponse { Error = true, ResponseObject = string.Join(",", response.Select(x => x.ErrorMessage)) };
                }
                else
                {
                    PersonnelResponseModel response = JsonConvert.DeserializeObject<PersonnelResponseModel>(JsonConvert.SerializeObject(personnel.ResponseObject));
                    PersonnelReportRecord personnelReportRecord = response.ReportRecords.FirstOrDefault();

                    if (personnelReportRecord == null)
                    {
                        return new APIResponse { Error = true, ResponseObject = "No records found for AP number, please try again." };
                    }

                    PoliceRankingVM rank = _policeRankingManager.GetPoliceRank(personnelReportRecord.RankCode);
                    if (rank != null)
                    {
                        string commandCode = string.Format("{0}-{1}-{2}-{3}", personnelReportRecord.CommandLevelCode, personnelReportRecord.CommandCode, personnelReportRecord.SubCommandCode, personnelReportRecord.SubSubCommandCode);

                        commandCode = commandCode.Replace("-0", "");

                        CommandVM command = _commandManager.GetCommandWithCode(commandCode);
                        if (command != null)
                        {
                            PolicerOfficerLog log = new PolicerOfficerLog
                            {
                                Name = string.Format("{0} {1}", personnelReportRecord.FirstName, personnelReportRecord.Surname).ToUpper(),
                                PhoneNumber = personnelReportRecord.PhoneNumber.Split(new char[] { ',', '/' })[0],
                                Rank = new PoliceRanking { Id = rank.Id },
                                IdentificationNumber = personnelReportRecord.ServiceNumber.ToUpper(),
                                IPPISNumber = personnelReportRecord.IPPSNumber,
                                Command = new Command { Id = command.Id },
                                Gender = personnelReportRecord.Gender,
                                AccountNumber = personnelReportRecord.AccountNumber,
                                BankCode = personnelReportRecord.BankCode
                            };

                            if (!_policerOfficerLogManager.Save(log))
                            {
                                throw new CouldNotSaveRecord("Could not save police officer log");
                            }

                            return new APIResponse { ResponseObject = JsonConvert.SerializeObject(new PoliceOfficerVM { Name = log.Name, IdNumber = log.IdentificationNumber, PoliceOfficerLogId = log.Id, RankId = rank.Id, RankName = rank.RankName, CommandName = command.Name, CommandId = command.Id, IppisNumber = log.IPPISNumber, AccountNumber = log.AccountNumber, RankCode = rank.ExternalDataCode }) };
                        }
                        Logger.Error($"Unable to map officer command with code {commandCode} for police officer with service number {serviceNumber}");
                        return new APIResponse { Error = true, ResponseObject = "Unable to map officer information" };
                    }
                    Logger.Error($"Unable to map officer rank with code {personnelReportRecord.RankCode} for police officer with service number {serviceNumber}");
                    return new APIResponse { Error = true, ResponseObject = "Unable to map officer information" };
                }

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _policerOfficerLogManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Gets formations selected to nominate officers by the AIG of the squad with the specified escort squad allocation id
        /// </summary>
        /// <param name="escortSquadAllocationId"></param>
        /// <param name="escortSquadAllocationGroup"></param>
        /// <returns></returns>
        public IEnumerable<AIGFormationVM> GetFormationsAllocatedToSquad(long escortSquadAllocationId, long escortSquadAllocationGroup)
        {
            try
            {
                return _escortFormationAllocationManager.GetFormationsAllocatedToSquad(escortSquadAllocationId, escortSquadAllocationGroup);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets proposed escort officers for a request with the specified request id allocated from the command with the specified command id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public APIResponse GetProposedEscortOfficers(long requestId, int commandId)
        {
            try
            {
                return new APIResponse { ResponseObject = _proposedEscortOfficerManager.GetProposedOfficersCollection(requestId, commandId) };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets number of officers requested from formation with the specified formation allocation id
        /// </summary>
        /// <param name="formationAllocationId"></param>
        /// <param name="allocationGroupId"></param>
        /// <returns></returns>
        public APIResponse GetNumberOfOfficersRequestedFromFormation(long formationAllocationId, long allocationGroupId)
        {
            try
            {
                return new APIResponse { ResponseObject = _escortFormationAllocationManager.GetNumberOfOfficersRequestedFromFormation(formationAllocationId, allocationGroupId) };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Performs user specific approval logic
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userId"></param>
        public EscortApprovalMessage ProcessCustomApproval(long requestId, int userId)
        {
            var approvalModel = _serviceTypeCustomApprovalImpl.EscortApproval(requestId, userId);
            if (approvalModel.CanApproveRequest)
            {
                return _escortApprovalComposition.ProcessEscortRequestApproval(requestId, userId);
            }
            return approvalModel;
        }
    }
}