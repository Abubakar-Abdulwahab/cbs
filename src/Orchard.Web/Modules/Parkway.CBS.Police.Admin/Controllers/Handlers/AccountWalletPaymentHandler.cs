using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class AccountWalletPaymentHandler : IAccountWalletPaymentHandler
    {
        private readonly IBankManager<Bank> _bankManager;
        private readonly IPSSExpenditureHeadManager<PSSExpenditureHead> _expenditureHeadManager;
        private readonly IAccountWalletConfigurationManager<AccountWalletConfiguration> _accountWalletConfigurationManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _serviceRequestFlowDefinitionLevelManager;
        private readonly IAccountPaymentRequestManager<AccountPaymentRequest> _accountPaymentRequestManager;
        private readonly IAccountPaymentRequestItemManager<AccountPaymentRequestItem> _accountPaymentRequestItemManager;
        private readonly IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover> _walletUserConfigurationPSServiceRequestFlowApproverManager;
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;

        ILogger Logger { get; set; }

        public AccountWalletPaymentHandler(IHandlerComposition handlerComposition, IBankManager<Bank> bankManager, IPSSExpenditureHeadManager<PSSExpenditureHead> expenditureHeadManager, IAccountWalletUserConfigurationPSServiceRequestFlowApproverManager<AccountWalletConfigurationPSServiceRequestFlowApprover> walletUserConfigurationPSServiceRequestFlowApproverManager, IAccountPaymentRequestManager<AccountPaymentRequest> accountPaymentRequestManager, IAccountPaymentRequestItemManager<AccountPaymentRequestItem> accountPaymentRequestItemManager, IOrchardServices orchardServices, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> serviceRequestFlowDefinitionLevelManager, IAccountWalletConfigurationManager<AccountWalletConfiguration> accountWalletConfigurationManager)
        {
            _handlerComposition = handlerComposition;
            Logger = NullLogger.Instance;
            _bankManager = bankManager;
            _expenditureHeadManager = expenditureHeadManager;
            _walletUserConfigurationPSServiceRequestFlowApproverManager = walletUserConfigurationPSServiceRequestFlowApproverManager;
            _accountPaymentRequestManager = accountPaymentRequestManager;
            _accountPaymentRequestItemManager = accountPaymentRequestItemManager;
            _orchardServices = orchardServices;
            _serviceRequestFlowDefinitionLevelManager = serviceRequestFlowDefinitionLevelManager;
            _accountWalletConfigurationManager = accountWalletConfigurationManager;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canInitateAccountWalletPayment"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission canInitateAccountWalletPayment)
        {
            _handlerComposition.IsAuthorized(canInitateAccountWalletPayment);
        }

        /// <summary>
        /// Get the view model for adding account wallet configuration
        /// </summary>
        /// <returns><see cref="InitiateAccountWalletPaymentVM"/></returns>
        public InitiateAccountWalletPaymentVM GetInitiateWalletPaymentVM()
        {

            return new InitiateAccountWalletPaymentVM
            {
                Banks = _bankManager.GetAllActiveBanks(),
                ExpenditureHeads = _expenditureHeadManager.GetActiveExpenditureHead(),
                AccountWalletConfigurations = _walletUserConfigurationPSServiceRequestFlowApproverManager.GetWalletsAssignedToUser(_orchardServices.WorkContext.CurrentUser.Id)
            };
        }

        /// <summary>
        /// Validates and Initates wallet payment request
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        public void InitiateWalletPaymentRequest(ref List<ErrorModel> errors, InitiateAccountWalletPaymentVM model)
        {
            try
            {

                ValidateUserInput(errors, model);

                int nextFlowDefinitionLevelId = _serviceRequestFlowDefinitionLevelManager.GetPaymentFirstLevelApprovalDefinition(_accountWalletConfigurationManager.GetFlowDefinitionByWalletId(model.WalletPaymentRequests.FirstOrDefault().SelectedWalletId)).Id;

                int selectedWalletId = model.WalletPaymentRequests.FirstOrDefault().SelectedWalletId;

                Core.DTO.AccountWalletConfigurationDTO walletDetails = _accountWalletConfigurationManager.GetAccountWalletConfigurationDetail(selectedWalletId);

                AccountPaymentRequest accountPaymentRequest = new AccountPaymentRequest
                {
                    AccountWalletConfiguration = new AccountWalletConfiguration { Id = selectedWalletId },
                    InitiatedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                    PaymentRequestStatus = (int)PaymentRequestStatus.AWAITINGAPPROVAL,
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextFlowDefinitionLevelId },
                    Bank = new Bank { Id = walletDetails.BankId},
                    AccountName = walletDetails.AccountName,
                    AccountNumber = walletDetails.AccountNumber,
                };

                if (!_accountPaymentRequestManager.Save(accountPaymentRequest))
                {
                    throw new CouldNotSaveRecord();
                }

               string paymentReference = _accountPaymentRequestManager.GetWalletPaymentReference(accountPaymentRequest.Id);

                DataTable dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(AccountPaymentRequestItem).Name);

                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.AccountPaymentRequest) + "_Id", typeof(long)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.PSSExpenditureHead) + "_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.Bank) + "_Id", typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.AccountName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.AccountNumber), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.Amount), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.BeneficiaryName), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.PaymentReference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.TransactionStatus), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestItem.UpdatedAtUtc), typeof(DateTime)));

                int serialNumber = 1;
                foreach (var paymentRequest in model.WalletPaymentRequests)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(AccountPaymentRequestItem.AccountPaymentRequest) + "_Id"] = accountPaymentRequest.Id;
                    row[nameof(AccountPaymentRequestItem.PSSExpenditureHead) + "_Id"] = paymentRequest.SelectedExpenditureHeadId;
                    row[nameof(AccountPaymentRequestItem.Bank) + "_Id"] = paymentRequest.SelectedBankId;
                    row[nameof(AccountPaymentRequestItem.AccountName)] = paymentRequest.AccountName;
                    row[nameof(AccountPaymentRequestItem.AccountNumber)] = paymentRequest.AccountNumber;
                    row[nameof(AccountPaymentRequestItem.Amount)] = paymentRequest.Amount;
                    row[nameof(AccountPaymentRequestItem.BeneficiaryName)] = paymentRequest.BeneficiaryName;
                    row[nameof(AccountPaymentRequestItem.TransactionStatus)] = (int)PaymentRequestStatus.AWAITINGAPPROVAL;
                    row[nameof(AccountPaymentRequestItem.PaymentReference)] = $"{paymentReference}-{serialNumber}";
                    row[nameof(AccountPaymentRequestItem.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(AccountPaymentRequestItem.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                    serialNumber++;
                }

                if (!_accountPaymentRequestItemManager.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(AccountPaymentRequestItem).Name))
                {
                    throw new CouldNotSaveRecord();

                }

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _accountPaymentRequestManager.RollBackAllTransactions();
                throw;
            }
        }

        private void ValidateUserInput(List<ErrorModel> errors, InitiateAccountWalletPaymentVM model)
        {
            if (model.WalletPaymentRequests != null && model.WalletPaymentRequests.Count > 0)
            {
                foreach (var paymentRequest in model.WalletPaymentRequests)
                {
                    if (string.IsNullOrEmpty(paymentRequest.AccountNumber?.Trim()))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"{nameof(paymentRequest.AccountNumber)} is required", FieldName = nameof(paymentRequest.AccountNumber) });
                        throw new DirtyFormDataException();
                    }

                    if (string.IsNullOrEmpty(paymentRequest.AccountName?.Trim()))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"{nameof(paymentRequest.AccountName)} is required", FieldName = nameof(paymentRequest.AccountName) });
                        throw new DirtyFormDataException();
                    }

                    if (paymentRequest.AccountNumber?.Trim().Length != 10)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"{nameof(paymentRequest.AccountNumber)} must be 10", FieldName = nameof(paymentRequest.AccountNumber) });
                        throw new DirtyFormDataException();
                    }

                    if (!paymentRequest.AccountNumber.Trim().All(char.IsDigit))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"{nameof(paymentRequest.AccountNumber)} must be digits", FieldName = nameof(paymentRequest.AccountNumber) });
                        throw new DirtyFormDataException();
                    }

                    if (paymentRequest.SelectedBankId <= 0)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"{nameof(paymentRequest.Bank)} must be selected", FieldName = nameof(paymentRequest.Bank) });
                        throw new DirtyFormDataException();
                    }
                    if (paymentRequest.SelectedExpenditureHeadId <= 0)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"Expenditure head must be selected", FieldName = nameof(paymentRequest.SelectedExpenditureHead) });
                        throw new DirtyFormDataException();
                    }
                    if (paymentRequest.SelectedWalletId <= 0)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"Source account must be selected", FieldName = nameof(paymentRequest.SelectedExpenditureHead) });
                        throw new DirtyFormDataException();
                    }
                    if (string.IsNullOrEmpty(paymentRequest.BeneficiaryName) || paymentRequest.BeneficiaryName.Trim().Length > 250 || paymentRequest.BeneficiaryName.Trim().Length < 3)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"Beneficiary Name field must be between 3 to 250 characters long.", FieldName = nameof(paymentRequest.BeneficiaryName) });
                        throw new DirtyFormDataException();
                    }
                    if (!_expenditureHeadManager.CheckIExpenditureHeadExist(paymentRequest.SelectedExpenditureHeadId))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"Kindly select a valid Expenditure head.", FieldName = nameof(paymentRequest.SelectedExpenditureHeadId) });
                        throw new DirtyFormDataException();
                    }
                    if (!_bankManager.CheckIfBankExist(paymentRequest.SelectedBankId))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = $"Kindly select a valid Bank.", FieldName = nameof(paymentRequest.SelectedExpenditureHeadId) });
                        throw new DirtyFormDataException();
                    }
                }

                if (!_accountWalletConfigurationManager.CheckIfAccountWalletConfigurationExist(model.WalletPaymentRequests.FirstOrDefault().SelectedWalletId))
                {
                    errors.Add(new ErrorModel { ErrorMessage = $"Kindly select a valid wallet configuration.", FieldName = "SelectedWalletId" });
                    throw new DirtyFormDataException();
                }
            }
            else
            {
                errors.Add(new ErrorModel { ErrorMessage = $"Kindly select users", FieldName = nameof(model.WalletPaymentRequests) });
                throw new DirtyFormDataException();
            }
        }

    }
}