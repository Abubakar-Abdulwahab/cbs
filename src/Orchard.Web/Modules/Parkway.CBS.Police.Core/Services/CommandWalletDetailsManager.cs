using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class CommandWalletDetailsManager : BaseManager<CommandWalletDetails>, ICommandWalletDetailsManager<CommandWalletDetails>
    {
        private readonly IRepository<CommandWalletDetails> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public CommandWalletDetailsManager(IRepository<CommandWalletDetails> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get command wallet details using the commandId <paramref name="commandId"/>
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns><see cref="CommandWalletDetailsVM"/></returns>
        public CommandWalletDetailsVM GetCommandWalletDetailsByCommandId(int commandId)
        {
            return _transactionManager.GetSession().Query<CommandWalletDetails>().Where(cm => cm.Command == new Command { Id = commandId })
               .Select(cm => new CommandWalletDetailsVM { Id = cm.Id, AccountNumber = cm.AccountNumber, BankCode = cm.BankCode, IsActive = cm.IsActive }).FirstOrDefault();
        }

        /// <summary>
        /// Get command wallet details using the commandCode <paramref name="commandCode"/>
        /// </summary>
        /// <param name="commandCode"></param>
        /// <returns><see cref="CommandWalletDetailsVM"/></returns>
        public CommandWalletDetailsVM GetCommandWalletDetailsByCommandCode(string commandCode)
        {
            try
            {
                var queryString = $"SELECT CWD.AccountNumber, CM.Name FROM Parkway_CBS_Police_Core_Command as CM " +
                    $"INNER JOIN Parkway_CBS_Police_Core_CommandWalletDetails as CWD ON CM.Id = CWD.Command_Id WHERE CM.Code = '{commandCode}'";

                return _transactionManager.GetSession().CreateSQLQuery(queryString).SetResultTransformer(Transformers.AliasToBean<CommandWalletDetailsVM>()).List<CommandWalletDetailsVM>().FirstOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, $"Exception getting the command wallet details for command code {commandCode}");
                throw;
            }
        }

        /// <summary>
        /// Checks if a account number exists
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns>boolean</returns>
        public bool CheckIfCommandWalletExist(string accountNumber)
        {
            return _transactionManager.GetSession().Query<CommandWalletDetails>().Count(cmwd => cmwd.AccountNumber == accountNumber.Trim() && cmwd.IsActive) > 0;
        }

        /// <summary>
        /// Checks if specified command already has the specified account type
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="accountType"></param>
        /// <returns>boolean</returns>

        public bool CheckIfCommandWalletAccountTypeExist(int commandId, int accountType)
        {
            return _transactionManager.GetSession().Query<CommandWalletDetails>().Count(cmwd => cmwd.Command == new Command { Id = commandId } && cmwd.SettlementAccountType == accountType && cmwd.IsActive) > 0;
        }


    }
}