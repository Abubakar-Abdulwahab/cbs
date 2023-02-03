using Orchard.Logging;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;
using Newtonsoft.Json;
using System.Linq;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using System;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.Contracts;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CorePoliceOfficerService : ICorePoliceOfficerService
    {
        private readonly IExternalDataOfficers _externalDataOfficers;
        private readonly IPoliceRankingManager<PoliceRanking> _policeRankingManager;
        private readonly ICommandManager<Command> _commandManager;
        private readonly IPolicerOfficerLogManager<PolicerOfficerLog> _policerOfficerLogManager;
        public ILogger Logger { get; set; }

        public CorePoliceOfficerService(IExternalDataOfficers externalDataOfficers, IPoliceRankingManager<PoliceRanking> policeRankingManager, ICommandManager<Command> commandManager, IPolicerOfficerLogManager<PolicerOfficerLog> policerOfficerLogManager)
        {
            _externalDataOfficers = externalDataOfficers;
            _policeRankingManager = policeRankingManager;
            _commandManager = commandManager;
            _policerOfficerLogManager = policerOfficerLogManager;
            Logger = NullLogger.Instance;
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
                var personnel = _externalDataOfficers.GetPoliceOfficer(new ExternalSourceData.HRSystem.PersonnelRequestModel
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
                                PhoneNumber = personnelReportRecord.PhoneNumber.Split(new char[] { ',' })[0],
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

                            return new APIResponse { ResponseObject = JsonConvert.SerializeObject(new PoliceOfficerVM { Name = log.Name, IdNumber = log.IdentificationNumber, PoliceOfficerLogId = log.Id, RankId = rank.Id, RankName = rank.RankName, CommandName = command.Name, CommandId = command.Id, IppisNumber = log.IPPISNumber, AccountNumber = log.AccountNumber }) };
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
    }
}