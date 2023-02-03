using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class AccountWalletConfigurationManager : BaseManager<AccountWalletConfiguration>, IAccountWalletConfigurationManager<AccountWalletConfiguration>
    {
        private readonly ITransactionManager _transactionManager;
        public new ILogger Logger { get; set; }

        public AccountWalletConfigurationManager(IRepository<AccountWalletConfiguration> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets the flow definition levels using the <paramref name="acctWalletId"/>
        /// </summary>
        /// <param name="acctWalletId"></param>
        /// <returns></returns>
        public List<PSServiceRequestFlowDefinitionLevelDTO> GetFlowDefinitionLevelByWalletId(int acctWalletId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfiguration>().Where(x => x.Id == acctWalletId && !x.IsDeleted).Select(s => s.FlowDefinition.LevelDefinitions.Select(ld => new PSServiceRequestFlowDefinitionLevelDTO { Id = ld.Id, PositionDescription = ld.PositionDescription, DefinitionName = ld.PositionName, DefinitionId = ld.Definition.Id })).SelectMany(x => x).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the account name
        /// </summary>
        /// <param name="acctWalletId"></param>
        /// <returns></returns>
        public string GetWalletName(int acctWalletId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfiguration>().Where(x => x.Id == acctWalletId && !x.IsDeleted).Select(x => new
                {
                    Name = x.CommandWalletDetail.Command.Name ?? x.PSSFeeParty.Name
                }).FirstOrDefault().Name;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the account number using the <paramref name="accountWalletConfigurationId"/>
        /// </summary>
        /// <param name="accountWalletConfigurationId"></param>
        /// <returns></returns>
        public string GetWalletAccountNumber(int accountWalletConfigurationId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfiguration>().Where(x => x.Id == accountWalletConfigurationId  && !x.IsDeleted).Select(x => new
                {
                    AccountNumber = x.CommandWalletDetail.AccountNumber ?? x.PSSFeeParty.AccountNumber

                }).SingleOrDefault()?.AccountNumber;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get account wallet configuration using <paramref name="accountWalletId"/>
        /// </summary>
        /// <param name="accountWalletId"></param>
        /// <returns></returns>
        public AccountWalletConfigurationDTO GetAccountWalletConfigurationDetail(int accountWalletId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfiguration>().Where(x => x.Id == accountWalletId && !x.IsDeleted).Select(x => new AccountWalletConfigurationDTO
                {
                    AccountName = x.CommandWalletDetail.Command.Name ?? x.PSSFeeParty.Name,
                    AccountNumber = x.CommandWalletDetail.AccountNumber ?? x.PSSFeeParty.AccountNumber,
                    BankId = x.CommandWalletDetail.Bank != null ? x.CommandWalletDetail.Bank.Id : x.PSSFeeParty.Bank.Id,
                    FlowDefinitionId = x.FlowDefinition.Id
                }).FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get account wallet configuration and command details using <paramref name="accountWalletId"/>
        /// </summary>
        /// <param name="accountWalletId"></param>
        /// <returns></returns>
        public AccountWalletConfigurationDTO GetAccountWalletConfigurationDetailWithCommandDetails(int accountWalletId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfiguration>().Where(x => x.Id == accountWalletId && !x.IsDeleted).Select(x => new AccountWalletConfigurationDTO
                {
                    AccountName = x.CommandWalletDetail.Command.Name,
                    AccountNumber = x.CommandWalletDetail.AccountNumber,
                    BankId = x.CommandWalletDetail.Bank.Id,
                    FlowDefinitionId = x.FlowDefinition.Id,
                    CommandId = x.CommandWalletDetail.Command.Id
                }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets command attached to source account with specified id
        /// </summary>
        /// <param name="accountWalletId"></param>
        /// <returns></returns>
        public CommandVM GetCommandAttachedToSourceAccount(int accountWalletId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfiguration>().Where(x => x.Id == accountWalletId && !x.IsDeleted).Select(x => new CommandVM
                {
                    Id = x.CommandWalletDetail.Command.Id
                }).SingleOrDefault();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets the flow defintion Id using <paramref name="acctWalletId"/>
        /// </summary>
        /// <param name="acctWalletId"></param>
        /// <returns></returns>
        public int GetFlowDefinitionByWalletId(int acctWalletId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfiguration>().Where(x => x.Id == acctWalletId && !x.IsDeleted).Select(x => x.FlowDefinition.Id).FirstOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Checks if wallet configuration exist using <paramref name="acctWalletId"/>
        /// </summary>
        /// <param name="acctWalletId"></param>
        /// <returns></returns>
        public bool CheckIfAccountWalletConfigurationExist(int acctWalletId)
        {
            try
            {
                return _transactionManager.GetSession().Query<AccountWalletConfiguration>().Count(x => x.Id == acctWalletId && !x.IsDeleted) > 0;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


    }
}