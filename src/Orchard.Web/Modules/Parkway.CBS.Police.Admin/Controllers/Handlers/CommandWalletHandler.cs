using Orchard.Logging;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class CommandWalletHandler : ICommandWalletHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly ICoreCommand _coreCommand;
        private readonly ICommandWalletDetailsManager<CommandWalletDetails> _commandWalletDetailsManager;
        private readonly IBankManager<Bank> _bankManager;

        ILogger Logger { get; set; }

        public CommandWalletHandler(IHandlerComposition handlerComposition, ICoreCommand coreCommand, ICommandWalletDetailsManager<CommandWalletDetails> commandWalletDetailsManager, IBankManager<Bank> bankManager)
        {
            _handlerComposition = handlerComposition;
            _coreCommand = coreCommand;
            _commandWalletDetailsManager = commandWalletDetailsManager;
            Logger = NullLogger.Instance;
            _bankManager = bankManager;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canCreateCommandWallet"></param>
        public void CheckForPermission(Permission canCreateCommandWallet)
        {
            _handlerComposition.IsAuthorized(canCreateCommandWallet);
        }

        /// <summary>
        /// Gets the view model for the get add command wallet
        /// </summary>
        /// <returns></returns>
        public AddCommandWalletVM GetAddCommandWalletVM()
        {
            return new AddCommandWalletVM
            {
                Commands = _coreCommand.GetCommands(),
                Banks = _bankManager.GetAllActiveBanks()
            };
        }

        /// <summary>
        /// Validates and Creates command wallet in <see cref="CommandWalletDetails"/>
        /// </summary>
        /// <param name="errors"> Validation errors</param>
        /// <param name="model">User input model</param>
        public void AddCommandWallet(ref List<ErrorModel> errors, AddCommandWalletVM model)
        {
            try
            {

                #region Validations

                if (model.SelectedCommandId <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select a command", FieldName = nameof(model.SelectedCommandId) });
                    throw new DirtyFormDataException();
                }

                if (string.IsNullOrEmpty(model.WalletNumber))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly enter a wallet number", FieldName = nameof(model.WalletNumber) });
                    throw new DirtyFormDataException();
                }

                if (!model.WalletNumber.All(char.IsDigit))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Only digits are allowed for wallet number", FieldName = nameof(model.WalletNumber) });
                    throw new DirtyFormDataException();
                }

                if (model.WalletNumber.Trim().Length != 10)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Wallet number must be 10 digits", FieldName = nameof(model.WalletNumber) });
                    throw new DirtyFormDataException();
                }

                if (model.SelectedBankId <= 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select a bank", FieldName = nameof(model.SelectedBankId) });
                    throw new DirtyFormDataException();
                }

                if (_commandWalletDetailsManager.CheckIfCommandWalletExist(model.WalletNumber))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Wallet number already exist", FieldName = nameof(model.WalletNumber) });
                    throw new DirtyFormDataException();
                }

                if (!_coreCommand.CheckIfCommandExist(model.SelectedCommandId))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select valid the formation/command", FieldName = nameof(model.SelectedCommandId) });
                    throw new DirtyFormDataException();
                }

                if (_commandWalletDetailsManager.CheckIfCommandWalletAccountTypeExist(model.SelectedCommandId, (int)model.SelectedAccountType))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Specified account type already exist for this command", FieldName = nameof(model.WalletNumber) });
                    throw new DirtyFormDataException();
                }


                BankViewModel bankVM = _bankManager.GetActiveBankByBankId(model.SelectedBankId);

                if (bankVM == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select a valid bank", FieldName = nameof(model.SelectedBankId) });
                    throw new DirtyFormDataException();
                }

                #endregion


                if (!_commandWalletDetailsManager.Save(new CommandWalletDetails
                {
                    AccountNumber = model.WalletNumber,
                    BankCode = bankVM.Code,
                    Bank = new Bank { Id = bankVM.Id },
                    IsActive = true,
                    Command = new Command { Id = model.SelectedCommandId },
                    SettlementAccountType = (int)model.SelectedAccountType
                }))
                {
                    throw new CouldNotSaveRecord();
                };
            }
            catch (System.Exception exception)
            {

                _commandWalletDetailsManager.RollBackAllTransactions();
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}