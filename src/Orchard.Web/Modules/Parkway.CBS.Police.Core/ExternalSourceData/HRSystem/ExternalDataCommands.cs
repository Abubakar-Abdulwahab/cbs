using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.RemoteClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem
{
    public class ExternalDataCommands : IExternalDataCommands
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ICommandExternalDataStagingManager<CommandExternalDataStaging> _commandExternalDataStagingManager;

        public ExternalDataCommands(IOrchardServices orchardServices, ICommandExternalDataStagingManager<CommandExternalDataStaging> commandExternalDataStagingManager)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _commandExternalDataStagingManager = commandExternalDataStagingManager;
        }

        /// <summary>
        /// Get commands from the HR external data source and populate the command table
        /// </summary>
        /// <returns>string</returns>
        public string GetCommands()
        {
            try
            {
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);

                var hrSystemBaseURL = siteConfig.Node.FirstOrDefault(x => x.Key == TenantConfigKeys.HRSystemBaseURL.ToString()).Value;
                var hrSystemUsername = siteConfig.Node.FirstOrDefault(x => x.Key == TenantConfigKeys.HRSystemUsername.ToString()).Value;
                var hrSystemKey = siteConfig.Node.FirstOrDefault(x => x.Key == TenantConfigKeys.HRSystemKey.ToString()).Value;

                string[] requiredParameters = { hrSystemBaseURL, hrSystemUsername, hrSystemKey };

                if (requiredParameters.Any(x => string.IsNullOrEmpty(x)))
                {
                    //throw exception
                    throw new Exception("Required parameter(s) for HR system not found");
                }

                string signature = $"{hrSystemUsername}{hrSystemKey}::";
                string encodedSignature = Util.SHA256ManagedHash(signature);
                string url = $"{hrSystemBaseURL}/commandcategory/{hrSystemUsername}/{encodedSignature}";

                IRemoteClient _remoteClient = new RemoteClient.RemoteClient();
                string response = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = null,
                    Model = null,
                    URL = url
                }, HttpMethod.Get, new Dictionary<string, string>());

                RootHRSystemResponse commandResponse = JsonConvert.DeserializeObject<RootHRSystemResponse>(response);

                if (!commandResponse.Error)
                {
                    try
                    {
                        List<CommandReportRecordModel> commands = JsonConvert.DeserializeObject<CommandResponseModel>(JsonConvert.SerializeObject(commandResponse.ResponseObject)).ReportRecords;

                        string callerDescription = "Called the HR system to get the list of commands.";
                        int callerLogId = _commandExternalDataStagingManager.GetExternalDataCallerLog(url, callerDescription);
                        ICollection<CommandExternalDataStaging> commandList = new List<CommandExternalDataStaging>();
                        foreach (CommandReportRecordModel command in commands)
                        {
                            switch (command.Name)
                            {
                                case "FORCE HEADQUARTERS":
                                    GetFHQCommandLineItem(command.Sub, callerLogId, ref commandList);
                                    break;
                                case "ZONE":
                                    GetZonalCommandLineItem(command.Sub, callerLogId, ref commandList);
                                    break;
                                case "STATE COMMAND":
                                    GetStateCommandLineItem(command.Sub, callerLogId, ref commandList);
                                    break;
                            }
                        }

                        bool saveResponse = _commandExternalDataStagingManager.SaveBundle(commandList);
                        if (!saveResponse)
                        {
                            throw new Exception("Unable to save commands records");
                        }

                        //Update the HR statecode and lgacode to match POSSAP state and lga
                        _commandExternalDataStagingManager.UpdateStateAndLGA();

                        ///Update records in command table that matches the code in CommandExternalDataStaging
                        _commandExternalDataStagingManager.UpdateRecordInCommandTable(callerLogId);

                        //Create the records that exist in CommandExternalDataStaging but doesn't exist in command
                        _commandExternalDataStagingManager.CreateNewRecordInCommandTable(callerLogId);

                        ///Update zonal command in command table that matches the zonalcode in CommandExternalDataStaging
                        _commandExternalDataStagingManager.UpdateZonalCodeInCommandTable(callerLogId);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception.Message, exception);
                        throw;
                    }
                }
                else
                {
                    List<HRErrorResponseModel> errorMessagesObject = JsonConvert.DeserializeObject<List<HRErrorResponseModel>>(JsonConvert.SerializeObject(commandResponse.ResponseObject));
                    string errorMessage = string.Join(" : ", errorMessagesObject.Select(p => $"{p.FieldName} {p.ErrorMessage}"));

                    Exception exception = new Exception($"Call to hr external system failed and returned error code : {commandResponse.ErrorCode} {errorMessage}");
                    Logger.Error(commandResponse.ErrorCode, exception);
                    throw exception;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
            return "Successful";
        }

        private ICollection<CommandExternalDataStaging> GetFHQCommandLineItem(List<CommandReportRecordModel> commands, int callerLogId, ref ICollection<CommandExternalDataStaging> commandList)
        {
            foreach (CommandReportRecordModel levelOneCommand in commands)
            {
                string FHQCode = "1";
                CommandExternalDataStaging commandObj = null;
                if (levelOneCommand.StateCode != "0" && levelOneCommand.LgaCode != "0")
                {
                    //CommandCategory for FHQ is 1
                    dynamic validateResponse = ValidateNameAndCode(levelOneCommand);
                    commandObj = new CommandExternalDataStaging { Name = levelOneCommand.Name, Code = $"{FHQCode}-{levelOneCommand.Code}", StateCode = levelOneCommand.StateCode, Address = levelOneCommand.Address, LGACode = levelOneCommand.LgaCode, CommandCategoryId = 1, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = FHQCode, ZonalCode = levelOneCommand.ZoneCode != "0" ? $"2-{levelOneCommand.ZoneCode}" : null };
                    commandList.Add(commandObj);
                }

                if (levelOneCommand.Sub != null)
                {
                    foreach (CommandReportRecordModel command in levelOneCommand.Sub)
                    {
                        CommandExternalDataStaging commandSubObj = null;
                        if (command.StateCode != "0" && command.LgaCode != "0")
                        {
                            //CommandCategory for departments under FHQ is 6
                            dynamic validateResponse = ValidateNameAndCode(command);
                            commandSubObj = new CommandExternalDataStaging { Name = $"{levelOneCommand.Name} - {command.Name}", Code = $"{FHQCode}-{levelOneCommand.Code}-{command.Code}", StateCode = command.StateCode, Address = command.Address, LGACode = command.LgaCode, CommandCategoryId = 6, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = $"{FHQCode}-{levelOneCommand.Code}", ZonalCode = command.ZoneCode != "0" ? $"2-{command.ZoneCode}" : null };
                            commandList.Add(commandSubObj);
                        }
                        if (command.Sub != null)
                        {
                            foreach (CommandReportRecordModel commandSub in command.Sub)
                            {
                                CommandExternalDataStaging commandSubSubObj = null;
                                if (commandSub.StateCode != "0" && commandSub.LgaCode != "0")
                                {
                                    //CommandCategory for sections under FHQ is 7
                                    dynamic validateResponse = ValidateNameAndCode(commandSub);
                                    commandSubSubObj = new CommandExternalDataStaging { Name = $"{command.Name} - {commandSub.Name}", Code = $"{FHQCode}-{levelOneCommand.Code}-{command.Code}-{commandSub.Code}", StateCode = commandSub.StateCode, Address = commandSub.Address, LGACode = commandSub.LgaCode, CommandCategoryId = 7, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = $"{FHQCode}-{levelOneCommand.Code}-{command.Code}", ZonalCode = commandSub.ZoneCode != "0" ? $"2-{commandSub.ZoneCode}" : null };
                                    commandList.Add(commandSubSubObj);
                                }

                                if (commandSub.Sub != null)
                                {
                                    foreach (CommandReportRecordModel commandSubSub in commandSub.Sub)
                                    {
                                        CommandExternalDataStaging commandSubSubSubObj = null;
                                        if (commandSubSub.StateCode != "0" && commandSubSub.LgaCode != "0")
                                        {
                                            //CommandCategory for sections under FHQ is 8
                                            dynamic validateResponse = ValidateNameAndCode(commandSubSub);
                                            commandSubSubSubObj = new CommandExternalDataStaging { Name = $"{commandSub.Name} - {commandSubSub.Name}", Code = $"{FHQCode}-{levelOneCommand.Code}-{command.Code}-{commandSub.Code}-{commandSubSub.Code}", StateCode = commandSubSub.StateCode, Address = commandSubSub.Address, LGACode = commandSubSub.LgaCode, CommandCategoryId = 8, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = $"{FHQCode}-{levelOneCommand.Code}-{command.Code}-{commandSub.Code}", ZonalCode = commandSubSub.ZoneCode != "0" ? $"2-{commandSubSub.ZoneCode}" : null };
                                            commandList.Add(commandSubSubSubObj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return commandList;
        }

        private ICollection<CommandExternalDataStaging> GetZonalCommandLineItem(List<CommandReportRecordModel> commands, int callerLogId, ref ICollection<CommandExternalDataStaging> commandList)
        {
            foreach (CommandReportRecordModel levelOneCommand in commands)
            {
                string ZonalCode = "2";
                CommandExternalDataStaging commandObj = null;
                if (levelOneCommand.StateCode != "0" && levelOneCommand.LgaCode != "0")
                {
                    //CommandCategory for Zonal is 2
                    dynamic validateResponse = ValidateNameAndCode(levelOneCommand);
                    commandObj = new CommandExternalDataStaging { Name = levelOneCommand.Name, Code = $"{ZonalCode}-{levelOneCommand.Code}", StateCode = levelOneCommand.StateCode, Address = levelOneCommand.Address, LGACode = levelOneCommand.LgaCode, CommandCategoryId = 2, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = ZonalCode };
                    commandList.Add(commandObj);
                }
                if (levelOneCommand.Sub != null)
                {
                    foreach (CommandReportRecordModel command in levelOneCommand.Sub)
                    {
                        CommandExternalDataStaging commandSubObj = null;
                        if (command.StateCode != "0" && command.LgaCode != "0")
                        {
                            //CommandCategory for departments under Zonal is 9
                            dynamic validateResponse = ValidateNameAndCode(command);
                            commandSubObj = new CommandExternalDataStaging { Name = $"{levelOneCommand.Name} - {command.Name}", Code = $"{ZonalCode}-{levelOneCommand.Code}-{command.Code}", StateCode = command.StateCode, Address = command.Address, LGACode = command.LgaCode, CommandCategoryId = 9, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = $"{ZonalCode}-{levelOneCommand.Code}" };
                            commandList.Add(commandSubObj);
                        }
                        if (command.Sub != null)
                        {
                            foreach (CommandReportRecordModel commandSubSub in command.Sub)
                            {
                                CommandExternalDataStaging commandSubSubObj = null;
                                if (commandSubSub.StateCode != "0" && commandSubSub.LgaCode != "0")
                                {
                                    //CommandCategory for units under Zonal is 10
                                    dynamic validateResponse = ValidateNameAndCode(commandSubSub);
                                    commandSubSubObj = new CommandExternalDataStaging { Name = $"{command.Name} - {commandSubSub.Name}", Code = $"{ZonalCode}-{levelOneCommand.Code}-{command.Code}-{commandSubSub.Code}", StateCode = commandSubSub.StateCode, Address = commandSubSub.Address, LGACode = commandSubSub.LgaCode, CommandCategoryId = 10, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = $"{ ZonalCode }-{ levelOneCommand.Code }-{ command.Code }" };
                                    commandList.Add(commandSubSubObj);
                                }
                            }
                        }
                    }
                }
            }
            return commandList;
        }

        private ICollection<CommandExternalDataStaging> GetStateCommandLineItem(List<CommandReportRecordModel> commands, int callerLogId, ref ICollection<CommandExternalDataStaging> commandList)
        {
            foreach (CommandReportRecordModel levelOneCommand in commands)
            {
                string StateCode = "3";
                CommandExternalDataStaging commandObj = null;
                if (levelOneCommand.StateCode != "0" && levelOneCommand.LgaCode != "0")
                {
                    //CommandCategory for state command is 3
                    dynamic validateResponse = ValidateNameAndCode(levelOneCommand);
                    commandObj = new CommandExternalDataStaging { Name = levelOneCommand.Name, Code = $"{StateCode}-{levelOneCommand.Code}", StateCode = levelOneCommand.StateCode, Address = levelOneCommand.Address, LGACode = levelOneCommand.LgaCode, CommandCategoryId = 3, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = StateCode, ZonalCode = levelOneCommand.ZoneCode != "0"? $"2-{levelOneCommand.ZoneCode}" : null };
                    commandList.Add(commandObj);
                }
                if (levelOneCommand.Sub != null)
                {
                    foreach (CommandReportRecordModel command in levelOneCommand.Sub)
                    {
                        CommandExternalDataStaging commandSubObj = null;
                        if (command.StateCode != "0" && command.LgaCode != "0")
                        {
                            dynamic validateResponse = ValidateNameAndCode(command);
                            //CommandCategory for area command is 4, while divisional command and departments under state is 5
                            if (!command.Name.Replace("'", "''").Contains("DIVISION"))
                            {
                                commandSubObj = new CommandExternalDataStaging { Name = $"{levelOneCommand.Name} - {command.Name}", Code = $"{StateCode}-{levelOneCommand.Code}-{command.Code}", StateCode = command.StateCode, Address = command.Address, LGACode = command.LgaCode, CommandCategoryId = 4, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = $"{StateCode}-{levelOneCommand.Code}", ZonalCode = command.ZoneCode != "0" ? $"2-{command.ZoneCode}" : null };
                                commandList.Add(commandSubObj);

                                if (command.Sub != null)
                                {
                                    foreach (CommandReportRecordModel commandSub in command.Sub)
                                    {
                                        if (commandSub.StateCode != "0" && commandSub.LgaCode != "0")
                                        {
                                            validateResponse = ValidateNameAndCode(commandSub);
                                            commandSubObj = new CommandExternalDataStaging { Name = $"{command.Name} - {commandSub.Name}", Code = $"{StateCode}-{levelOneCommand.Code}-{command.Code}-{commandSub.Code}", StateCode = commandSub.StateCode, Address = commandSub.Address, LGACode = commandSub.LgaCode, CommandCategoryId = 5, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = $"{StateCode}-{levelOneCommand.Code}-{command.Code}", ZonalCode = command.ZoneCode != "0" ? $"2-{command.ZoneCode}" : null };
                                            commandList.Add(commandSubObj);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                commandSubObj = new CommandExternalDataStaging { Name = $"{levelOneCommand.Name} - {command.Name}", Code = $"{StateCode}-{levelOneCommand.Code}-{command.Code}", StateCode = command.StateCode, Address = command.Address, LGACode = command.LgaCode, CommandCategoryId = 5, AddedBy = 2, LastUpdatedBy = 2, CommandTypeId = 1, CallLogForExternalSystemId = callerLogId, HasError = validateResponse.HasError, ErrorMessage = validateResponse.ErrorMessage, ParentCode = $"{StateCode}-{levelOneCommand.Code}", ZonalCode = command.ZoneCode != "0" ? $"2-{command.ZoneCode}" : null };
                                commandList.Add(commandSubObj);
                            }
                        }
                    }
                }
            }
            return commandList;
        }

        /// <summary>
        /// Validate the command name and command code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private dynamic ValidateNameAndCode(CommandReportRecordModel model)
        {
            bool hasError = false;
            string errorMessage = string.Empty;
            if (string.IsNullOrEmpty(model.Name) || model.Name.Trim().Length == 0)
            {
                hasError = true;
                errorMessage = "Name is not valid |";
            }
            if (string.IsNullOrEmpty(model.Code) || model.Code.Trim().Length == 0 || model.Code == "0")
            {
                hasError = true;
                errorMessage += "Code is not valid |";
            }

            if (hasError) { errorMessage = errorMessage.TrimEnd(new char[] { '|' }); }
            return new { HasError = hasError, ErrorMessage = errorMessage };
        }
    }

}