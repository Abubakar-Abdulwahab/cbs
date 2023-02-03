using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class CommandManager : BaseManager<Command>, ICommandManager<Command>
    {
        private readonly IRepository<Command> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public CommandManager(IRepository<Command> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Get commands by state Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetCommandsByState(int stateId)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => cm.State == new StateModel { Id = stateId })
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, });
        }


        /// <summary>
        /// Get commands by LGA Id
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>List{CommandVM}</returns>
        public List<CommandVM> GetCommandsByLGA(int lgaid)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => cm.LGA == new LGA { Id = lgaid })
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, }).ToList();
        }

        /// <summary>
        /// Get area and divisional commands by LGA Id
        /// </summary>
        /// <param name="lgaid"></param>
        /// <returns>List{CommandVM}</returns>
        public List<CommandVM> GetAreaAndDivisionalCommandsByLGA(int lgaid)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => cm.LGA == new LGA { Id = lgaid } && (cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Area || cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Divisional))
             .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, }).ToList();
        }

        /// <summary>
        /// Get area and divisional commands by State Id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>List{CommandVM}</returns>
        public List<CommandVM> GetAreaAndDivisionalCommandsByStateId(int stateId)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => cm.State == new StateModel { Id = stateId } && (cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Area  || cm.CommandCategory.CategoryLevel ==  (int)PSSCommandCategoryLevel.Divisional))
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, }).ToList();
        }

        /// <summary>
        /// Gets area and divisional commands for state with specified id and optional LGA with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns>returns all area and divisional commands for specified state if no LGA is specified</returns>
        public IEnumerable<CommandVM> GetAreaAndDivisionalCommandsByStateAndLGA(int stateId, int lgaId)
        {
            return (lgaId > 0) ? _transactionManager.GetSession().Query<Command>().Where(cm => (cm.State == new StateModel { Id = stateId }) && cm.LGA == new LGA { Id = lgaId } && ((cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Area) || (cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Divisional))).OrderBy(cm => cm.Name).Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, }) : _transactionManager.GetSession().Query<Command>().Where(cm => (cm.State == new StateModel { Id = stateId }) && ((cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Area) || (cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Divisional))).OrderBy(cm => cm.Name)
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, });
        }

        /// <summary>
        /// Get the list of commands for the specified command category
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>List{CommandVM}</returns>
        public List<CommandVM> GetCommandsByCommandCategory(int commandCategoryId)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => cm.CommandCategory == new CommandCategory { Id = commandCategoryId })
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, }).ToList();
        }

        /// <summary>
        /// Get command
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetCommandDetails(int commandId)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => cm.Id == commandId && cm.IsActive)
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, LGAName = cm.LGA.Name, StateName = cm.State.Name, CommandCategoryId = cm.CommandCategory.Id, LGAId = cm.LGA.Id, StateId = cm.State.Id, Address = cm.Address }).FirstOrDefault();
        }

        /// <summary>
        /// Checks if a command exists
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        public bool CheckIfCommandExist(int commandId)
        {
            return _transactionManager.GetSession().Query<Command>().Count(cm => cm.Id == commandId && cm.IsActive) > 0;
        }

        /// <summary>
        /// Get the state command
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetStateCommandDetails(int stateId)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => (cm.State == new StateModel { Id = stateId } && cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.State))
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, LGAName = cm.LGA.Name, StateName = cm.State.Name, CommandCategoryId = cm.CommandCategory.Id, LGAId = cm.LGA.Id, StateId = cm.State.Id, Address = cm.Address }).SingleOrDefault();
        }


        /// <summary>
        /// Get the federal level command
        /// </summary>
        /// <returns>CommandVM</returns>
        public CommandVM GetFederalLevelCommand()
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => (cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Force))
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, LGAName = cm.LGA.Name, StateName = cm.State.Name, CommandCategoryId = cm.CommandCategory.Id, LGAId = cm.LGA.Id, StateId = cm.State.Id, Address = cm.Address }).SingleOrDefault();
        }

        /// <summary>
        /// Gets the inspector general of the police command
        /// </summary>
        /// <returns></returns>
        public CommandVM GetIGPOfficeCommand()
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => (cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Force))
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, LGAName = cm.LGA.Name, StateName = cm.State.Name, CommandCategoryId = cm.CommandCategory.Id, LGAId = cm.LGA.Id, StateId = cm.State.Id, Address = cm.Address }).OrderBy(x => x.Code).First();
        }


        /// <summary>
        /// Get list of commands
        /// </summary>
        /// <returns>List{CommandVM}</returns>
        public List<CommandVM> GetCommands()
        {
            return _transactionManager.GetSession().Query<Command>()
               .Select(cm => new CommandVM { Id = cm.Id, Name = cm.Name, }).ToList();
        }


        /// <summary>
        /// Get command with the specified code
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns>CommandVM</returns>
        public CommandVM GetCommandWithCode(string code)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => cm.Code == code)
               .Select(cm => new CommandVM { Id = cm.Id, Code = cm.Code, Name = cm.Name, CommandCategoryId = cm.CommandCategory.Id }).SingleOrDefault();
        }


        /// <summary>
        /// Gets Criminal Investigation Department for specified state
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public CommandVM GetStateCID(int stateId)
        {
            return _transactionManager.GetSession().Query<Command>().Where(cm => cm.State == new StateModel { Id = stateId } && cm.Name.Like("%CRIMINAL%") && cm.CommandCategory.CategoryLevel == (int)PSSCommandCategoryLevel.Area).Select(cm => new CommandVM { Id = cm.Id, Name = cm.Name, Code = cm.Code, Address = cm.Address, LGAName = cm.LGA.Name, StateId = cm.State.Id, StateName = cm.State.Name }).SingleOrDefault();
        }


        /// <summary>
        /// Gets commands for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetCommandsForCommandTypeWithId(int commandTypeId)
        {
            return _transactionManager.GetSession().Query<Command>().Where(x => x.CommandType == new CommandType { Id = commandTypeId } && x.IsActive).OrderByDescending(x => x.Id).Select(x => new CommandVM
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                CommandTypeId = x.CommandType.Id
            });
        }


        /// <summary>
        /// Gets command with specified id for command type with specified id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public CommandVM GetCommandForCommandTypeWithId(int commandTypeId, int commandId)
        {
            return _transactionManager.GetSession().Query<Command>().Where(x => x.Id == commandId && x.CommandType == new CommandType { Id = commandTypeId } && x.IsActive).Select(x => new CommandVM
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
            }).SingleOrDefault();
        }


        /// <summary>
        /// Gets next level commands using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetNextLevelCommandsWithCode(string code)
        {
            return _transactionManager.GetSession().Query<Command>().Where(x => x.Code.Like($"%{code}-%") && x.IsActive).OrderBy(x => x.Name).Select(x => new CommandVM
            {
                Id = x.Id,
                Name = $"{x.Name} - {x.Address}, {x.LGA.State.Name}"
            });
        }


        /// <summary>
        /// Gets next level commands for state with specified id using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetNextLevelCommandsWithCodeForState(int stateId, string code)
        {
            return _transactionManager.GetSession().Query<Command>().Where(x => x.Code.Like($"{code}-%") && x.IsActive && x.State == new StateModel { Id = stateId }).Select(x => new CommandVM
            {
                Id = x.Id,
                Name = x.Name
            });
        }


        /// <summary>
        /// Gets next level area and divisional commands for LGA with specified id using specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetNextLevelAreaAndDivisionalCommandsWithCodeForLGA(int lgaId, string code)
        {
            return _transactionManager.GetSession().Query<Command>().Where(x => x.Code.Like($"{code}-%") && x.IsActive && x.LGA == new LGA { Id = lgaId } && ((x.CommandCategory.Id == 4) || (x.CommandCategory.Id == 5))).Select(x => new CommandVM
            {
                Id = x.Id,
                Name = x.Name
            });
        }


        /// <summary>
        /// Gets commands with the specified parent code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public IEnumerable<CommandVM> GetCommandsByParentCode(string parentCode)
        {
            return _transactionManager.GetSession().Query<Command>().Where(x => x.ParentCode == parentCode).Select(x => new CommandVM { Id = x.Id,  Name = x.Name, Code = x.Code, StateId = x.State.Id, LGAId = x.LGA.Id, CommandCategoryId = x.CommandCategory.Id });
        }
    }
}