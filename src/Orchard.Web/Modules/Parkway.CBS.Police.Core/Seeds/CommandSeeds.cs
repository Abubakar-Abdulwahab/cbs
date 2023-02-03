using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Seeds.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Seeds
{
    public class CommandSeeds : ICommandSeeds
    {
        private readonly ICommandManager<Command> _commandManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IStateModelManager<StateModel> _stateManager;
        private readonly ILGAManager<LGA> _lgaManager;
        private readonly ICommandCategoryManager<CommandCategory> _commandCategoryManager;

        public CommandSeeds(IOrchardServices orchardServices, ICommandManager<Command> commandManager, IStateModelManager<StateModel> stateManager, ILGAManager<LGA> lgaManager, ICommandCategoryManager<CommandCategory> commandCategoryManager)
        {
            _orchardServices = orchardServices;
            _commandManager = commandManager;
            _commandCategoryManager = commandCategoryManager;
            _stateManager = stateManager;
            _lgaManager = lgaManager;
        }

        public CommandStatVM AddCommands(List<CommandVM> commands)
        {
            try
            {
                int countersuccessful = 0;
                int counterunsuccessful = 0;
                List<CommandVM> unsuccessfulList = new List<CommandVM>();
                List<Command> commandList = new List<Command>();

                foreach (var command in commands)
                {
                    var state = _stateManager.GetState(command.StateId);
                    var lga = _lgaManager.GetLGA(command.LGAId);
                    var commandCategory = _commandCategoryManager.GetCategory(command.CommandCategoryId);

                    if (state.FirstOrDefault() == null || lga.FirstOrDefault() == null || commandCategory.FirstOrDefault() == null)
                    {
                        counterunsuccessful++;
                        unsuccessfulList.Add(command);
                    }else
                    {
                        Command commandObj = new Command();
                        commandObj.Name = command.Name;
                        commandObj.Code = command.Code;
                        commandObj.CommandCategory = new CommandCategory { Id = command.CommandCategoryId };
                        commandObj.State = new StateModel { Id = command.StateId };
                        commandObj.LGA = new LGA { Id = command.LGAId };
                        commandObj.IsActive = true;
                        commandObj.Address = command.Address;
                        commandObj.LastUpdatedBy = new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id };
                        commandObj.AddedBy = new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id };
                        commandList.Add(commandObj);
                        countersuccessful++;
                    }
                }
                _commandManager.SaveBundleUnCommitStateless(commandList);

                return new CommandStatVM { NumberofSuccessfulRecords = countersuccessful, NumberofUnSuccessfulRecords = counterunsuccessful, UnSuccessfulRecords = unsuccessfulList };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}